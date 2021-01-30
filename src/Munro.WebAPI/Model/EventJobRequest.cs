namespace EventManager.WebAPI.Model
{
    public class EventJobRequest
    {
        public string Name { get; set; }

        public string UserName { get; set; }

        public int[] Data { get; set; }
    }
}