using System;
using System.Threading;

namespace EventManager.WebAPI.Services
{
    public class LongTasks
    {
        public static int[] Sort(int[] array)
        {
            // Simulate long running task
            Thread.Sleep(10000);
            Array.Sort(array);
            return array;
        }
    }
}