using System;

namespace EventManager.WebAPI.Services
{
    public class LongTasks
    {
        public static int[] Sort(int[] array)
        {
            Array.Sort(array);
            return array;
        }
    }
}