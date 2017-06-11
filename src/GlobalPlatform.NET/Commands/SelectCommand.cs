using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;
using System;

namespace GlobalPlatform.NET.Commands
{
    public interface ISelectCommandApplicationPicker
    {
        /// <summary>
        /// The data field of the command shall contain the AID of the Application to be selected.
        /// The Lc and data field of the SELECT command may be omitted if the Issuer Security Domain
        /// is being selected.
        /// </summary>
        /// <param name="application"></param>
        /// <returns></returns>
        IApduBuilder Select(byte[] application);

        /// <summary>
        /// The data field of the command shall contain the AID of the Application to be selected.
        /// The Lc and data field of the SELECT command may be omitted if the Issuer Security Domain
        /// is being selected.
        /// </summary>
        /// <returns></returns>
        IApduBuilder SelectIssuerSecurityDomain();
    }

    /// <summary>
    /// The SELECT command is used for selecting an Application. The OPEN only processes SELECT
    /// commands indicating the SELECT [by name] option. All options other than SELECT [by name]
    /// shall be passed to the currently selected Security Domain or Application on the indicated
    /// logical channel.
    /// <para>Based on section 11.9 of the v2.3 GlobalPlatform Card Specification.</para>
    /// </summary>
    public class SelectCommand : CommandP1P2Base<SelectCommand, SelectCommand.P1, SelectCommand.P2, ISelectCommandApplicationPicker>,
        ISelectCommandApplicationPicker
    {
        private byte[] application;

        public enum P1 : byte
        {
            SelectByName = 0b00000100,
        }

        public enum P2
        {
            FirstOrOnlyOccurrence = 0b00000000,
            NextOccurrence = 0b00000010,
        }

        public override Apdu Build() => Apdu.Build(ApduClass.Iso7816, ApduInstruction.Select, this.p1, this.p2, this.application);

        public IApduBuilder Select(byte[] application)
        {
            if (application.Length < 5 || application.Length > 16)
            {
                throw new ArgumentException("Length must be between 5-16 bytes (inclusive).", nameof(application));
            }

            this.application = application;

            return this;
        }

        public IApduBuilder SelectIssuerSecurityDomain()
        {
            this.application = new byte[0];

            return this;
        }

        public override IP2Picker<P2, ISelectCommandApplicationPicker> WithP1(byte p1)
        {
            this.p1 = p1;

            return this;
        }

        public override IP2Picker<P2, ISelectCommandApplicationPicker> WithP1(P1 p1) => this.WithP1((byte)p1);

        public override ISelectCommandApplicationPicker WithP2(byte p2)
        {
            this.p2 = p2;

            return this;
        }

        public override ISelectCommandApplicationPicker WithP2(P2 p2) => this.WithP2((byte)p2);
    }
}
