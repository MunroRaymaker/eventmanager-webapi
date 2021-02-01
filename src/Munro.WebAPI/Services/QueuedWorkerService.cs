using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventManager.WebAPI.Services
{
    /// <summary>
    /// Represents a background worker service that processes a queue.
    /// </summary>
    public class QueuedWorkerService : BackgroundService
    {
        private readonly ILogger<QueuedWorkerService> logger;
        public IBackgroundTaskQueue TaskQueue { get; }

        public QueuedWorkerService(ILogger<QueuedWorkerService> logger, IBackgroundTaskQueue taskQueue)
        {
            this.logger = logger;
            TaskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Wait 60 seconds before processing to allow for some jobs to be added
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);

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
                await Task.Delay(TimeSpan.FromSeconds(60), ct);
            }
        }
    }
}
