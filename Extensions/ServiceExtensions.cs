using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using OSItemIndex.Data.Database;

namespace OSItemIndex.Data.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddEntityFrameworkContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<DbOptions>(configuration.GetSection(DbOptions.Section));
            services.AddDbContextPool<OsItemIndexDbContext>(builder => builder.UseNpgsql(NpgsqlConnectionStringFromConfig(configuration).ConnectionString));
            services.AddSingleton<IDbContextHelper, DbContextHelper>();
            return services;
        }

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
