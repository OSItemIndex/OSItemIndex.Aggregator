using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OSItemIndex.Aggregator.Services;
using OSItemIndex.Data.Database;
using OSItemIndex.Data.Extensions;
using OSItemIndex.Data.Repositories;

namespace OSItemIndex.Aggregator
{
    public class Startup
    {
        private const string CorsPolicy = "_corsPolicy";
        private readonly IConfiguration _configuration;

        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder();

            builder.Sources.Clear();
            builder.SetBasePath(env.ContentRootPath);
            builder.AddJsonFile("appsettings.json", true, true);
            builder.AddJsonFile($"appsettings.{env.EnvironmentName}.json", true);
            builder.AddKeyPerFile("/run/secrets", true); // docker secrets - https://docs.microsoft.com/en-us/dotnet/core/extensions/configuration-providers#key-per-file-configuration-provider
            builder.AddEnvironmentVariables();

            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(CorsPolicy,
                                  builder =>
                                  {
                                      builder.WithOrigins("https://ositemindex.com", "http://localhost:8080")
                                             .AllowAnyHeader();
                                  });
            });

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<BrotliCompressionProvider>();
                options.Providers.Add<GzipCompressionProvider>();
            });

            services.AddControllers();
            services.AddEntityFrameworkContext(_configuration);
            services.AddSingleton<IDbInitializerService, DbInitializerService>();

            services.AddHttpClient<OsrsBoxClient>();
            services.AddHttpClient<RealtimePriceClient>();

            services.AddSingleton<IOsrsBoxService, OsrsBoxService>();
            services.AddSingleton<IRealtimePriceService, RealtimePricesService>();

            services.AddSingleton<IEventRepository, EventRepository>();
            services.AddSingleton<IStatefulServiceRepository, StatefulServiceRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(CorsPolicy);
            app.UsePathBase("/api");
            app.UseRouting();
            app.UseResponseCompression();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
