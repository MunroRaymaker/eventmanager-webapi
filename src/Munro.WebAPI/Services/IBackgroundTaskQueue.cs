using System;
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
}