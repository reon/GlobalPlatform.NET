using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using GlobalPlatform.NET.Commands;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests
{
    [TestClass]
    public class CommandBuilderTests
    {
        private static readonly byte[] aid = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        private static IEnumerable<Apdu> apdus;

        [TestMethod]
        public void Select()
        {
            apdus = SelectCommand.Create
                .WithP1(SelectCommand.P1.SelectByName)
                .WithP2(SelectCommand.P2.FirstOrOnlyOccurrence)
                .SelectIssuerSecurityDomain()
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x00 });

            apdus = SelectCommand.Create
                .WithP1(SelectCommand.P1.SelectByName)
                .WithP2(SelectCommand.P2.NextOccurrence)
                .Select(aid)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x02, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });
        }
    }
}
