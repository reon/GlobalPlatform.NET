using System.Linq;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET
{
    internal class ScratchPad
    {
        private static readonly byte[] aid = Enumerable.Range(0, 8).Cast<byte>().ToArray();

        public ScratchPad()
        {
            LoadCommand.Create
                .WithDapBlock(aid, Enumerable.Range(8, 8).Cast<byte>().ToArray())
                .Load(new byte[8192])
                .WithBlockSize(0x80)
                .AsApdu();

            ManageChannelCommand.Create
                .OpenChannel()
                .AsApdu();

            ManageChannelCommand.Create
                .CloseChannel()
                .WithIdentifier(0x01)
                .AsApdu();

            SetStatusCommand.Create
                .SetIssuerSecurityDomainStatus()
                .To(CardLifeCycleCoding.Initialized)
                .AsApdu();

            SetStatusCommand.Create
                .SetSecurityDomainStatus()
                .To(SecurityDomainLifeCycleCoding.Personalized)
                .For(aid)
                .AsApdu();

            SetStatusCommand.Create
                .SetApplicationStatus()
                .To(ApplicationLifeCycleCoding.Selectable)
                .For(aid)
                .AsApdu();
        }
    }
}
