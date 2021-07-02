/***
  * @author     Lampjaw
  * @date       6-9-2019
  * @github     https://github.com/voidwell/Voidwell.DaybreakGames/blob/master/src/Voidwell.DaybreakGames.Data/DesignTimeDbContextFactory.cs
*/

/*** Original author: https://github.com/Lampjaw
 * Modified by https://github.com/Twinki14 for OSItemIndex
 * DesignTimeDbContextFactory: Design-time services such as ef core migrations need to be able to create a dbcontext of our db.
 * DesignTimeDbContextFactory provides an implementation of IDesignTimeDbContextFactory to serve that purpose.
 */

using System;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using OSItemIndex.Data.Extensions;

namespace OSItemIndex.Data.Database
{
    /// <summary>
    ///     IDesignTimeDbContextFactory implementation that's used by design-time services.
    ///     https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.design.idesigntimedbcontextfactory-1?view=efcore-5.0
    /// </summary>
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<OsItemIndexDbContext>
    {
        /// <summary>
        ///     Creates a new instance of a OSItemIndexDbContext.
        /// </summary>
        /// <returns>A new instance of OSItemIndexDbContext.</returns>
        public OsItemIndexDbContext CreateDbContext(string[] args) // https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1#evcp
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var configuration = new ConfigurationBuilder() // TODO Consider consting this somewhere, keep it all in once place, honestly prob not
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appsettings.json", true, true)
                                .AddJsonFile($"appsettings.{environment}.json", true)
                                .AddKeyPerFile("/run/secrets", true) // docker secrets dir
                                .AddEnvironmentVariables()
                                .Build();

            var connStrBuilder = DatabaseExtensions.NpgsqlConnectionStringFromConfig(configuration);

            var builder = new DbContextOptionsBuilder<OsItemIndexDbContext>()
                .UseNpgsql(connStrBuilder.ConnectionString, o => o.CommandTimeout(15));
            return new OsItemIndexDbContext(builder.Options);
        }
    }
}
