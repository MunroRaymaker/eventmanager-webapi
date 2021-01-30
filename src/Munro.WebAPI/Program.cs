using Microsoft.AspNetCore.Hosting;
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
                CreateHostBuilder(args).Build().Run();
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
                });
    }
}
