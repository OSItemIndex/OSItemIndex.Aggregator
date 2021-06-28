using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace OSItemIndex.Data.Database
{
    public class DbContextHelper : IDbContextHelper
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DbContextHelper(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        ///     Creates a new instance of DbContextFactory and with it creates a new DbContext instance.
        /// </summary>
        public DbContextFactory GetFactory()
        {
            return new(_scopeFactory);
        }

        public class DbContextFactory : IDisposable, IAsyncDisposable
        {
            private readonly IServiceScope _scope;
            private readonly OsItemIndexDbContext _dbContext;

            public DbContextFactory(IServiceScopeFactory scopeFactory)
            {
                _scope = scopeFactory.CreateScope();
                _dbContext = _scope.ServiceProvider.GetRequiredService<OsItemIndexDbContext>();

                // Disable tracking on our underlying entities in DbContext
                // https://docs.microsoft.com/en-us/ef/core/querying/tracking
                _dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }

            public OsItemIndexDbContext GetDbContext()
            {
                return _dbContext;
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            public async ValueTask DisposeAsync()
            {
                await DisposeAsyncCore();

                Dispose(false);

#pragma warning disable CA1816 // Dispose methods should call SuppressFinalize
                GC.SuppressFinalize(this);
#pragma warning restore CA1816 // Dispose methods should call SuppressFinalize
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                _scope.Dispose();
                _dbContext.Dispose();
            }

            protected virtual async ValueTask DisposeAsyncCore()
            {
                _scope.Dispose();
                await _dbContext.DisposeAsync();
            }
        }
    }
}
