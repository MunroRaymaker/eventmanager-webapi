using EventManager.WebAPI.Model;

namespace EventManager.WebAPI.Mapping
{
    /// <summary>
    /// Represents a simple mapper.
    /// </summary>
    public static class Mapper
    {
        public static EventJob Map(EventJobRequest source)
        {
            return new EventJob
            {
                Data = source.Data,
                Name = source.Name,
                UserName = source.UserName
            };
        }
    }
}
