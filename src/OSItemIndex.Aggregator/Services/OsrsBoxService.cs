using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using OSItemIndex.Data;
using Serilog;

namespace OSItemIndex.Aggregator.Services
{
    public class OsrsBoxService : StatefulService, IOsrsBoxService, IDisposable
    {
        private Timer _timer;
        private bool _isWorking;
        private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);

        private readonly OsrsBoxClient _client;

        public override string ServiceName => "osrsbox";

        public OsrsBoxService(OsrsBoxClient client)
        {
            _isWorking = false;
            _client = client;
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
            Log.Information("{@Service} requesting items-complete from osrsbox", ServiceName);
            using var response = await _client.GetOsrsBoxItemsAsync();
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                Log.Error(e, "Response status not OK");
                Log.Debug("{@Response}", response);
                return;
            }

            var items = await response.Content.ReadFromJsonAsync<Dictionary<string, OsrsBoxItem>>();
            await _client.PostEntitiesAsync(items.Values, new EntityRepoVersion(), "/items");
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
