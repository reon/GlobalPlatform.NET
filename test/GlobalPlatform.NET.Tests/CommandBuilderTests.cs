using System;
using FluentAssertions;
using GlobalPlatform.NET.Commands;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

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
            var apdus = DeleteCommand.Build
                .DeleteCardContent()
                .WithAID(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x0A, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = DeleteCommand.Build
                .DeleteCardContent()
                .WithAID(aid)
                .AndRelatedObjects()
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x80, 0x0A, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = DeleteCommand.Build
                .DeleteCardContent()
                .WithAID(aid)
                .AndRelatedObjects()
                .UsingToken(token)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x80, 0x14, 0x4F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x9E, 0x08, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0xEE, 0x00 });

            apdus = DeleteCommand.Build
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x03, 0xD2, 0x01, 0x6F, 0x00 });

            apdus = DeleteCommand.Build
                .DeleteKey()
                .WithIdentifier(0x0F)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x03, 0xD0, 0x01, 0x0F, 0x00 });

            apdus = DeleteCommand.Build
                .DeleteKey()
                .WithVersionNumber(0x6F)
                .WithIdentifier(0x0F)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xE4, 0x00, 0x00, 0x06, 0xD0, 0x01, 0x0F, 0xD2, 0x01, 0x6F, 0x00 });
        }

        [TestMethod]
        public void GetData()
        {
            var apdus = GetDataCommand.Build
                .GetDataFrom(DataObject.ListApplications)
                .WithTagList(0x5C, 0x00)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xCA, 0x2F, 0x00, 0x02, 0x5C, 0x00, 0x00 });

            apdus = GetDataCommand.Build
                .GetDataFrom(DataObject.KeyInformationTemplate)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xCA, 0x00, 0xE0, 0x00 });
        }

        [TestMethod]
        public void GetStatus()
        {
            var apdus = GetStatusCommand.Build
                .GetStatusOf(GetStatusScope.IssuerSecurityDomain)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF2, 0x80, 0x00, 0x02, 0x4F, 0x00, 0x00 });

            apdus = GetStatusCommand.Build
                .GetStatusOf(GetStatusScope.ExecutableLoadFilesAndModules)
                .WithFilter(new byte[] { 0xA0, 0x00 })
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF2, 0x10, 0x00, 0x04, 0x4F, 0x02, 0xA0, 0x00, 0x00 });
        }

        [TestMethod]
        public void Load()
        {
            byte[] data = new byte[new Random().Next(1, 32767)];
            byte blockSize = (byte)new Random().Next(128, 240);

            var apdus = LoadCommand.Build
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

            apdus = LoadCommand.Build
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
            var apdus = ManageChannelCommand.Build
                .OpenChannel()
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0x70, 0x00, 0x00, 0x01 });

            apdus = ManageChannelCommand.Build
                .CloseChannel()
                .WithIdentifier(0x01)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0x70, 0x80, 0x01 });
        }

        [TestMethod]
        public void PutKey()
        {
            const byte keyVersion = 0x7F;
            const byte keyIdentifier = 0x01;

            var apdus = PutKeyCommand.Build
                .WithKeyVersion(keyVersion)
                .WithKeyIdentifier(1)
                .UsingKEK(new byte[16])
                .PutFirstKey(KeyTypeCoding.DES, new byte[16])
                .PutSecondKey(KeyTypeCoding.DES, new byte[16])
                .PutThirdKey(KeyTypeCoding.DES, new byte[16])
                .AsApdu()
                .ToList();

            apdus.First().Buffer.Take(5).Should().BeEquivalentTo(new byte[] { 0x80, 0xD8, keyVersion, keyIdentifier, 0x3A });
            apdus.First().CommandData.First().Should().Be(keyVersion);
            apdus.First().CommandData.Skip(1).Split(19).ForEach(block =>
            {
                block.First().Should().Be(0x80);
                block.Skip(1).First().Should().Be(0x10);
            });
        }

        [TestMethod]
        public void Select()
        {
            var apdus = SelectCommand.Build
                .SelectIssuerSecurityDomain()
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x00 });

            apdus = SelectCommand.Build
                .SelectFirstOrOnlyOccurrence()
                .Of(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x00, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = SelectCommand.Build
                .SelectNextOccurrence()
                .Of(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x00, 0xA4, 0x04, 0x02, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });
        }

        [TestMethod]
        public void SetStatus()
        {
            var apdus = SetStatusCommand.Build
                .SetIssuerSecurityDomainStatus()
                .To(CardLifeCycleCoding.Initialized)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF0, 0x80, 0x07 });

            apdus = SetStatusCommand.Build
                .SetSecurityDomainStatus()
                .To(SecurityDomainLifeCycleCoding.Personalized)
                .For(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF0, 0x40, 0x0F, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });

            apdus = SetStatusCommand.Build
                .SetApplicationStatus()
                .To(ApplicationLifeCycleCoding.Selectable)
                .For(aid)
                .AsApdu();

            apdus.First().Buffer.Should().BeEquivalentTo(new byte[] { 0x80, 0xF0, 0x60, 0x07, 0x08, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x00 });
        }

        [TestMethod]
        public void StoreData()
        {
            byte[] data = new byte[new Random().Next(256, 510)];
            byte blockSize = (byte)new Random().Next(128, 240);

            var apdus = StoreDataCommand.Build
                .WithP1(0x10)
                .StoreData(data)
                .WithBlockSize(blockSize)
                .AsApdu()
                .ToList();

            apdus.First().Buffer.Take(5).Should().BeEquivalentTo(new byte[] { 0x80, 0xE2, 0x10, 0x00, blockSize });
        }
    }
}