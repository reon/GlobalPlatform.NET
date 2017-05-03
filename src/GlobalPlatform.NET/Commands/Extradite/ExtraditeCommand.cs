using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using System.Collections.Generic;

namespace GlobalPlatform.NET.Commands.Extradite
{
    /// <summary>
    /// The SELECT command is used for selecting an Application. The OPEN only processes SELECT
    /// commands indicating the SELECT [by name] option. All options other than SELECT [by name]
    /// shall be passed to the currently selected Security Domain or Application on the indicated
    /// logical channel.
    /// <para> Based on v2.3 of the GlobalPlatform Card Specification. </para>
    /// </summary>
    public class ExtraditeCommand : IApduBuilder, IExtraditeCommandApplicationPicker, IExtraditeCommandSecurityDomainPicker
    {
        private byte[] application;

        private byte[] securityDomain;

        /// <summary>
        /// Starts building the command. 
        /// </summary>
        public static IExtraditeCommandApplicationPicker Build => new ExtraditeCommand();

        public IExtraditeCommandSecurityDomainPicker Extradite(byte[] application)
        {
            this.application = application;

            return this;
        }

        public IApduBuilder To(byte[] securityDomain)
        {
            this.securityDomain = securityDomain;

            return this;
        }

        public Apdu ToApdu()
        {
            var data = new List<byte>();

            data.AddRangeWithLength(this.securityDomain);
            data.Add(0x00);
            data.AddRangeWithLength(this.application);
            data.Add(0x00);

            return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.Install, 0b00010000, 0, data.ToArray());
        }

        public static class P1
        {
            public static byte SelectByName = 0b00000100;
        }

        public static class P2
        {
            public static byte FirstOrOnlyOccurrence = 0b00000000;
            public static byte NextOccurrence = 0b00000010;
        }
    }

    public interface IExtraditeCommandApplicationPicker
    {
        IExtraditeCommandSecurityDomainPicker Extradite(byte[] application);
    }

    public interface IExtraditeCommandSecurityDomainPicker
    {
        IApduBuilder To(byte[] securityDomain);
    }
}
