using System.Collections.Generic;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public enum GetStatusScope : byte
    {
        IssuerSecurityDomain = 0b10000000,
        Applications = 0b01000000,
        ExecutableLoadFiles = 0b00100000,
        ExecutableLoadFilesAndModules = 0b00010000
    }

    public interface IGetStatusScopePicker
    {
        IGetStatusApplicationFilter GetStatusOf(GetStatusScope scope);
    }

    public interface IGetStatusApplicationFilter : IApduBuilder
    {
        IApduBuilder WithFilter(byte[] applicationFilter);
    }

    public class GetStatusCommand : CommandBase<GetStatusCommand, IGetStatusScopePicker>,
        IGetStatusScopePicker,
        IGetStatusApplicationFilter
    {
        private byte[] applicationFilter = new byte[0];

        public enum Tag : byte
        {
            ApplicationAID = 0x4F,
        }

        public IGetStatusApplicationFilter GetStatusOf(GetStatusScope scope)
        {
            this.P1 = (byte)scope;

            return this;
        }

        public IApduBuilder WithFilter(byte[] applicationFilter)
        {
            this.applicationFilter = applicationFilter;

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            var apdu = Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.GetStatus, this.P1, this.P2);

            var data = new List<byte>();

            data.AddTag((byte)Tag.ApplicationAID, this.applicationFilter);

            apdu.CommandData = data.ToArray();

            yield return apdu;
        }
    }
}
