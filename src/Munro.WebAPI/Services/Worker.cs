using System;
using System.Collections.Generic;
using System.Linq;

namespace EventManager.WebAPI.Services
{
    /// <summary>
    /// Represents a class that holds business logic.
    /// </summary>
    public interface IWorker
    {
        /// <summary>
        /// Sorts an array. Default is ascending order.
        /// </summary>
        /// <param name="array">The array of integers to be sorted</param>
        /// <param name="sortOrder">The sorting order.</param>
        /// <returns>A sorted array of integers.</returns>
        int[] DoWork(IEnumerable<int> array, SortOrder sortOrder = SortOrder.Ascending);
    }

    public class Worker : IWorker
    {
        public int[] DoWork(IEnumerable<int> data, SortOrder sortOrder = SortOrder.Ascending)
        {
            var array = data as int[] ?? Array.Empty<int>();

            // For debugging
            if(array.Contains(99))
            {
                throw new ArgumentException("Value must not be 99 for some strange reason");
            }

            Array.Sort(array);

            if (sortOrder == SortOrder.Descending)
            {
                Array.Reverse(array);
            }

            return array;
        }
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }
}