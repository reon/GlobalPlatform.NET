using System;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Commands
{
    public interface IPutKeyVersionPicker
    {
        IPutKeyIdentifierPicker WithKeyVersion(byte version);
    }

    public interface IPutKeyIdentifierPicker
    {
        IPutKeyEncryptionKeyPicker WithKeyIdentifier(byte identifier);
    }

    public interface IPutKeyEncryptionKeyPicker
    {
        IPutKeyFirstKeyPicker UsingKEK(byte[] kek);
    }

    public interface IPutKeyFirstKeyPicker
    {
        IPutKeySecondKeyPicker PutFirstKey(KeyTypeCoding keyType, byte[] key);
    }

    public interface IPutKeySecondKeyPicker : IApduBuilder
    {
        IPutKeyThirdKeyPicker PutSecondKey(KeyTypeCoding keyType, byte[] key);
    }

    public interface IPutKeyThirdKeyPicker : IApduBuilder
    {
        IApduBuilder PutThirdKey(KeyTypeCoding keyType, byte[] key);
    }

    public class PutKeyCommand : CommandBase<PutKeyCommand, IPutKeyVersionPicker>,
        IPutKeyVersionPicker,
        IPutKeyIdentifierPicker,
        IPutKeyEncryptionKeyPicker,
        IPutKeyFirstKeyPicker,
        IPutKeySecondKeyPicker,
        IPutKeyThirdKeyPicker
    {
        private byte keyVersion;
        private byte keyIdentifier;
        private (KeyTypeCoding KeyType, byte[] Key) key1;
        private (KeyTypeCoding KeyType, byte[] Key) key2;
        private (KeyTypeCoding KeyType, byte[] Key) key3;
        private byte[] kek;

        public IPutKeyIdentifierPicker WithKeyVersion(byte keyVersion)
        {
            this.keyVersion = keyVersion;

            return this;
        }

        public IPutKeyEncryptionKeyPicker WithKeyIdentifier(byte keyIdentifier)
        {
            this.keyIdentifier = keyIdentifier;

            return this;
        }

        public IPutKeyFirstKeyPicker UsingKEK(byte[] kek)
        {
            this.kek = kek;

            return this;
        }

        public IPutKeySecondKeyPicker PutFirstKey(KeyTypeCoding keyType, byte[] key)
        {
            this.key1 = (keyType, key);

            return this;
        }

        public IPutKeyThirdKeyPicker PutSecondKey(KeyTypeCoding keyType, byte[] key)
        {
            this.key2 = (keyType, key);

            return this;
        }

        public IApduBuilder PutThirdKey(KeyTypeCoding keyType, byte[] key)
        {
            this.key3 = (keyType, key);

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            var apdu = Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.PutKey, this.keyVersion, this.keyIdentifier, 0x00);

            var data = new List<byte>();

            data.Add(this.keyVersion);

            if (this.key1.Key.Any())
            {
                data.Add((byte)this.key1.KeyType);
                data.AddRangeWithLength(this.key1.Key);
                data.Add(0x00);
            }

            if (this.key2.Key.Any())
            {
                data.Add((byte)this.key2.KeyType);
                data.AddRangeWithLength(this.key2.Key);
                data.Add(0x00);
            }

            if (this.key3.Key.Any())
            {
                data.Add((byte)this.key3.KeyType);
                data.AddRangeWithLength(this.key3.Key);
                data.Add(0x00);
            }

            apdu.CommandData = data.ToArray();

            yield return apdu;
        }
    }
}