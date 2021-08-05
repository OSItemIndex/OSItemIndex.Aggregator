using System.Threading;
using System.Threading.Tasks;

namespace OSItemIndex.Updater.Services
{
    public interface IStatefulService
    {
        string ServiceName { get; }

        Task OnApplicationStartup(CancellationToken cancellationToken =  default);
        Task OnApplicationShutdown(CancellationToken cancellationToken = default);
        Task StartAsync(CancellationToken cancellationToken =  default);
        Task StopAsync(CancellationToken cancellationToken =  default);
        Task<ServiceState> GetStateAsync(CancellationToken cancellationToken =  default);
    }
}
