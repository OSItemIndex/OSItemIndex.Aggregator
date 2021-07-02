using System;
using System.Threading;
using System.Threading.Tasks;

namespace OSItemIndex.Data.Database
{
    public interface IDbInitializerService
    {
        public void InitializeDatabase(IServiceProvider serviceProvider);
        public Task InitializeDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default);
    }
}
