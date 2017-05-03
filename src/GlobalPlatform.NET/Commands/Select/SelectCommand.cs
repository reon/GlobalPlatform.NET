using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands.Select
{
    /// <summary>
    /// The SELECT command is used for selecting an Application. The OPEN only processes SELECT
    /// commands indicating the SELECT [by name] option. All options other than SELECT [by name]
    /// shall be passed to the currently selected Security Domain or Application on the indicated
    /// logical channel.
    /// <para> Based on v2.3 of the GlobalPlatform Card Specification. </para>
    /// </summary>
    public class SelectCommand : IApduBuilder, ISelectCommandP1Picker, ISelectCommandP2Picker, ISelectCommandApplicationPicker
    {
        private byte p1;

        private byte p2;

        private byte[] application;

        /// <summary>
        /// Starts building the command. 
        /// </summary>
        public static ISelectCommandP1Picker Build => new SelectCommand();

        public ISelectCommandP2Picker WithP1(byte p1)
        {
            this.p1 = p1;

            return this;
        }

        public ISelectCommandApplicationPicker WithP2(byte p2)
        {
            this.p2 = p2;

            return this;
        }

        public IApduBuilder SelectIssuerSecurityDomain()
        {
            this.application = new byte[0];

            return this;
        }

        public IApduBuilder Select(byte[] application)
        {
            this.application = application;

            return this;
        }

        public Apdu ToApdu() => Apdu.Build(ApduClass.Iso7816, ApduInstruction.Select, this.p1, this.p2, this.application);

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

    public interface ISelectCommandP1Picker
    {
        ISelectCommandP2Picker WithP1(byte p1);
    }

    public interface ISelectCommandP2Picker
    {
        ISelectCommandApplicationPicker WithP2(byte p2);
    }

    public interface ISelectCommandApplicationPicker
    {
        IApduBuilder SelectIssuerSecurityDomain();

        IApduBuilder Select(byte[] application);
    }
}