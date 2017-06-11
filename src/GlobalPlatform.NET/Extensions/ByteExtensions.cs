using System.Collections.Generic;

namespace GlobalPlatform.NET.Extensions
{
    internal static class ByteExtensions
    {
        /// <summary>
        /// Adds a range of bytes to the collection, prefixed by a single byte denoting the range's length.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static byte AddRangeWithLength(this ICollection<byte> bytes, byte[] range)
        {
            byte length = range.LengthChecked();

            bytes.Add(length);
            bytes.AddRange(range);

            return length;
        }

        /// <summary>
        /// Adds a tag to the collection, followed by the length of the data, followed by the data itself.
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static byte AddTag(this ICollection<byte> bytes, byte tag, params byte[] data)
        {
            bytes.Add((byte)tag);

            byte length = data.LengthChecked();

            bytes.Add(length);
            bytes.AddRange(data);

            return length;
        }

        /// <summary>
        /// Returns the length of the array, as a checked byte.
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static byte LengthChecked(this byte[] array) => checked((byte)array.Length);
    }
}
