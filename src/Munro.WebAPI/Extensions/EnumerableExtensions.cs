using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventManager.WebAPI.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<int> Shuffle(this IEnumerable<int> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            
            Random rnd = new Random();

            var array = source as int[] ?? source.ToArray();

            for (int i = 0; i < array.Count(); i++)
            {
                var tmp = array[i];
                var newIndex = rnd.Next(0, array.Count() - 1);
                array[i] = array[newIndex];
                array[newIndex] = tmp;
            }

            return array;
        }
    }
}
