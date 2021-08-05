using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Serilog;

namespace OSItemIndex.Updater.Services
{
    public class StatefulServiceRepository : IStatefulServiceRepository
    {
        public IEnumerable<IStatefulService> Services { get; }

        public StatefulServiceRepository(IServiceProvider serviceProvider)
        {
            // Finds our IStatefulService implementations, and stores them in a collection to manipulate like a repository

            var serviceTypes = typeof(IStatefulService)
                                       .GetTypeInfo()
                                       .Assembly.GetTypes()
                                       .Where(a => typeof(IStatefulService).IsAssignableFrom(a) && !typeof(IStatefulService).IsEquivalentTo(a)).ToList(); // any type that can be assignable from IStatefulService but isn't IStatefulService within the assembly

            var statefulServices = serviceTypes.Where(a => a.GetTypeInfo().IsClass && !a.GetTypeInfo().IsAbstract); // class implementations

            Services = serviceTypes
                       .Where(a => a.GetTypeInfo().IsInterface && statefulServices.Any(a.IsAssignableFrom)) // type is Interface, and any of the statefulServices is assignable from the type
                       .Select(a => serviceProvider.GetService(a) as IStatefulService) // get the service from the service provider, and "cast" with as to IStatefulService
                       .Where(a => a != null)
                       .OrderBy(a => a.ServiceName);

            Log.Information("Registered stateful services: {@Services}", Services);
        }

        public async Task StartServicesAsync()
        {
            var serviceStateTasks = Services.Select(a => a.StartAsync(CancellationToken.None));
            await Task.WhenAll(serviceStateTasks);
        }

        public async Task StopServicesAsync()
        {
            var serviceStateTasks = Services.Select(a => a.StopAsync(CancellationToken.None));
            await Task.WhenAll(serviceStateTasks);
        }

        public async Task<IEnumerable<ServiceState>> GetStatesAsync()
        {
            var serviceStateTasks = Services.Select(a => a.GetStateAsync(CancellationToken.None));
            return await Task.WhenAll(serviceStateTasks);
        }

        public async Task<bool> StartAsync(string serviceName)
        {
            var service = Services.FirstOrDefault(a => a.ServiceName == serviceName);
            if (service == null)
            {
                return await Task.FromResult(false);
            }

            await service.StartAsync(CancellationToken.None);

            return await Task.FromResult(true);
        }

        public async Task<bool> StopAsync(string serviceName)
        {
            var service = Services.FirstOrDefault(a => a.ServiceName == serviceName);
            if (service == null)
            {
                return await Task.FromResult(false);
            }

            await service.StopAsync(CancellationToken.None);

            return await Task.FromResult(true);
        }

        public async Task<ServiceState> GetStateAsync(string serviceName)
        {
            var service = Services.FirstOrDefault(a => a.ServiceName == serviceName);
            if (service == null)
            {
                return await Task.FromResult(new ServiceState());
            }

            return await service.GetStateAsync(CancellationToken.None);
        }
    }
}
