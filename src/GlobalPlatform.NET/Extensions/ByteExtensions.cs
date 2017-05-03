using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPlatform.NET.Extensions
{
    internal static class ByteExtensions
    {
        public static byte LengthChecked(this byte[] array) => checked((byte)array.Length);

        public static byte AddRangeWithLength(this ICollection<byte> bytes, byte[] range)
        {
            byte length = range.LengthChecked();

            bytes.Add(length);
            bytes.AddRange(range);

            return length;
        }
    }
}
