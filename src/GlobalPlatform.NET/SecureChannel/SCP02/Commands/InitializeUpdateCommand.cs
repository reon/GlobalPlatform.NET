using System;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Cryptography;
using GlobalPlatform.NET.Reference;
using System.Collections.Generic;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Commands
{
    public interface IInitializeUpdateKeyVersionPicker
    {
        IInitializeUpdateHostChallengePicker WithKeyVersion(byte version);
    }

    public interface IInitializeUpdateHostChallengePicker
    {
        IApduBuilder WithHostChallenge(byte[] hostChallenge);

        IApduBuilder WithHostChallenge(out byte[] hostChallenge);
    }

    public class InitializeUpdateCommand : CommandBase<InitializeUpdateCommand, IInitializeUpdateKeyVersionPicker>,
        IInitializeUpdateKeyVersionPicker,
        IInitializeUpdateHostChallengePicker
    {
        private byte[] hostChallenge;

        public IInitializeUpdateHostChallengePicker WithKeyVersion(byte version)
        {
            this.P1 = version;

            return this;
        }

        public IApduBuilder WithHostChallenge(byte[] hostChallenge)
        {
            this.hostChallenge = hostChallenge;

            return this;
        }

        public IApduBuilder WithHostChallenge(out byte[] hostChallenge)
        {
            this.hostChallenge = hostChallenge = SecureRandom.GetBytes(8);

            return this;
        }

        public override Apdu AsApdu() => Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.InitializeUpdate, this.P1, this.P2, this.hostChallenge);
    }
}
