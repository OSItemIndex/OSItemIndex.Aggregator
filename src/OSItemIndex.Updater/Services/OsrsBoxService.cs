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

namespace OSItemIndex.Updater.Services
{
    public class OsrsBoxService : StatefulService, IOsrsBoxService, IDisposable
    {
        private Timer _timer;
        private bool _isWorking;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(15);

        private string _version;

        private readonly OsrsBoxClient _client;
        private readonly IDbContextHelper _context;
        private readonly IEventRepository _events;

        public override string ServiceName => "osrsbox";

        public OsrsBoxService(OsrsBoxClient client, IDbContextHelper context, IEventRepository events)
        {
            _isWorking = false;
            _client = client;
            _context = context;
            _events = events;
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
            return Task.FromResult(new { version = _version } as object);
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
            Log.Information("{@Service} > checking remote osrsbox version", ServiceName);

            var project = await _client.GetProjectDetailsAsync();
            if (project == null)
            {
                Log.Fatal("{@Service} > failed to get osrsbox project details, null", ServiceName);
                return;
            }

            Log.Information("{@Service} remote version > {@Version}", ServiceName, project.Version);
            Log.Information("{@Service} local version > {@Version}", ServiceName, _version);

            if (project.Version == _version) // no need to update, TODO
            {
                return;
            }

            _version = project.Version;

            Log.Information("{@Service} requesting items-complete from osrsbox", ServiceName);

            var items = await _client.GetOsrsBoxItemsAsync();
            if (items == null)
            {
                Log.Fatal("{@Service} failed to get osrsbox items, null", ServiceName);
                return;
            }

            await using (var factory = _context.GetFactory())
            {
                try
                {
                    var dbContext = factory.GetDbContext();
                    var uploader = new NpgsqlBulkUploader(dbContext);

                    var entityType = dbContext.Model.FindEntityType(typeof(OsrsBoxItem));
                    var primKeyProp = entityType.FindPrimaryKey().Properties.Select(k => k.PropertyInfo).Single();
                    var props = entityType.GetProperties().Select(o => o.PropertyInfo).ToArray();

                    await uploader.InsertAsync(items, InsertConflictAction.UpdateProperty<OsrsBoxItem>(primKeyProp, props));

                    var itemsEvent = new Event
                    {
                        Type = EventType.Update,
                        Source = EventSource.Items,
                        Details = new { version = _version, rowCount = await dbContext.Items.CountAsync() }
                    };

                    await _events.AddAsync(itemsEvent);
                }
                catch (NpgsqlException e)
                {
                    Log.Fatal(e, "Npgsql fatal failure");
                    throw;
                }
            }
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
