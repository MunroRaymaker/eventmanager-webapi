using System;
using System.Threading;
using System.Threading.Tasks;

namespace EventManager.WebAPI.Services
{
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Queues a work item in background thread to be processed by a worker.
        /// </summary>
        /// <param name="workItem">The work item.</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// Dequeue a work item from a background queue.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Func with the task to be processed.</returns>
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}