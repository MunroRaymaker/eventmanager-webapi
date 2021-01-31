using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace EventManager.WebAPI.Services
{
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Queues a workitem in background thread to be processed by a worker.
        /// </summary>
        /// <param name="workItem">The workitem.</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// Dequeues a workitem from a background queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Func with the task to be processed.</returns>
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> workItems = new ConcurrentQueue<Func<CancellationToken, Task>>();
        private readonly SemaphoreSlim signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem is null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            this.workItems.Enqueue(workItem);
            this.signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await this.signal.WaitAsync(cancellationToken);
            this.workItems.TryDequeue(out var workItem);
            return workItem;
        }
    }
}