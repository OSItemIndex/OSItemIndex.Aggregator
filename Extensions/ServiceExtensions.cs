using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OSItemIndex.Data.Database;

/***
 * Design: Want this to work when running locally OR when being used in docker, with no manual changes - it needs to connect to the postgres docker image if it's being ran in docker, otherwise try to connect to a local dev postgres instance
 *      Attempt 1: utilize a appsettings.json to override the use of a pgpass file, a pgpass file will only be used if the password in dboptions in null, empty, or plain whitespace, and it'll only be used for the password
 *          But, if we do this, the appsettings.json will be passed to the docker image, because we don't ignore it, and we shouldn't ignore it, so it's easy to configure.
 *          We could check the pgpass environment var, and if it exists, we set the password to null, but this needs to be made known
 *          We could also just use environment variables instead for everything, as those would override anything in appsettings.json - but that comes with the security flaw of exposing env vars to the machines
 *          Could check if the secrets file exists instead of the env var for the pgpass, if it does then we just set the password to null - set a custom env var like SECRET_PGPASS as the path to the secret,
 *              and just check if it's null - if it isn't, it's running in a docker image that's meant to use the pgpass file
 *
 *      I think the best thing to do would be to utilize ASP.NETs configuration priorities to automatically determine what's used, without needing a bunch of custom code like we'd need to
 *          but this comes with the security flaw of using plain text env vars for the connection str
 *
 */


namespace OSItemIndex.Data.Extensions
{
    public static class DatabaseExtensions
    {
        /// <summary>
        ///     TODO
        /// </summary>
        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<DbOptions>(configuration.GetSection(DbOptions.Section));
            services.AddDbContextPool<OsItemIndexDbContext>(builder => builder.UseNpgsql(NpgsqlConnectionStringFromConfig(configuration).ConnectionString));
            services.AddSingleton<IDbContextHelper, DbContextHelper>();
            return services;
        }

        /// <summary>
        ///    TODO
        /// </summary>
        /// <returns></returns>
        public static NpgsqlConnectionStringBuilder NpgsqlConnectionStringFromConfig(IConfiguration configuration)
        {
            var config = configuration.GetSection(DbOptions.Section).Get<DbOptions>() ?? new DbOptions();
            var builder = new NpgsqlConnectionStringBuilder
            {
                Username = config.DbUser,
                Password = config.DbPass,
                Host = config.DbHost,
                Port = config.DbPort,
                Database = config.DbDatabase,
                Pooling = true,
                MinPoolSize = 0,
                MaxPoolSize = config.PgPoolSize
            };
            return builder;
        }
    }
}
