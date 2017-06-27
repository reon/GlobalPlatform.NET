using System.Collections.Generic;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;
using GlobalPlatform.NET.SecureChannel.SCP02.Reference;

namespace GlobalPlatform.NET.SecureChannel.SCP02.Commands
{
    public interface ISecurityLevelPicker
    {
        IHostChallengePicker WithSecurityLevel(SecurityLevel securityLevel);
    }

    public interface IHostChallengePicker
    {
        ISecureChannelKeyPicker UsingHostChallenge(byte[] hostChallenge);
    }

    public interface ISecureChannelKeyPicker
    {
        IApduBuilder AndKey(byte[] key);
    }

    public class ExternalAuthenticateCommand : CommandBase<ExternalAuthenticateCommand, ISecurityLevelPicker>,
        ISecurityLevelPicker,
        IHostChallengePicker,
        ISecureChannelKeyPicker
    {
        private byte[] hostChallenge;
        private byte[] key;
        private byte[] hostCryptogram;

        public IHostChallengePicker WithSecurityLevel(SecurityLevel securityLevel)
        {
            this.P1 = (byte)securityLevel;

            return this;
        }

        public ISecureChannelKeyPicker UsingHostChallenge(byte[] hostChallenge)
        {
            this.hostChallenge = hostChallenge;

            return this;
        }

        public IApduBuilder AndKey(byte[] key)
        {
            this.key = key;

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            yield return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.ExternalAuthenticate, this.P1, this.P2, this.hostCryptogram);
        }
    }
}
