using System;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System.Collections.Generic;
using System.Linq;

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

    /// <summary>
    /// The STORE DATA command is used to transfer data to an Application or the Security Domain
    /// processing the command.
    /// <para>
    /// The Security Domain determines if the command is intended for itself or an Application
    /// depending on a previously received command. If a preceding command was an INSTALL [for
    /// personalization] command, the STORE DATA command is destined for an Application.
    /// </para>
    /// <para>
    /// Multiple STORE DATA commands are used to send data to the Application or Security Domain by
    /// breaking the data into smaller components for transmission. The Security Domain shall be
    /// informed of the last block.
    /// </para>
    /// <para>
    /// A personalization session starts when a Security Domain receives a valid INSTALL [for
    /// personalization] command designating an Application (implementing either the Application or
    /// the Personalization interface) to which the Security Domain shall forward subsequently
    /// received STORE DATA commands.
    /// </para>
    /// <para>Based on section 11.11 of the v2.3 GlobalPlatform Card Specification.</para>
    /// </summary>
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