using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
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
    public class OsrsBoxService : StatefulService, IOsrsBoxService, IDisposable
    {
        private Timer _timer;
        private bool _isWorking;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(60);

        private readonly OsrsBoxClient _client;
        private readonly IDbContextHelper _dbContextHelper;

        private readonly string _sqlCreate;
        private readonly string _sqlCopy;
        private readonly string _sqlUpsert;
        private readonly string _sqlLock;

        public override string ServiceName => "osrsbox";

        public OsrsBoxService(OsrsBoxClient client, IDbContextHelper dbContextHelper)
        {
            _isWorking = false;
            _client = client;
            _dbContextHelper = dbContextHelper;

            // Collect information from our dbmodel so we can dynamically construct our sql statements
            using var factory = _dbContextHelper.GetFactory();
            {
                var dbContext = factory.GetDbContext();

                var model = dbContext.Model;
                var tableEntityType = model.FindEntityType(typeof(OsrsBoxItem));
                var tableColumnNames = new List<string>
                {
                    "id",
                    "name",
                    "duplicate",
                    "noted",
                    "placeholder",
                    "stackable",
                    "tradeable_on_ge",
                    "last_updated",
                    "document"
                };

                var tableName = tableEntityType.GetTableName();
                var tempTableName = $"_temp_{tableName}";

                _sqlCreate = $"CREATE TEMP TABLE {tempTableName} (LIKE {tableName} INCLUDING DEFAULTS) ON COMMIT DROP";
                _sqlLock = $"LOCK TABLE {tableName} IN ACCESS EXCLUSIVE MODE";

                var builder = new StringBuilder();

                builder.AppendFormat("COPY {0} (", tempTableName);
                builder.AppendJoin(",", tableColumnNames);
                builder.Append(") FROM STDIN (FORMAT BINARY)");

                _sqlCopy = builder.ToString();

                builder.Clear();
                builder.AppendFormat("INSERT INTO {0} SELECT * FROM {1} ON CONFLICT (id) DO UPDATE SET ", tableName, tempTableName);
                using (var en = tableColumnNames.GetEnumerator())
                {
                    en.MoveNext();

                    if (en.Current != null)
                    {
                        builder.AppendFormat("{0}=EXCLUDED.{0}", en.Current);
                    }

                    while (en.MoveNext())
                    {
                        builder.Append(',');
                        builder.AppendFormat("{0}=EXCLUDED.{0}", en.Current);

                    }
                }
                _sqlUpsert = builder.ToString();
            }
        }

        public override Task StartInternalAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();

            Log.Information("{@ServiceName} service started", ServiceName);

            _timer = new Timer(ExecuteAsync, null, 0, (int) _interval.TotalMilliseconds);

            return Task.CompletedTask;
        }

        public override Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _timer?.Dispose();

            Log.Information("{@ServiceName} service stopped", ServiceName);

            return Task.CompletedTask;
        }

        protected override Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return base.GetStatusAsync(cancellationToken);

            //return new { ActiveQueue = activeCountTask.Result, QueueLength = totalCountTask.Result };
        }

        private async void ExecuteAsync(object stateInfo)
        {
            if (_isWorking)
                return;

            _isWorking = true;
            Log.Information("{@Service} has started working", ServiceName);

            await Task.WhenAll(AggregateAsync());

            Log.Information("{@Service} has finished working", ServiceName); // TODO analytics
            _isWorking = false;
        }

        public async Task AggregateAsync()
        {
            Log.Information("{@Service} requesting items-complete from our osrsbox repository", ServiceName);
            using var response = await _client.GetRawItemsCompleteAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Log.Error(e, "items-complete response status not OK");
                Log.Debug("Response: {@Response}", response);
                throw;
            }

            await using var stream = await response.Content.ReadAsStreamAsync();
            using var dbFactory = _dbContextHelper.GetFactory();
            {
                int itemsImported;
                var dbContext = dbFactory.GetDbContext();
                var conn = dbContext.Database.GetNpgsqlConnection();
                var connOpenedHere = await conn.EnsureConnectedAsync();
                var transaction = dbContext.EnsureOrStartTransaction(IsolationLevel.ReadCommitted);
                try
                {
                    await conn.ExecuteNonQueryAsync(_sqlCreate); // Create temp table
                    await using (var importer = conn.BeginBinaryImport(_sqlCopy))
                    {
                        ImportFromContentStream(stream, importer, out itemsImported);
                        await importer.CompleteAsync();
                    }
                    await conn.ExecuteNonQueryAsync(_sqlLock);   // Lock table
                    await conn.ExecuteNonQueryAsync(_sqlUpsert); // Upsert into main table
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

                Log.Information("{@Service} upserted {@Inserted} items into to our db", ServiceName, itemsImported);
            }

            static void ImportFromContentStream(Stream stream, NpgsqlBinaryImporter importer, out int imported)
            {
                imported = 0;
                using var jSr = new Utf8JsonStreamReader(stream, 32 * 1024);
                while (jSr.Read())
                {
                    if (jSr.CurrentDepth != 1 || jSr.TokenType != JsonTokenType.StartObject)
                        continue;

                    using var doc = jSr.GetJsonDocument();
                    {
                        importer.StartRow();
                        importer.Write(doc.RootElement.GetProperty("id").GetInt32());
                        importer.Write(doc.RootElement.GetProperty("name").GetString());
                        importer.Write(doc.RootElement.GetProperty("duplicate").GetBoolean());
                        importer.Write(doc.RootElement.GetProperty("noted").GetBoolean());
                        importer.Write(doc.RootElement.GetProperty("placeholder").GetBoolean());
                        importer.Write(doc.RootElement.GetProperty("stackable").GetBoolean());
                        importer.Write(doc.RootElement.GetProperty("tradeable_on_ge").GetBoolean());
                        importer.Write(doc.RootElement.GetProperty("last_updated").GetDateTime());
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
