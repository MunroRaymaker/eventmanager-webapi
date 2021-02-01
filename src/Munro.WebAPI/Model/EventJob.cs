using System;

namespace EventManager.WebAPI.Model
{
    /// <summary>
    /// Represents a job which holds data to be processed.
    /// </summary>
    public class EventJob
    {
        /// <summary>
        /// Gets or sets the unique id. Uses integers for easy reference.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a name for the job.
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

        /// <summary>
        /// Gets or sets the timestamp when the job was created.
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the duration of the processing.
        /// </summary>
        public long Duration { get; private set; }
        
        /// <summary>
        /// Gets or sets if the job is completed.
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// Gets the status of the job.
        /// </summary>
        public string Status => IsCompleted ? "Completed" : "Pending";

        public void Complete()
        {
            Duration = (DateTime.UtcNow - TimeStamp).Ticks;
            IsCompleted = true;
        }

        public void ReProcess()
        {
            IsCompleted = false;
        }
    }
}
