using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using OSItemIndex.Aggregator.Utils;
using OSItemIndex.Data;
using OSItemIndex.Data.Database;
using OSItemIndex.Data.Extensions;
using Serilog;

namespace OSItemIndex.Aggregator.Services
{
    public class RealtimePricesService : StatefulService, IRealtimePricesService, IDisposable
    {
        private Timer _timer;
        private Stopwatch _fiveMinuteStopwatch;
        private Stopwatch _oneHourStopwatch;
        private TimeSpan _fiveMinuteUpdateTimestamp = TimeSpan.Zero;
        private TimeSpan _oneHourUpdateTimestamp = TimeSpan.Zero;
        private bool _isWorking;

        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _fiveMinuteInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _oneHourInterval = TimeSpan.FromMinutes(60);

        private readonly RealtimePriceClient _client;
        private readonly IDbContextHelper _dbContextHelper;

        private readonly string _sqlMainTableName;
        private readonly string _sqlTempTableName;
        private readonly IEnumerable<string> _sqlColumnNames;
        private readonly string _sqlLock;

        public override string ServiceName => "realtimeprices";

        public RealtimePricesService(RealtimePriceClient client, IDbContextHelper dbContextHelper)
        {
            _isWorking = false;
            _client = client;
            _dbContextHelper = dbContextHelper;

            // Collect information from our dbmodel so we can dynamically construct our sql statements
            using var factory = _dbContextHelper.GetFactory();
            {
                var dbContext = factory.GetDbContext();

                var model = dbContext.Model;
                var tableEntityType = model.FindEntityType(typeof(RealtimeItemPrice));

                _sqlMainTableName = tableEntityType.GetTableName();
                _sqlTempTableName = $"_temp_{_sqlMainTableName}";
                _sqlColumnNames = new List<string> { "latest", "five_minute", "one_hour" };
                _sqlLock = $"LOCK TABLE {_sqlMainTableName} IN ACCESS EXCLUSIVE MODE";
            }
        }

        public override Task StartInternalAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();

            Log.Information("{@ServiceName} service started", ServiceName);

            _timer = new Timer(ExecuteAsync, null, 0, (int) _interval.TotalMilliseconds);
            _fiveMinuteStopwatch = new Stopwatch();
            _oneHourStopwatch = new Stopwatch();

            return Task.CompletedTask;
        }

        public override Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();
            _fiveMinuteStopwatch?.Reset();
            _oneHourStopwatch?.Reset();

            Log.Information("{@ServiceName} service stopped", ServiceName);

            return Task.CompletedTask;
        }

        private async void ExecuteAsync(object stateInfo)
        {
            if (_isWorking)
                return;

            _isWorking = true;
            Log.Information("{@Service} has started working", ServiceName);

            await Task.WhenAll(AggregateAsync(), AggregateAsync(RealtimeRequest.FiveMinute), AggregateAsync(RealtimeRequest.OneHour));

            Log.Information("{@Service} has finished working", ServiceName); // TODO analytics
            _isWorking = false;
        }

        private static bool CheckStopwatch(Stopwatch stopwatch, TimeSpan interval, TimeSpan offset)
        {
            if (stopwatch.IsRunning)
            {
                var elapsed = stopwatch.Elapsed + offset;
                if (elapsed < interval)
                    return false;
            }
            stopwatch.Restart();
            return true;
        }

        public async Task AggregateAsync() => await AggregateAsync(RealtimeRequest.Latest);
        public async Task AggregateAsync(RealtimeRequest requestType)
        {
            switch (requestType)
            {
                case RealtimeRequest.Latest: break;
                case RealtimeRequest.FiveMinute:
                case RealtimeRequest.OneHour:

                    var stopwatch = requestType is RealtimeRequest.FiveMinute ? _fiveMinuteStopwatch : _oneHourStopwatch;
                    var interval = requestType is RealtimeRequest.FiveMinute ? _fiveMinuteInterval : _oneHourInterval;
                    var timeSinceLastUpdate = requestType is RealtimeRequest.FiveMinute ? _fiveMinuteUpdateTimestamp : _oneHourUpdateTimestamp;

                    if (CheckStopwatch(stopwatch, interval, timeSinceLastUpdate))
                        break;

                    var timeDifference = interval - (stopwatch.Elapsed + timeSinceLastUpdate);
                    Log.Information("{@Service} {@Difference} minutes until next [{@RequestType}] upsert", ServiceName, timeDifference.Minutes, requestType);

                    return;
                default: throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null);
            }

            Log.Information("{@Service} requesting price information [{@RequestType}] from our realtime-prices repository", ServiceName, requestType);
            using var response = await (requestType switch
            {
                RealtimeRequest.Latest => _client.GetRawLatestPricesAsync(),
                RealtimeRequest.FiveMinute => _client.GetRawFiveMinutePricesAsync(),
                RealtimeRequest.OneHour => _client.GetRawOneHourPricesAsync(),
                _ => throw new ArgumentOutOfRangeException(nameof(requestType), requestType, null)
            });
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Log.Error(e, "realtime-prices response status not OK");
                Log.Debug("Response: {@Response}", response);
                throw;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var dbFactory = _dbContextHelper.GetFactory();
            {
                int itemsImported;
                int foundTimestamp;
                var dbContext = dbFactory.GetDbContext();
                var conn = dbContext.Database.GetNpgsqlConnection();
                var connOpenedHere = await conn.EnsureConnectedAsync();
                var transaction = dbContext.EnsureOrStartTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    var modelColumn = _sqlColumnNames.ElementAt((int) requestType);
                    var sqlCreate = $"CREATE TEMP TABLE {_sqlTempTableName} (id INT PRIMARY KEY, {modelColumn} JSON) ON COMMIT DROP";
                    var sqlCopy = $"COPY {_sqlTempTableName} (id, {modelColumn}) FROM STDIN (FORMAT BINARY)";
                    var sqlUpsert = $"INSERT INTO {_sqlMainTableName} (id, {modelColumn}) SELECT * FROM {_sqlTempTableName} ON CONFLICT (id) DO UPDATE SET id=EXCLUDED.id, {modelColumn}=EXCLUDED.{modelColumn}";

                    await conn.ExecuteNonQueryAsync(sqlCreate); // Create temp table
                    await using (var importer = conn.BeginBinaryImport(sqlCopy))
                    {
                        ImportFromContentStream(stream, importer, out itemsImported, out foundTimestamp);
                        await importer.CompleteAsync();
                    }

                    await conn.ExecuteNonQueryAsync(_sqlLock);  // Lock table
                    await conn.ExecuteNonQueryAsync(sqlUpsert); // Upsert into main table
                    if (transaction != null)
                        await transaction.CommitAsync();
                }
                catch
                {
                    try
                    {
                        if (transaction != null)
                            await transaction.RollbackAsync();
                    }
                    catch
                    {
                        // ignored
                    }

                    throw;
                }
                finally
                {
                    if (connOpenedHere)
                        await conn.CloseAsync();
                }

                if (foundTimestamp > 0)
                {
                    // TODO
                    // The Timestamp in the API represents the start of the time block
                    // so the next update will happen in TimeStamp - (Length of time block / interval) - UtcTimeNow
                    // so, timestamp is from 74 minutes ago, minus 60 minutes, it's been 14 minutes since the last update
                    switch (requestType)
                    {
                        default: break;
                        case RealtimeRequest.FiveMinute:
                            _fiveMinuteUpdateTimestamp = (DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds(foundTimestamp)) - _fiveMinuteInterval;
                            break;
                        case RealtimeRequest.OneHour:
                            _oneHourUpdateTimestamp = (DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds(foundTimestamp)) - _oneHourInterval;
                            break;
                    }
                }

                Log.Information("{@Service} upserted {@Inserted} [{@RequestType}] item prices into to our db", ServiceName, itemsImported, requestType);
            }

            static void ImportFromContentStream(Stream stream, NpgsqlBinaryImporter importer, out int imported, out int foundTimestamp)
            {
                imported = 0;
                foundTimestamp = -1;
                using var jSr = new Utf8JsonStreamReader(stream, 32 * 1024);
                while (jSr.Read())
                {
                    if (jSr.CurrentDepth == 1 && jSr.TokenType == JsonTokenType.Number)
                    {
                        foundTimestamp = jSr.GetInt32();
                        continue;
                    }

                    if (jSr.CurrentDepth != 2 || jSr.TokenType != JsonTokenType.PropertyName)
                        continue;

                    if (!int.TryParse(jSr.GetString(), out var id))
                    {
                        Log.Error("Error"); // TODO
                        continue;
                    }

                    jSr.Read();
                    using var doc = jSr.GetJsonDocument();
                    {
                        importer.StartRow();
                        importer.Write(id);
                        importer.Write(doc.RootElement.GetRawText());
                        imported++;
                    }
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
