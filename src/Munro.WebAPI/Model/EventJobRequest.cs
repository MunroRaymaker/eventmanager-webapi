namespace EventManager.WebAPI.Model
{
    /// <summary>
    /// Represents a request for a job.
    /// </summary>
    public class EventJobRequest
    {
        /// <summary>
        /// Gets or sets the name of the job.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the user that initiated the job.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the data to be processed.
        /// </summary>
        public int[] Data { get; set; }
    }
}