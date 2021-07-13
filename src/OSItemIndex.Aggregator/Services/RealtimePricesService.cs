using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Npgsql.Bulk;
using OSItemIndex.Data;
using OSItemIndex.Data.Database;
using OSItemIndex.Data.Repositories;
using Serilog;

namespace OSItemIndex.Aggregator.Services
{
    public class RealtimePricesService : StatefulService, IRealtimePriceService, IDisposable
    {
        private static readonly SemaphoreSlim Semaphore = new(1);

        private Timer _latestTimer;
        private Timer _fiveMinuteTimer;
        private Timer _oneHourTimer;

        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _fiveMinuteInterval = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _oneHourInterval = TimeSpan.FromMinutes(60);

        private readonly RealtimePriceClient _client;
        private readonly IDbContextHelper _context;
        private readonly IEventRepository _events;

        public override string ServiceName => "realtimeprices";

        private enum RequestType { Latest, FiveMinute, OneHour }

        public RealtimePricesService(RealtimePriceClient client, IDbContextHelper dbContextHelper, IEventRepository events)
        {
            _client = client;
            _context = dbContextHelper;
            _events = events;
        }

        public override Task StartInternalAsync(CancellationToken cancellationToken)
        {
            _latestTimer?.Dispose();
            _fiveMinuteTimer?.Dispose();
            _oneHourTimer?.Dispose();

            Log.Information("{@ServiceName} service started", ServiceName);

            _latestTimer = new Timer(ExecuteAsync, RequestType.Latest, 0, (int) _interval.TotalMilliseconds);
            _fiveMinuteTimer = new Timer(ExecuteAsync, RequestType.FiveMinute, 0, (int) _fiveMinuteInterval.TotalMilliseconds);
            _oneHourTimer = new Timer(ExecuteAsync, RequestType.OneHour, 0, (int) _oneHourInterval.TotalMilliseconds);

            return Task.CompletedTask;
        }

        public override Task StopInternalAsync(CancellationToken cancellationToken)
        {
            _latestTimer?.Dispose();
            _fiveMinuteTimer?.Dispose();
            _oneHourTimer?.Dispose();

            Log.Information("{@ServiceName} service stopped", ServiceName);

            return Task.CompletedTask;
        }

        private async void ExecuteAsync(object stateInfo)
        {
            var type = (RequestType) stateInfo;
            Log.Information("{@Service} [{@RequestType}] has started working", ServiceName, type);
            switch (type)
            {
                case RequestType.Latest:
                    await AggregateAsync<RealtimeItemPrice.LatestPrice>(Endpoints.Realtime.Latest, EventSource.PricesRealtimeLatest);
                    break;
                case RequestType.FiveMinute:
                    await AggregateAsync<RealtimeItemPrice.FiveMinutePrice>(Endpoints.Realtime.FiveMinute, EventSource.PricesRealtimeFiveMinute);
                    break;
                case RequestType.OneHour:
                    await AggregateAsync<RealtimeItemPrice.OneHourPrice>(Endpoints.Realtime.OneHour, EventSource.PricesRealtimeOneHour);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Log.Information("{@Service} [{@RequestType}] has finished working", ServiceName, type); // TODO analytics
        }

        public async Task AggregateAsync<T>(string uri, EventSource source) where T : PriceEntity
        {
            await Semaphore.WaitAsync();

            Log.Information("{@Service} requesting price information [{@RequestType}] from the client",
                            ServiceName,
                            typeof(T).Name);

            var prices = await _client.GetPricesAsync<T>(uri);
            if (prices == null)
            {
                Log.Fatal("{@Service} failed to get prices, null", ServiceName);
                Semaphore.Release();
                return;
            }

            await using (var factory = _context.GetFactory())
            {
                try
                {
                    var dbContext = factory.GetDbContext();
                    var uploader = new NpgsqlBulkUploader(dbContext);

                    var entityType = dbContext.Model.FindEntityType(typeof(T));
                    var primKeyProp = entityType.FindPrimaryKey().Properties.Select(k => k.PropertyInfo).Single();
                    var props = entityType.GetProperties().Select(o => o.PropertyInfo).ToArray();

                    await uploader.InsertAsync(prices, InsertConflictAction.UpdateProperty<T>(primKeyProp, props));

                    var pricesEvent = new Event
                    {
                        Type = EventType.Update,
                        Source = EventSource.Prices | source
                    };

                    await _events.SubmitAsync(pricesEvent);
                }
                catch (NpgsqlException e)
                {
                    Log.Fatal(e, "Npgsql fatal failure");
                    throw;
                }
            }

            Semaphore.Release();
        }

        public void Dispose()
        {
            _latestTimer?.Dispose();
            _fiveMinuteTimer?.Dispose();
            _oneHourTimer?.Dispose();
        }
    }
}
