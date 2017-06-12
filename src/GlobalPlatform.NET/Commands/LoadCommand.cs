using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public enum Tag : byte
    {
        DapBlock = 0xE2,
        SecurityDomainAID = 0x4F,
        LoadFileDataBlockSignature = 0xC3,
        LoadFileDataBlock = 0xC4
    }

    public interface ILoadCommandBlockSizePicker : IApduBuilder
    {
        IApduBuilder WithBlockSize(byte blockSize);
    }

    public interface ILoadFileStructureBuilder
    {
        ILoadFileStructureBuilder WithDapBlock(byte[] securityDomainAID, byte[] signature);

        ILoadCommandBlockSizePicker WithData(byte[] data);
    }

    public class LoadCommand : CommandBase<LoadCommand, ILoadFileStructureBuilder>,
                ILoadFileStructureBuilder,
        ILoadCommandBlockSizePicker
    {
        private byte blockSize = 247;
        private byte[] data;
        private byte[] securityDomainAID;
        private byte[] signature;

        public override IEnumerable<Apdu> Build()
        {
            if (this.data.Length > this.blockSize)
            {
                this.p1 ^= 0b10000000;
            }

            var data = new List<byte>();

            if (this.securityDomainAID.Any())
            {
                var signatureData = new List<byte>();
                signatureData.AddTag((byte)Tag.SecurityDomainAID, this.securityDomainAID);
                signatureData.AddTag((byte)Tag.LoadFileDataBlockSignature, this.signature);

                data.AddTag((byte)Tag.DapBlock, signatureData.ToArray());
            }

            data.AddTag((byte)Tag.LoadFileDataBlock, this.data);

            return this.data.Split(this.blockSize).Select((block, i) => Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.Load, this.p1, (byte)i, block.ToArray()));
        }

        public IApduBuilder WithBlockSize(byte blockSize)
        {
            this.blockSize = blockSize;

            return this;
        }

        public ILoadFileStructureBuilder WithDapBlock(byte[] securityDomainAID, byte[] signature)
        {
            this.securityDomainAID = securityDomainAID;
            this.signature = signature;

            return this;
        }

        public ILoadCommandBlockSizePicker WithData(byte[] data)
        {
            this.data = data;

            return this;
        }
    }
}
