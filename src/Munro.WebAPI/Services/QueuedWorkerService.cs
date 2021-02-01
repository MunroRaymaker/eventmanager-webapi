using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace EventManager.WebAPI.Services
{
    /// <summary>
    /// Represents a background worker service that processes a queue.
    /// </summary>
    public class QueuedWorkerService : BackgroundService
    {
        private readonly ILogger<QueuedWorkerService> logger;
        private readonly int waitDelaySeconds;
        public IBackgroundTaskQueue TaskQueue { get; }


        public QueuedWorkerService(ILogger<QueuedWorkerService> logger, IBackgroundTaskQueue taskQueue, IConfiguration config)
        {
            this.logger = logger;
            this.waitDelaySeconds = int.Parse(config.GetSection("WaitDelay").Value);
            TaskQueue = taskQueue;
            logger.LogInformation($"{nameof(QueuedWorkerService)} WaitDelay is {this.waitDelaySeconds} seconds");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait 60 seconds before processing to allow for some jobs to be added
            await Task.Delay(TimeSpan.FromSeconds(this.waitDelaySeconds), stoppingToken);

            this.logger.LogInformation("Queued Worker Service is running at: {time}.", DateTimeOffset.Now);
            await Process(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            this.logger.LogInformation("Queued Worker Service is stopping.");
            await base.StopAsync(stoppingToken);
        }

        private async Task Process(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(ct);

                try
                {
                    await workItem(ct);
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                }

                // Add 60 seconds latency so jobs can be queried
                await Task.Delay(TimeSpan.FromSeconds(this.waitDelaySeconds), ct);
            }
        }
    }
}
