using System;
using System.Threading.Tasks;

namespace EventManager.WebAPI.Services
{
    /// <summary>
    ///     Represents a class that holds business logic.
    /// </summary>
    public class Worker
    {
        /// <summary>
        ///     Sorts an array. Default is ascending order.
        /// </summary>
        /// <param name="array">The array of integers to be sorted</param>
        /// <param name="sortOrder">The sorting order.</param>
        /// <returns>A sorted array of integers.</returns>
        public static int[] Sort(int[] array, SortOrder sortOrder = SortOrder.Ascending)
        {
            Array.Sort(array);

            if (sortOrder == SortOrder.Descending) Array.Reverse(array);

            return array;
        }

        /// <summary>
        ///     Sorts an array. Default is ascending order.
        /// </summary>
        /// <param name="array">The array of integers to be sorted</param>
        /// <param name="sortOrder">The sorting order.</param>
        /// <returns>A sorted array of integers.</returns>
        public static async Task<int[]> SortAsync(int[] array, SortOrder sortOrder = SortOrder.Ascending)
        {
            return await Task.Run(() =>
            {
                Array.Sort(array);

                if (sortOrder == SortOrder.Descending) Array.Reverse(array);

                return array;
            });
        }
    }

    public enum SortOrder
    {
        Ascending,
        Descending
    }
}