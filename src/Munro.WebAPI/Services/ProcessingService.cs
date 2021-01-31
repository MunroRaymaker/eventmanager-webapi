using System;
using EventManager.WebAPI.Model;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventManager.WebAPI.Controllers;
using EventManager.WebAPI.Extensions;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Services
{
    public interface IProcessingService
    {

    }

    public class ProcessingService : IProcessingService, IDisposable
    {
        private readonly ILogger<EventManagerController> logger;

        private readonly CancellationTokenSource cts = new CancellationTokenSource();
        private readonly CancellationToken cancellationToken;

        private int totalProcessed;
        public int TotalProcessed => this.totalProcessed;

        // This must be static to keep state across requests, since webapi is stateless.
        public static ConcurrentQueue<EventJob> Jobs { get; } = new ConcurrentQueue<EventJob>();

        public ProcessingService(ILogger<EventManagerController> logger)
        {
            this.logger = logger;
            this.cancellationToken = this.cts.Token;

            // Add some fake data
            for (int i = 1; i <= 10; i++)
            {
                Jobs.Enqueue(new EventJob
                {
                    Id = i,
                    IsCompleted = false,
                    Status = "Pending",
                    TimeStamp = DateTime.UtcNow,
                    Duration = 0,
                    Data = Enumerable.Range(1, 10).Shuffle().ToArray()
                });
            }
        }

        public void StartProcessing()
        {
            this.logger.LogInformation("Starting Processing service");

            // Start background listener to process jobs
            Task.Run(() => Process());
        }

        private void Process()
        {
            while (!this.cancellationToken.IsCancellationRequested)
            {
                Action action = async () =>
                {
                    // Get a job that has not been processed
                    EventJob workItem = Jobs.FirstOrDefault(x => x.IsCompleted == false);

                    if (workItem == null)
                    {
                        // Nothing to do
                        // Wait 10 seconds before next iteration to save CPU cycles
                        await Task.Delay(10000, this.cancellationToken);
                        return;
                    }

                    this.logger.LogInformation("Background task {Id} started.", workItem.Id);
                    Interlocked.Increment(ref this.totalProcessed);

                    // Do some work
                    workItem.Data = LongTasks.Sort(workItem.Data);
                    workItem.Status = "Processed";
                    workItem.IsCompleted = true;
                    workItem.Duration = (DateTime.Now - workItem.TimeStamp).Ticks;
                };

                action();

                // Spin up three threads to process data in parallel
                //Parallel.Invoke(new ParallelOptions
                //{
                //    CancellationToken = ct,
                //    MaxDegreeOfParallelism = 3
                //}, action, action, action);
            }
        }

        public void Dispose()
        {
            this.cts.Cancel();
        }
    }
}
