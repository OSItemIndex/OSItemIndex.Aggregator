using System.Collections.Generic;
using System.Threading.Tasks;

namespace OSItemIndex.Updater.Services
{
    public interface IStatefulServiceRepository
    {
        IEnumerable<IStatefulService> Services { get; }

        Task StartServicesAsync();
        Task StopServicesAsync();
        Task<IEnumerable<ServiceState>> GetStatesAsync();
        Task<bool> StartAsync(string serviceName);
        Task<bool> StopAsync(string serviceName);
        Task<ServiceState> GetStateAsync(string serviceName);
    }
}
