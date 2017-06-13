using System;
using System.Collections.Generic;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public interface ISelectCommandScopePicker
    {
        IApduBuilder SelectIssuerSecurityDomain();

        ISelectCommandApplicationPicker SelectFirstOrOnlyOccurrence();

        ISelectCommandApplicationPicker SelectNextOccurrence();
    }

    public interface ISelectCommandApplicationPicker
    {
        IApduBuilder Of(byte[] application);
    }

    /// <summary>
    /// The SELECT command is used for selecting an Application. The OPEN only processes SELECT
    /// commands indicating the SELECT [by name] option. All options other than SELECT [by name]
    /// shall be passed to the currently selected Security Domain or Application on the indicated
    /// logical channel.
    /// <para> Based on section 11.9 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class SelectCommand : CommandBase<SelectCommand, ISelectCommandScopePicker>,
        ISelectCommandScopePicker,
        ISelectCommandApplicationPicker
    {
        private byte[] application;

        public IApduBuilder SelectIssuerSecurityDomain()
        {
            this.application = new byte[0];

            return this;
        }

        public ISelectCommandApplicationPicker SelectFirstOrOnlyOccurrence()
        {
            this.P2 = 0b00000000;

            return this;
        }

        public ISelectCommandApplicationPicker SelectNextOccurrence()
        {
            this.P2 = 0b00000010;

            return this;
        }

        public IApduBuilder Of(byte[] application)
        {
            if (application.Length < 5 || application.Length > 16)
            {
                throw new ArgumentException("Length must be between 5-16 bytes (inclusive).", nameof(application));
            }

            this.application = application;

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            yield return Apdu.Build(ApduClass.Iso7816, ApduInstruction.Select, 0x04, this.P2, this.application);
        }
    }
}
