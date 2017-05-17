using System;
using System.Collections.Generic;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    /// <summary>
    /// The DELETE command is used to delete a uniquely identifiable object such as an Executable
    /// Load File, an Application, an Executable Load File and its related Applications or a key. In
    /// order to delete an object, the object shall be uniquely identifiable by the selected Application.
    /// <para>Based on section 11.2 of the v2.3 GlobalPlatform Card Specification.</para>
    /// </summary>
    public class DeleteCommand : IApduBuilder, IDeleteCommandP1Picker, IDeleteCommandP2Picker, IDeleteCommandScopePicker, IDeleteCommandApplicationPicker, IDeleteCommandKeyPicker
    {
        private byte p1;

        private byte p2;

        private byte[] application;

        private byte keyIdentifier;
        private byte keyVersionNumber;

        private DeleteCommandScope scope;

        /// <summary>
        /// Starts building the command.
        /// </summary>
        public static IDeleteCommandP1Picker Build => new DeleteCommand();

        public IDeleteCommandP2Picker WithP1(byte p1)
        {
            this.p1 = p1;

            return this;
        }

        public IDeleteCommandScopePicker WithP2(byte p2)
        {
            this.p2 = p2;

            return this;
        }

        private enum DeleteCommandScope
        {
            CardContent,
            Key
        }
        public IDeleteCommandApplicationPicker WithCardContentScope()
        {
            this.scope = DeleteCommandScope.CardContent;

            return this;
        }

        public IDeleteCommandKeyPicker WithKeyScope()
        {
            this.scope = DeleteCommandScope.Key;

            return this;
        }

        public IApduBuilder Delete(byte[] aid)
        {
            if (aid.Length < 5 || aid.Length > 16)
            {
                throw new ArgumentException("Length must be between 5-16 bytes (inclusive).", nameof(aid));
            }

            this.application = aid;

            return this;
        }

        public IDeleteCommandKeyPicker DeleteIdentifier(byte identifier)
        {
            if (identifier < 1 || identifier > 0x7F)
            {
                throw new ArgumentException("Identifier must be between 1-7F (inclusive).", nameof(identifier));
            }

            this.keyIdentifier = identifier;

            return this;
        }
        public IDeleteCommandKeyPicker DeleteVersionNumber(byte versionNumber)
        {
            if (versionNumber < 1 || versionNumber > 0x7F)
            {
                throw new ArgumentException("Version number must be between 1-7F (inclusive).", nameof(versionNumber));
            }

            this.keyVersionNumber = versionNumber;

            return this;
        }

        public Apdu ToApdu()
        {
            var apdu = Apdu.Build(ApduClass.Iso7816, ApduInstruction.Delete, this.p1, this.p2);

            var data = new List<byte>();

            switch (this.scope)
            {
                case DeleteCommandScope.CardContent:
                    data.Add(0x4F);
                    data.AddRangeWithLength(this.application);
                    break;

                case DeleteCommandScope.Key:
                    if (this.keyIdentifier == 0 && this.keyVersionNumber == 0)
                    {
                        throw new InvalidOperationException("A key identifier or key version number must be specified.");
                    }
                    if (this.keyIdentifier > 0)
                    {
                        data.AddTag(Tag.KeyIdentifier, this.keyIdentifier);
                    }
                    if (this.keyVersionNumber > 0)
                    {
                        data.AddTag(Tag.KeyVersionNumber, this.keyVersionNumber);
                    }
                    break;
            }

            apdu.CommandData = data.ToArray();

            return apdu;
        }

        public static class P1
        {
            public static byte LastOrOnlyCommand = 0b00000000;
            public static byte MoreDeleteCommands = 0b10000000;
        }

        public static class P2
        {
            public static byte DeleteObject = 0b00000000;
            public static byte DeleteObjectAndRelatedObjects = 0b10000000;
        }
    }

    public interface IDeleteCommandP1Picker
    {
        IDeleteCommandP2Picker WithP1(byte p1);
    }

    public interface IDeleteCommandP2Picker
    {
        IDeleteCommandScopePicker WithP2(byte p2);
    }

    public interface IDeleteCommandScopePicker
    {
        IDeleteCommandApplicationPicker WithCardContentScope();
        IDeleteCommandKeyPicker WithKeyScope();
    }

    public interface IDeleteCommandApplicationPicker
    {
        IApduBuilder Delete(byte[] aid);
    }

    public interface IDeleteCommandKeyPicker : IApduBuilder
    {
        IDeleteCommandKeyPicker DeleteIdentifier(byte keyIdentifier);
        IDeleteCommandKeyPicker DeleteVersionNumber(byte keyVersionNumber);
    }
}
