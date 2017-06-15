using System;
using System.Linq;
using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace GlobalPlatform.NET.Tests
{
    [TestClass]
    public class CommandBuilderTests
    {
        private static readonly byte[] aid = { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF };
        private static readonly byte[] token = { 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE };

        [TestMethod]
        public void Delete()
        {
            var apdus = DeleteCommand.Create
                .DeleteCardContent()
                .WithAID(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x0A, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteCardContent()
                .WithAID(aid)
                .AndRelatedObjects()
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x80, 0x0A, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteCardContent()
                .WithAID(aid)
                .AndRelatedObjects()
                .UsingToken(token)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x80, 0x14, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x9E, 0x08, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x03, 0xD2, 0x01, 0x6F, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteKey()
                .WithIdentifier(0x0F)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x03, 0xD0, 0x01, 0x0F, 0x00 });

            apdus = DeleteCommand.Create
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .WithIdentifier(0x0F)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x06, 0xD0, 0x01, 0x0F, 0xD2, 0x01, 0x6F, 0x00 });
        }

        [TestMethod]
        public void GetData()
        {
            var apdus = GetDataCommand.Create
                .GetDataFrom(DataObject.ListApplications)
                .WithTagList(0x5C, 0x00)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xCA, 0x2F, 0x00, 0x02, 0x5C, 0x00, 0x00 });

            apdus = GetDataCommand.Create
                .GetDataFrom(DataObject.KeyInformationTemplate)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xCA, 0x00, 0xE0, 0x00 });
        }

        [TestMethod]
        public void GetStatus()
        {
            var apdus = GetStatusCommand.Create
                .GetStatusOf(GetStatusScope.IssuerSecurityDomain)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF2, 0x80, 0x00, 0x02, 0x4F, 0x00, 0x00 });

            apdus = GetStatusCommand.Create
                .GetStatusOf(GetStatusScope.ExecutableLoadFilesAndModules)
                .WithFilter(new byte[] { 0xA0, 0x00 })
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF2, 0x10, 0x00, 0x04, 0x4F, 0x02, 0xA0, 0x00, 0x00 });
        }

        [TestMethod]
        public void Load()
        {
            byte[] data = new byte[new Random().Next(1, 32768)];
            byte blockSize = (byte)new Random().Next(128, 240);

            var apdus = LoadCommand.Create
                .Load(data)
                .WithBlockSize(blockSize)
                .AsApdu()
                .ToList();

            byte[] dataBlock = apdus.SelectMany(apdu => apdu.CommandData).ToArray();

            apdus.ForEach((apdu, index, isLast) => apdu.Buffer.Take(5)
                .ShouldBeEquivalentTo(new byte[]
                    {0x80, 0xE8, (byte) (isLast ? 0x80 : 0x00), (byte) index, (byte) (isLast ? dataBlock.Length % blockSize : blockSize)}));

            apdus.ForEach(apdu => apdu.Buffer.Last()
                .Should().Be(0x00));

            apdus = LoadCommand.Create
                .WithDapBlock(aid, Enumerable.Range(8, 8).Select(x => (byte)x).ToArray())
                .Load(data)
                .WithBlockSize(blockSize)
                .AsApdu()
                .ToList();

            dataBlock = apdus.SelectMany(apdu => apdu.CommandData).ToArray();

            apdus.ForEach((apdu, index, isLast) => apdu.Buffer.Take(5)
                .ShouldBeEquivalentTo(new byte[]
                    {0x80, 0xE8, (byte) (isLast ? 0x80 : 0x00), (byte) index, (byte) (isLast ? dataBlock.Length % blockSize : blockSize)}));

            apdus.ForEach(apdu => apdu.Buffer.Last()
                .Should().Be(0x00));
        }

        [TestMethod]
        public void ManageChannel()
        {
            var apdus = ManageChannelCommand.Create
                .OpenChannel()
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0x70, 0x00, 0x00, 0x01 });

            apdus = ManageChannelCommand.Create
                .CloseChannel()
                .WithIdentifier(0x01)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0x70, 0x80, 0x01 });
        }

        [TestMethod]
        public void Select()
        {
            var apdus = SelectCommand.Create
                .SelectIssuerSecurityDomain()
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x00 });

            apdus = SelectCommand.Create
                .SelectFirstOrOnlyOccurrence()
                .Of(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = SelectCommand.Create
                .SelectNextOccurrence()
                .Of(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x02, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });
        }

        [TestMethod]
        public void SetStatus()
        {
            var apdus = SetStatusCommand.Create
                .SetIssuerSecurityDomainStatus()
                .To(CardLifeCycleCoding.Initialized)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF0, 0x80, 0x07 });

            apdus = SetStatusCommand.Create
                .SetSecurityDomainStatus()
                .To(SecurityDomainLifeCycleCoding.Personalized)
                .For(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF0, 0x40, 0x0F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = SetStatusCommand.Create
                .SetApplicationStatus()
                .To(ApplicationLifeCycleCoding.Selectable)
                .For(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF0, 0x60, 0x07, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });
        }
    }
}
