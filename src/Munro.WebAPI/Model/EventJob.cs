using System;

namespace EventManager.WebAPI.Model
{
    public class EventJob
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string UserName { get; set; }

        public int[] Data { get; set; }

        public DateTime TimeStamp { get; set; }

        public TimeSpan Duration { get; set; }
        
        public bool IsCompleted { get; set; }

        public string Status { get; set; } // pending or completed
    }
}
