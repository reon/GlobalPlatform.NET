using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Commands
{
    public interface IDeleteCommandApplicationPicker
    {
        IDeleteCommandTokenPicker WithAID(byte[] aid);
    }

    public interface IDeleteCommandKeyPicker : IApduBuilder
    {
        IDeleteCommandKeyPicker WithIdentifier(byte keyIdentifier);

        IDeleteCommandKeyPicker WithVersionNumber(byte keyVersionNumber);
    }

    public interface IDeleteCommandScopePicker
    {
        IDeleteCommandApplicationPicker DeleteCardContent();

        IDeleteCommandKeyPicker DeleteKey();
    }

    public interface IDeleteCommandTokenPicker : IApduBuilder
    {
        IApduBuilder UsingToken(byte[] token);
    }

    /// <summary>
    /// The DELETE command is used to delete a uniquely identifiable object such as an Executable
    /// Load File, an Application, an Executable Load File and its related Applications or a key. In
    /// order to delete an object, the object shall be uniquely identifiable by the selected Application.
    /// <para>Based on section 11.2 of the v2.3 GlobalPlatform Card Specification.</para>
    /// </summary>
    public class DeleteCommand : CommandBase<DeleteCommand, DeleteCommand.P1, DeleteCommand.P2, IDeleteCommandScopePicker>,
        IDeleteCommandScopePicker,
        IDeleteCommandApplicationPicker,
        IDeleteCommandTokenPicker,
        IDeleteCommandKeyPicker
    {
        private byte[] application;
        private byte keyIdentifier;
        private byte keyVersionNumber;
        private DeleteCommandScope scope;
        private byte[] token = new byte[0];

        public enum P1 : byte
        {
            LastOrOnlyCommand = 0b00000000,
            MoreDeleteCommands = 0b10000000,
        }

        public enum P2 : byte
        {
            DeleteObject = 0b00000000,
            DeleteObjectAndRelatedObjects = 0b10000000,
        }

        public enum Tag : byte
        {
            ExecutableLoadFileOrApplicationAID = 0x4F,
            DeleteToken = 0x9E,
            KeyIdentifier = 0xD0,
            KeyVersionNumber = 0xD2,
        }

        private enum DeleteCommandScope
        {
            CardContent,
            Key
        }

        public override Apdu Build()
        {
            var apdu = Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.Delete, this.p1, this.p2);

            var data = new List<byte>();

            switch (this.scope)
            {
                case DeleteCommandScope.CardContent:
                    data.AddTag((byte)Tag.ExecutableLoadFileOrApplicationAID, this.application);

                    if (this.token.Any())
                    {
                        data.AddTag((byte)Tag.DeleteToken, this.token);
                    }
                    break;

                case DeleteCommandScope.Key:
                    if (this.keyIdentifier == 0 && this.keyVersionNumber == 0)
                    {
                        throw new InvalidOperationException("A key identifier or key version number must be specified.");
                    }
                    if (this.keyIdentifier > 0)
                    {
                        data.AddTag((byte)Tag.KeyIdentifier, this.keyIdentifier);
                    }
                    if (this.keyVersionNumber > 0)
                    {
                        data.AddTag((byte)Tag.KeyVersionNumber, this.keyVersionNumber);
                    }
                    break;
            }

            apdu.CommandData = data.ToArray();

            return apdu;
        }

        public IDeleteCommandApplicationPicker DeleteCardContent()
        {
            this.scope = DeleteCommandScope.CardContent;

            return this;
        }

        public IDeleteCommandKeyPicker DeleteKey()
        {
            this.scope = DeleteCommandScope.Key;

            return this;
        }

        /// <summary>
        /// The presence of the Delete Token depends on the Card Issuer’s policy.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IApduBuilder UsingToken(byte[] token)
        {
            if (!token.Any())
            {
                throw new ArgumentException("Length must be at least 1.", nameof(token));
            }

            this.token = token;

            return this;
        }

        /// <summary>
        /// The identity of the Application or Executable Load File to delete shall be specified
        /// using the tag for an AID ('4F') followed by a length and the AID of the Application or
        /// Executable Load File. When simultaneously deleting an Executable Load File and all its
        /// related Applications, only the identity of the Executable Load File shall be provided.
        /// </summary>
        /// <param name="aid"></param>
        /// <returns></returns>
        public IDeleteCommandTokenPicker WithAID(byte[] aid)
        {
            if (aid.Length < 5 || aid.Length > 16)
            {
                throw new ArgumentException("Length must be between 5-16 bytes (inclusive).", nameof(aid));
            }

            this.application = aid;

            return this;
        }

        /// <summary>
        /// A single key is deleted when both the Key Identifier ('D0') and the Key Version Number
        /// ('D2') are provided in the DELETE command message data field. Multiple keys may be
        /// deleted if one of these values is omitted (i.e. all keys with the specified Key
        /// Identifier or Key Version Number). The options available for omitting these values are
        /// conditional on the Issuer’s policy.
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public IDeleteCommandKeyPicker WithIdentifier(byte identifier)
        {
            if (identifier < 1 || identifier > 0x7F)
            {
                throw new ArgumentException("Identifier must be between 1-7F (inclusive).", nameof(identifier));
            }

            this.keyIdentifier = identifier;

            return this;
        }

        public override IP2Picker<P2, IDeleteCommandScopePicker> WithP1(byte p1)
        {
            this.p1 = p1;

            return this;
        }

        public override IP2Picker<P2, IDeleteCommandScopePicker> WithP1(P1 p1) => this.WithP1((byte)p1);

        public override IDeleteCommandScopePicker WithP2(byte p2)
        {
            this.p2 = p2;

            return this;
        }

        public override IDeleteCommandScopePicker WithP2(P2 p2) => this.WithP2((byte)p2);

        /// <summary>
        /// A single key is deleted when both the Key Identifier ('D0') and the Key Version Number
        /// ('D2') are provided in the DELETE command message data field. Multiple keys may be
        /// deleted if one of these values is omitted (i.e. all keys with the specified Key
        /// Identifier or Key Version Number). The options available for omitting these values are
        /// conditional on the Issuer’s policy.
        /// </summary>
        /// <param name="versionNumber"></param>
        /// <returns></returns>
        public IDeleteCommandKeyPicker WithVersionNumber(byte versionNumber)
        {
            if (versionNumber < 1 || versionNumber > 0x7F)
            {
                throw new ArgumentException("Version number must be between 1-7F (inclusive).", nameof(versionNumber));
            }

            this.keyVersionNumber = versionNumber;

            return this;
        }
    }
}
