using System;
using System.Collections.Generic;
using System.Linq;

namespace TeamShrimp.GGJ23
{
    // ReSharper disable once InconsistentNaming
    public static class IEnumerableExt
    {
        public static void IterI<T>(
            this IEnumerable<T> items, Action<int, T> action)
        {
            var arr = items.ToArray();
            for (var i = 0; i < arr.Length; i++) action(i, arr[i]);
        }

        public static T Random<T>(this IEnumerable<T> items)
        {
            var arr = items.ToArray();
            return arr[UnityEngine.Random.Range(0, arr.Length)];
        }

        public static T WeightedRandom<T>(
            this IEnumerable<T> items, Func<T, float> weightSelector)
        {
            var arr = items.ToArray();
            var weights = arr.Select(weightSelector).ToArray();
            var totalWeight = weights.Sum();
            var t = UnityEngine.Random.Range(0, totalWeight);

            for (var i = 0; i < arr.Length; i++)
            {
                t -= weights[i];
                if (t <= 0) return arr[i];
            }

            throw new Exception("No items or something idk");
        }
    }
}