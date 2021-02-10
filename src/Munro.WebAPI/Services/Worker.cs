using System;
using System.Collections.Generic;

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