using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace OSItemIndex.Data.Database
{
    /// <summary>
    /// Ensures the <see cref="Microsoft.EntityFrameworkCore.DbContext"/> from a <see cref="IDbContextHelper"/> factory helper instance is 'initialized' according to the implementation as a hosted service.
    /// <remarks>Add as a hosted service after a <see cref="IDbContextHelper"/> singleton addition, but before any services injecting the <see cref="IDbContextHelper"/> factory helper.</remarks>
    /// </summary>
    public class DatabaseInitializerService : IDatabaseInitializerService
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseInitializerService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void InitializeDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContextHelper = scope.ServiceProvider.GetRequiredService<IDbContextHelper>();
                using (var factory = dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();
                    dbContext.Database.EnsureCreated(); // TODO ~ Look into migrations
                }
            }
        }

        public async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContextHelper = scope.ServiceProvider.GetRequiredService<IDbContextHelper>();
                using (var factory = dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();
                    await dbContext.Database.EnsureCreatedAsync(cancellationToken); // TODO ~ Look into migrations
                }
            }
        }
    }
}
