using System;
using System.Threading.Tasks;

namespace EventManager.WebAPI.Services
{
    public class BackendWorker
    {
        public static async Task<int[]> Sort(int[] array)
        {
            // Logic here
            await Task.Delay(1000);
            Array.Sort(array);
            return array;
        }
    }
}