using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> enumerable, int blockSize)
        {
            int returned = 0;

            do
            {
                yield return enumerable.Skip(returned).Take(blockSize);

                returned += blockSize;
            }
            while (returned < blockSize);
        }
    }
}
