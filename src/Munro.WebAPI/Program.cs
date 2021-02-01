using EventManager.WebAPI.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace EventManager.WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup NLog logging first to catch all errors
            var logger = NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

            try
            {
                logger.Debug("Initialized Main");
                var host = CreateHostBuilder(args).Build();

                // Start the background process to listen for queued work items
                //var backgroundProcessingServiceService = host.Services.GetRequiredService<ProcessingService>();
                //backgroundProcessingServiceService.StartProcessing();

                host.Run();
            }
            catch (System.Exception ex)
            {
                // Catch setup errors
                logger.Error(ex, "Stopped program because an exception occured");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application exit (avoid segmentation faults).
                NLog.LogManager.Shutdown();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                        .ConfigureLogging((hostingContext, logging) =>
                        {
                            logging.ClearProviders();
                            logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                            logging.AddConsole();
                            logging.SetMinimumLevel(LogLevel.Trace);
                        })
                        .UseNLog();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    // Need to register the service early in the pipeline to allow for service startup
                    //services.AddSingleton<ProcessingService>();
                });
    }
}
