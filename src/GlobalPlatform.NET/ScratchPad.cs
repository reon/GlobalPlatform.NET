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
            GetDataCommand.Create
                .WithP1P2(GetDataCommand.P1P2.ListApplications)
                .AsApdu();

            GetStatusCommand.Create
                .GetStatusOf(GetStatusScope.IssuerSecurityDomain)
                .AsApdu();

            GetStatusCommand.Create
                .GetStatusOf(GetStatusScope.ExecutableLoadFilesAndModules)
                .WithFilter(new byte[] { 0xA0, 0x00 })
                .AsApdu();

            LoadCommand.Create
                .WithDapBlock(aid, Enumerable.Range(8, 8).Cast<byte>().ToArray())
                .Load(new byte[8192])
                .WithBlockSize(0x80)
                .AsApdu();

            LoadCommand.Create
                .Load(new byte[4096])
                .WithBlockSize(0xB0)
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
