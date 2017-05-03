using FluentAssertions;
using GlobalPlatform.NET.Commands.Extradite;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GlobalPlatform.NET.Tests.Commands
{
    [TestClass]
    public class ExtraditeCommandTests
    {
        [TestMethod]
        public void ExtraditeCommand_Should_Build()
        {
            byte[] appliation = Enumerable.Range(1, 8).Select(x => (byte)x).ToArray();
            byte[] securityDomain = Enumerable.Range(9, 8).Select(x => (byte)x).ToArray();

            var cmd = ExtraditeCommand.Build
                .Extradite(appliation)
                .To(securityDomain)
                .ToApdu();

            cmd.CLA.Should().Be(ApduClass.GlobalPlatform);
            cmd.INS.Should().Be(ApduInstruction.Install);
            cmd.P1.Should().Be(0b00010000);
            cmd.P2.Should().Be(0x00);
            cmd.Lc.Should().Be(20);
            cmd.CommandData.Skip(0).First().Should().Be(8);
            cmd.CommandData.Skip(1).Take(8).Should().BeEquivalentTo(securityDomain);
            cmd.CommandData.Skip(9).First().Should().Be(0x00);
            cmd.CommandData.Skip(10).First().Should().Be(8);
            cmd.CommandData.Skip(11).Take(8).Should().BeEquivalentTo(appliation);
            cmd.CommandData.Skip(19).First().Should().Be(0x00);
            cmd.Le.Should().Be(0x00);
        }
    }
}
