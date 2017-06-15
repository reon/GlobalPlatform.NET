using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Extensions
{
    public static class EnumerableExtensions
    {
        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static void ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            int index = 0;

            foreach (var item in source)
            {
                action(item, index);

                index++;
            }
        }

        public static void ForEach<TSource>(this ICollection<TSource> collection, Action<TSource, int, bool> action)
        {
            int index = 0;

            foreach (var item in collection)
            {
                action(item, index, collection.Count == index + 1);

                index++;
            }
        }

        public static IEnumerable<TResult> Select<TSource, TResult>(this ICollection<TSource> collection,
            Func<TSource, int, bool, TResult> selector)
        {
            int index = 0;

            foreach (var item in collection)
            {
                yield return selector(item, index, collection.Count == index + 1);

                index++;
            }
        }

        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int blockSize)
        {
            int returned = 0;

            do
            {
                yield return enumerable.Skip(returned).Take(blockSize);

                returned += blockSize;
            }
            while (returned < enumerable.Count());
        }
    }
}
