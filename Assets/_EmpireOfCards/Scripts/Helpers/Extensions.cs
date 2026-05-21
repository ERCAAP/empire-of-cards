using System;
using System.Collections.Generic;
using EmpireOfCards.Core;

namespace EmpireOfCards.Helpers
{
    /// <summary>
    /// Collection of utility extension methods used across the project.
    /// </summary>
    public static class Extensions
    {
        private static readonly Random Rng = new Random();

        /// <summary>
        /// Shuffles the list in place using the Fisher-Yates algorithm.
        /// </summary>
        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rng.Next(n + 1);
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }

        /// <summary>
        /// Returns the element at the given index, or null if the index is out of range.
        /// Only works with reference types.
        /// </summary>
        public static T SafeGet<T>(this IList<T> list, int index) where T : class
        {
            if (list == null || index < 0 || index >= list.Count)
                return null;

            return list[index];
        }

        /// <summary>
        /// Checks whether the given CardTag array contains the specified tag.
        /// </summary>
        public static bool HasTag(this CardTag[] tags, CardTag tag)
        {
            if (tags == null)
                return false;

            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == tag)
                    return true;
            }

            return false;
        }
    }
}
