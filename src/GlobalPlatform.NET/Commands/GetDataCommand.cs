using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GlobalPlatform.NET.Commands
{
    public class GetDataCommand : IP1P2Picker<GetDataCommand.P1P2, IApduBuilder>,
        IApduBuilder
    {
        private P1P2 p1p2;

        public enum P1P2 : ushort
        {
            IssuerIdentificationNumber = 0x0042,
            CardImageNumber = 0x0045,
            CardData = 0x0066,
            KeyInformationTemplate = 0x00E0,
            CardCapabilityInformation = 0x0067,
            CurrentSecurityLevel = 0x00D3,
            ListApplications = 0x2F00,
            ExtendedCardResourcesInformation = 0xFF21,
            SecurityDomainManagerUrl = 0x5F50
        }

        /// <summary>
        /// Starts building the command. 
        /// </summary>
        public static IP1P2Picker<P1P2, IApduBuilder> Create => new GetDataCommand();

        public IEnumerable<Apdu> Build()
        {
            var bytes = BitConverter.GetBytes((ushort)this.p1p2);

            if (BitConverter.IsLittleEndian)
            {
                bytes.Reverse();
            }

            byte p1 = bytes.First();
            byte p2 = bytes.Last();

            yield return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.GetData, p1, p2);
        }

        public IApduBuilder WithP1P2(P1P2 p1p2)
        {
            this.p1p2 = p1p2;

            return this;
        }
    }
}
