using System.Collections.Generic;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public interface IApplicationFilter
    {
        IApduBuilder WithNoFilter();

        IApduBuilder WithFilter(byte[] applicationFilter);
    }

    public class GetStatusCommand : CommandP1P2Base<GetStatusCommand, GetStatusCommand.P1, GetStatusCommand.P2, IApplicationFilter>,
            IApplicationFilter
    {
        private byte[] applicationFilter;

        public enum P1 : byte
        {
            IssuerSecurityDomain = 0b10000000,
            Applications = 0b01000000,
            ExecutableLoadFiles = 0b00100000,
            ExecutableLoadFilesAndModules = 0b00010000
        }

        public enum P2 : byte
        {
            GetFirstOrAllOccurrences = 0b00000000,
            GetNextOccurrence = 0b00000001
        }

        public enum Tag : byte
        {
            ApplicationAID = 0x4F,
        }

        public override IEnumerable<Apdu> Build()
        {
            var apdu = Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.GetData, p1, p2);

            var data = new List<byte>();

            data.AddTag((byte)Tag.ApplicationAID, this.applicationFilter);

            apdu.CommandData = data.ToArray();

            yield return apdu;
        }

        public IApduBuilder WithNoFilter()
        {
            this.applicationFilter = new byte[0];

            return this;
        }

        public IApduBuilder WithFilter(byte[] applicationFilter)
        {
            this.applicationFilter = applicationFilter;

            return this;
        }

        public override IP2Picker<P2, IApplicationFilter> WithP1(byte p1)
        {
            this.p1 = p1;

            return this;
        }

        public override IP2Picker<P2, IApplicationFilter> WithP1(P1 p1) => this.WithP1((byte)p1);

        public override IApplicationFilter WithP2(byte p2)
        {
            this.p2 = p2;

            return this;
        }

        public override IApplicationFilter WithP2(P2 p2) => this.WithP2((byte)p2);
    }
}
