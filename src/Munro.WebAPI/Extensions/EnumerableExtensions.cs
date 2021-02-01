using System;
using System.Collections.Generic;
using System.Linq;

namespace EventManager.WebAPI.Extensions
{
    public static class EnumerableExtensions
    {
        /// <summary>
        ///     Shuffles an array of integers in random order.
        /// </summary>
        /// <param name="source">The input array.</param>
        /// <returns>A shuffled array.</returns>
        public static IEnumerable<int> Shuffle(this IEnumerable<int> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var rnd = new Random();

            var array = source as int[] ?? source.ToArray();

            for (var i = 0; i < array.Length; i++)
            {
                var tmp = array[i];
                var newIndex = rnd.Next(0, array.Length - 1);
                array[i] = array[newIndex];
                array[newIndex] = tmp;
            }

            return array.ToArray();
        }
    }
}