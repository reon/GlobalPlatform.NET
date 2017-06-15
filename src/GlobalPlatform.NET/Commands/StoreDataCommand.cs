using System;
using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public interface IStoreDataP1Picker
    {
        IStoreDataPicker WithP1(byte p1);
    }

    public interface IStoreDataPicker
    {
        IStoreDataBlockSizePicker StoreData(byte[] data);
    }

    public interface IStoreDataBlockSizePicker : IApduBuilder
    {
        IApduBuilder WithBlockSize(byte blockSize);
    }

    public class StoreDataCommand : CommandBase<StoreDataCommand, IStoreDataP1Picker>,
        IStoreDataP1Picker,
        IStoreDataPicker,
        IStoreDataBlockSizePicker
    {
        private byte[] data;
        private byte blockSize = 247;

        public IStoreDataPicker WithP1(byte p1)
        {
            this.P1 = p1;

            return this;
        }

        public IStoreDataBlockSizePicker StoreData(byte[] data)
        {
            this.data = data;

            return this;
        }

        public IApduBuilder WithBlockSize(byte blockSize)
        {
            this.blockSize = blockSize;

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            var chunks = this.data.Split(this.blockSize).ToList();

            return chunks.Select((block, index, isLast) =>
            {
                if (isLast)
                {
                    this.P1 ^= 0x80;
                }

                return Apdu.Build(
                    ApduClass.GlobalPlatform,
                    ApduInstruction.StoreData,
                    this.P1,
                    (byte)index,
                    block.ToArray());
            });
        }
    }
}
