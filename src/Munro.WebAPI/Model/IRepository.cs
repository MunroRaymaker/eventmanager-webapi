using System.Collections.Generic;

namespace EventManager.WebAPI.Model
{
    public interface IRepository
    {
        /// <summary>
        /// Inserts or updates a job.
        /// </summary>
        /// <param name="job">The <see cref="EventJob"/> to be saved.</param>
        /// <returns>The id of the job.</returns>
        int Upsert(EventJob job);

        /// <summary>
        /// Gets all jobs.
        /// </summary>
        /// <returns>A list of <see cref="EventJob"/> objects.</returns>
        IEnumerable<EventJob> GetJobs();

        /// <summary>
        /// Gets a job by it's id.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>An <see cref="EventJob"/> object if exists. Returns null if not found.</returns>
        EventJob GetJob(int id);
    }
}