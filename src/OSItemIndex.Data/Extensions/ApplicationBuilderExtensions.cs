using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OSItemIndex.Data.Database;

namespace OSItemIndex.Data.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder MigrateDatabases(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var dbContextHelper = scope.ServiceProvider.GetRequiredService<IDbContextHelper>();
                using (var factory = dbContextHelper.GetFactory())
                {
                    var dbContext = factory.GetDbContext();
                    dbContext?.Database.EnsureCreated(); // TODO ~ Look into migrations
                }
            }

            return app;
        }
    }
}
