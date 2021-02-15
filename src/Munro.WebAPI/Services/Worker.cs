using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventManager.WebAPI.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace EventManager.WebAPI.Services
{
    /// <summary>
    /// Represents a class that holds business logic.
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Performs the actual unit of work
        /// </summary>
        /// <param name="id">The id of the job to work on.</param>
        Task DoWork(long id);
    }

    public class Worker : IWorker
    {
        private readonly ILogger<Worker> logger;

        public Worker(ILogger<Worker> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task DoWork(long id)
        {
            this.logger.LogInformation("Queued background task with id {Id} started ", job.Id);

            using (var ctx = new JobContext())
            {
                // get work item from storage
                var jobItem = await ctx.JobItems.FindAsync(id);
                if (jobItem == null) throw new ArgumentException($"Item not found with id '{job.Id}'");

                try
                {
                    // sort data
                    jobItem.Data = DoWork(jobItem.Data);
                    jobItem.Complete();
                }
                catch (Exception ex)
                {
                    // What if DoWork fails? Mark as failed and refer to manual processing.
                    this.logger.LogError("Queued background task with id {Id} failed with message {Message}", jobItem.Id,
                        ex.Message);

                    jobItem.Failed();
                }

                this.logger.LogInformation("Queued background task with id {Id} completed in {Duration} ticks", jobItem.Id,
                    jobItem.Duration);

                //var state = ctx.Entry(jobItem).State;

                // save data
                try
                {
                    await ctx.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException) when (!ctx.JobItems.Any(e => e.Id == jobItem.Id))
                {
                    throw new ArgumentException("Not found");
                }
                catch (Exception ex)
                {
                    this.logger.LogError(ex, "Could not save. " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Sorts an array. Default is ascending order.
        /// </summary>
        /// <param name="data">The array of integers to be sorted</param>
        /// <param name="sortOrder">The sorting order.</param>
        /// <returns>A sorted array of integers.</returns>
        private int[] DoWork(IEnumerable<int> data, SortOrder sortOrder = SortOrder.Ascending)
        {
            var array = data as int[] ?? Array.Empty<int>();

            // For debugging
            if(array.Contains(99))
            {
                throw new ArgumentException("Value must not be 99 for some strange reason");
            }

            Array.Sort(array);

            if (sortOrder == SortOrder.Descending)
            {
                Array.Reverse(array);
            }

            return array;
        }
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }
}