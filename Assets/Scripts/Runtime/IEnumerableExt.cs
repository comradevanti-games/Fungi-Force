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
    }
}