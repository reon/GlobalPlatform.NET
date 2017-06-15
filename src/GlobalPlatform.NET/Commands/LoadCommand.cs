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

        ILoadCommandBlockSizePicker Load(byte[] data);
    }

    public class LoadCommand : CommandBase<LoadCommand, ILoadFileStructureBuilder>,
        ILoadFileStructureBuilder,
        ILoadCommandBlockSizePicker
    {
        private byte blockSize = 247;
        private byte[] data;
        private byte[] securityDomainAID = new byte[0];
        private byte[] signature = new byte[0];

        public override IEnumerable<Apdu> AsApdu()
        {
            var commandData = new List<byte>();

            if (this.securityDomainAID.Any())
            {
                var signatureData = new List<byte>();
                signatureData.AddTag((byte)Tag.SecurityDomainAID, this.securityDomainAID);
                signatureData.AddTag((byte)Tag.LoadFileDataBlockSignature, this.signature);

                commandData.AddTag((byte)Tag.DapBlock, signatureData.ToArray());
            }

            commandData.Add((byte)Tag.LoadFileDataBlock);
            commandData.AddRange(this.data);

            var chunks = commandData.Split(this.blockSize).ToList();

            return chunks.Select((block, index, isLast) => Apdu.Build(
                ApduClass.GlobalPlatform,
                ApduInstruction.Load,
                (byte)(isLast ? 0x80 : 0x00),
                (byte)index,
                block.ToArray()));
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

        public ILoadCommandBlockSizePicker Load(byte[] data)
        {
            this.data = data;

            return this;
        }
    }
}
