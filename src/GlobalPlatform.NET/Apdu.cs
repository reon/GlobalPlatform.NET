using System;
using System.Collections.Generic;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

using System;

namespace GlobalPlatform.NET
{
    public class Apdu
    {
        public ApduClass CLA { get; set; }

        public ApduInstruction INS { get; set; }

        public byte P1 { get; set; }

        public byte P2 { get; set; }

        public byte Lc => this.CommandData.LengthChecked();

        public byte[] CommandData { get; set; }

        public byte Le { get; set; }

        public byte[] Buffer
        {
            get
            {
                var buffer = new List<byte>()
                {
                    (byte)this.CLA,
                    (byte)this.INS,
                    this.P1,
                    this.P2,
                    this.Lc
                };

                buffer.AddRange(this.CommandData);
                buffer.Add(this.Le);

                return buffer.ToArray();
            }
        }

        public static Apdu Build(ApduClass cla, ApduInstruction ins, byte p1, byte p2, params byte[] data)
        {
            if (data.Length > 255)
            {
                throw new ArgumentException("Data exceeds 255 bytes.", nameof(data));
            }

            return new Apdu()
            {
                CLA = cla,
                INS = ins,
                P1 = p1,
                P2 = p2,
                CommandData = data,
                Le = 0x00
            };
        }
    }
}
