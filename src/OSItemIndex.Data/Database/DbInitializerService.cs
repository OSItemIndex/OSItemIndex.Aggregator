using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OSItemIndex.Data.Database
{
    /// <summary>
    /// Ensures the <see cref="Microsoft.EntityFrameworkCore.DbContext"/> from a <see cref="IDbContextHelper"/> factory helper instance is 'initialized' according to the implementation as a hosted service.
    /// <remarks>Add as a hosted service after a <see cref="IDbContextHelper"/> singleton addition, but before any services injecting the <see cref="IDbContextHelper"/> factory helper.</remarks>
    /// </summary>
    public class DbInitializerService : IDbInitializerService
    {
        private readonly IServiceProvider _serviceProvider;

        public DbInitializerService(IServiceProvider serviceProvider)
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
                    dbContext.Database.MigrateAsync();
                }
            }
        }

        public async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContextHelper = scope.ServiceProvider.GetRequiredService<IDbContextHelper>();
                using (var factory = dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();
                    await dbContext.Database.MigrateAsync(cancellationToken);
                }
            }
        }
    }
}
