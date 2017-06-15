using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET
{
    public class Apdu
    {
        public byte[] Buffer
        {
            get
            {
                var buffer = new List<byte>
                {
                    (byte)this.CLA,
                    (byte)this.INS,
                    this.P1,
                    this.P2,
                };

                if (this.CommandData.Any())
                {
                    buffer.Add(this.Lc);
                    buffer.AddRange(this.CommandData);
                }

                buffer.AddRange(this.Le);

                return buffer.ToArray();
            }
        }

        public ApduClass CLA { get; set; }

        public ApduInstruction INS { get; set; }

        public byte P1 { get; set; }

        public byte P2 { get; set; }

        public byte Lc => this.CommandData.LengthChecked();

        public byte[] CommandData { get; set; } = new byte[0];

        public byte[] Le { get; set; } = new byte[0];

        public static Apdu Build(ApduClass cla, ApduInstruction ins, byte p1, byte p2)
            => new Apdu
            {
                CLA = cla,
                INS = ins,
                P1 = p1,
                P2 = p2
            };

        public static Apdu Build(ApduClass cla, ApduInstruction ins, byte p1, byte p2, byte le)
        {
            var apdu = Build(cla, ins, p1, p2);

            apdu.Le = new[] { le };

            return apdu;
        }

        public static Apdu Build(ApduClass cla, ApduInstruction ins, byte p1, byte p2, byte[] data)
        {
            if (data.Length > 255)
            {
                throw new ArgumentException("Data exceeds 255 bytes.", nameof(data));
            }

            var apdu = Build(cla, ins, p1, p2);

            apdu.CommandData = data;
            apdu.Le = new byte[] { 0x00 };

            return apdu;
        }

        public static Apdu Build(ApduClass cla, ApduInstruction ins, byte p1, byte p2, byte[] data, byte le)
        {
            var apdu = Build(cla, ins, p1, p2, data);

            apdu.Le = new[] { le };

            return apdu;
        }

        public override string ToString() => BitConverter.ToString(this.Buffer);
    }
}
