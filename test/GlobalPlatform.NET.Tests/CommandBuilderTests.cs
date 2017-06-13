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
        private static readonly byte[] token = { 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE };
        private static IEnumerable<Apdu> apdus;

        [TestMethod]
        public void Delete()
        {
            apdus = DeleteCommand.Create
                .DeleteCardContent()
                .WithAID(aid)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x0A, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteCardContent()
                .WithAID(aid)
                .AndRelatedObjects()
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x80, 0x0A, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteCardContent()
                .WithAID(aid)
                .AndRelatedObjects()
                .UsingToken(token)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x80, 0x14, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x9E, 0x08, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x03, 0xD2, 0x01, 0x6F, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteKey()
                .WithIdentifier(0x0F)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x03, 0xD0, 0x01, 0x0F, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .WithIdentifier(0x0F)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x06, 0xD0, 0x01, 0x0F, 0xD2, 0x01, 0x6F, 0x00 });
        }

        [TestMethod]
        public void Select()
        {
            apdus = SelectCommand.Create
                .SelectIssuerSecurityDomain()
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x00 });

            apdus = SelectCommand.Create
                .SelectFirstOrOnlyOccurrence()
                .Of(aid)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = SelectCommand.Create
                .SelectNextOccurrence()
                .Of(aid)
                .Build();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x02, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });
        }
    }
}
