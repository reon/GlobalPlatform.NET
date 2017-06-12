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
            DeleteCommand.Create
                .WithP1(DeleteCommand.P1.LastOrOnlyCommand)
                .WithP2(DeleteCommand.P2.DeleteObjectAndRelatedObjects)
                .DeleteCardContent()
                .WithAID(aid)
                .UsingToken(Enumerable.Range(8, 8).Cast<byte>().ToArray())
                .Build();

            DeleteCommand.Create
                .WithP1(DeleteCommand.P1.LastOrOnlyCommand)
                .WithP2(DeleteCommand.P2.DeleteObject)
                .DeleteKey()
                .WithIdentifier(2)
                .WithVersionNumber(3)
                .Build();

            GetDataCommand.Create
                .WithP1P2(GetDataCommand.P1P2.ListApplications)
                .Build();

            GetStatusCommand.Create
                .WithP1(GetStatusCommand.P1.IssuerSecurityDomain)
                .WithP2(GetStatusCommand.P2.GetFirstOrAllOccurrences)
                .WithNoFilter()
                .Build();

            GetStatusCommand.Create
                .WithP1(GetStatusCommand.P1.ExecutableLoadFilesAndModules)
                .WithP2(GetStatusCommand.P2.GetFirstOrAllOccurrences)
                .WithFilter(new byte[] { 0xA0, 0x00 })
                .Build();

            LoadCommand.Create
                .WithDapBlock(aid, Enumerable.Range(8, 8).Cast<byte>().ToArray())
                .WithData(new byte[8192])
                .WithBlockSize(0x80)
                .Build();

            LoadCommand.Create
                .WithData(new byte[4096])
                .WithBlockSize(0xB0)
                .Build();

            ManageChannelCommand.Create
                .OpenChannel()
                .Build();

            ManageChannelCommand.Create
                .CloseChannel()
                .WithIdentifier(0x01)
                .Build();

            SelectCommand.Create
                .WithP1(SelectCommand.P1.SelectByName)
                .WithP2(SelectCommand.P2.FirstOrOnlyOccurrence)
                .SelectIssuerSecurityDomain()
                .Build();

            SelectCommand.Create
                .WithP1(SelectCommand.P1.SelectByName)
                .WithP2(SelectCommand.P2.NextOccurrence)
                .Select(aid)
                .Build();

            SetStatusCommand.Create
                .SetIssuerSecurityDomainStatus()
                .To(CardLifeCycleCoding.Initialized)
                .Build();

            SetStatusCommand.Create
                .SetSecurityDomainStatus()
                .To(SecurityDomainLifeCycleCoding.Personalized)
                .For(aid)
                .Build();

            SetStatusCommand.Create
                .SetApplicationStatus()
                .To(ApplicationLifeCycleCoding.Selectable)
                .For(aid)
                .Build();
        }
    }
}
