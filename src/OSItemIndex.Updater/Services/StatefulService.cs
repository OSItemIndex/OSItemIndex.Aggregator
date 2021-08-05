using System.Threading;
using System.Threading.Tasks;

namespace OSItemIndex.Updater.Services
{
    public abstract class StatefulService : IStatefulService
    {
        protected bool IsRunning { get; set; } = false;
        public abstract string ServiceName { get; }

        public virtual async Task OnApplicationStartup(CancellationToken cancellationToken)
        {
            IsRunning = true;
            await StartInternalAsync(cancellationToken);
        }

        public virtual Task OnApplicationShutdown(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            IsRunning = true;
            await StartInternalAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            IsRunning = false;
            await StopInternalAsync(cancellationToken);
        }

        public async Task<ServiceState> GetStateAsync(CancellationToken cancellationToken)
        {
            var details = await GetStatusAsync(cancellationToken);
            return new ServiceState
            {
                Name = ServiceName,
                IsEnabled = IsRunning,
                Details = details
            };
        }

        public abstract Task StartInternalAsync(CancellationToken cancellationToken);
        public abstract Task StopInternalAsync(CancellationToken cancellationToken);

        protected virtual Task<object> GetStatusAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(null as object);
        }
    }
}
