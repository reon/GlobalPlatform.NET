using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System;
using System.Collections.Generic;

namespace GlobalPlatform.NET
{
    public class Apdu
    {
        public static byte[] Build(ApduClass cla, ApduInstruction ins, byte p1, byte p2, params byte[] data)
        {
            if (data.Length > 255)
            {
                throw new ArgumentException("Data exceeds 255 bytes.", nameof(data));
            }

            var buffer = new List<byte>
            {
                (byte)cla,
                (byte)ins,
                p1,
                p2,
                data.LengthChecked()
            };
            buffer.AddRange(data);
            buffer.Add(0x00);

            return buffer.ToArray();
        }
    }
}