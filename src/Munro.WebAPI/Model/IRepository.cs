using System.Collections.Generic;

namespace EventManager.WebAPI.Model
{
    public interface IRepository
    {
        /// <summary>
        /// Inserts or updates a job.
        /// </summary>
        /// <param name="job">The <see cref="Job"/> to be saved.</param>
        /// <returns>The id of the job.</returns>
        int Upsert(Job job);

        /// <summary>
        /// Gets all jobs.
        /// </summary>
        /// <returns>A list of <see cref="Job"/> objects.</returns>
        IEnumerable<Job> GetJobs();

        /// <summary>
        /// Gets a job by it's id.
        /// </summary>
        /// <param name="id">The id to search for.</param>
        /// <returns>An <see cref="Job"/> object if exists. Returns null if not found.</returns>
        Job GetJob(int id);

        /// <summary>
        /// Deletes a job by it's id.
        /// </summary>
        /// <param name="id">The id to delete.</param>
        void DeleteJob(int id);
    }
}