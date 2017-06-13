using System.Collections.Generic;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public interface IChannelOperationPicker
    {
        IApduBuilder OpenChannel();

        IChannelPicker CloseChannel();
    }

    public interface IChannelPicker
    {
        IApduBuilder WithIdentifier(byte identifier);
    }

    public class ManageChannelCommand : CommandBase<ManageChannelCommand, IChannelOperationPicker>,
        IChannelOperationPicker,
        IChannelPicker
    {
        public IApduBuilder OpenChannel()
        {
            this.operation = Operation.Open;

            return this;
        }

        public IChannelPicker CloseChannel()
        {
            this.operation = Operation.Close;

            return this;
        }

        public IApduBuilder WithIdentifier(byte identifier)
        {
            this.identifier = identifier;

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            this.P1 = (byte)this.operation;
            this.P2 = this.operation == Operation.Close ? this.identifier : (byte)0x00;

            yield return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.ManageChannel, this.P1, this.P2);
        }

        private enum Operation : byte
        {
            Open = 0x00,
            Close = 0x80
        }

        private Operation operation;
        private byte identifier;
    }
}
