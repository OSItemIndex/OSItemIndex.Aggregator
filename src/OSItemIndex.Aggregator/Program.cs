using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OSItemIndex.Aggregator.Services;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;

namespace OSItemIndex.Aggregator
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                         .Enrich.FromLogContext()
                         .Enrich.WithExceptionDetails()
                         .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                         .CreateBootstrapLogger();

            var webHost = CreateWebHost(args).Build();
            using (var scope = webHost.Services.CreateScope()) // Start all IStatefulServices
            {
                var servicesController = scope.ServiceProvider.GetRequiredService<IStatefulServiceRepository>();
                await servicesController.StartServicesAsync();
            }
            await webHost.RunAsync();

            Log.CloseAndFlush();
        }

        public static IHostBuilder CreateWebHost(string[] args)
        {
            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(builder =>
            {
                builder.UseStartup<Startup>()
                       .UseSerilog((context, configuration) =>
                       {
                           configuration.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                                        .Enrich.FromLogContext()
                                        .Enrich.WithExceptionDetails()
                                        .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"); // {Properties:j}
                       });
            });
        }
    }
}
