using System;
using System.Linq;
using GlobalPlatform.NET.Commands;

namespace GlobalPlatform.NET
{
    internal class ScratchPad
    {
        public ScratchPad()
        {
            DeleteCommand.Create
                .WithP1(DeleteCommand.P1.LastOrOnlyCommand)
                .WithP2(DeleteCommand.P2.DeleteObjectAndRelatedObjects)
                .DeleteCardContent()
                .WithAID(Enumerable.Range(0, 8).Cast<byte>().ToArray())
                .UsingToken(Enumerable.Range(8, 8).Cast<byte>().ToArray())
                .Build();

            DeleteCommand.Create
                .WithP1(DeleteCommand.P1.LastOrOnlyCommand)
                .WithP2(DeleteCommand.P2.DeleteObject)
                .DeleteKey()
                .WithIdentifier(2)
                .WithVersionNumber(3)
                .Build();
        }
    }
}
