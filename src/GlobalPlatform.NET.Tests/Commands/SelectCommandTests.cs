using FluentAssertions;
using GlobalPlatform.NET.Commands.Select;
using GlobalPlatform.NET.Extensions;
using GlobalPlatform.NET.Reference;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace GlobalPlatform.NET.Tests.Commands
{
    [TestClass]
    public class SelectCommandTests
    {
        [TestMethod]
        public void SelectCommand_Should_Build()
        {
            var cmd = SelectCommand.Build
                .WithP1(SelectCommand.P1.SelectByName)
                .WithP2(SelectCommand.P2.FirstOrOnlyOccurrence)
                .SelectIssuerSecurityDomain()
                .ToApdu();

            cmd.CLA.Should().Be(ApduClass.Iso7816);
            cmd.INS.Should().Be(ApduInstruction.Select);
            cmd.P1.Should().Be(SelectCommand.P1.SelectByName);
            cmd.P2.Should().Be(SelectCommand.P2.FirstOrOnlyOccurrence);
            cmd.Lc.Should().Be(0x00);
            cmd.CommandData.Should().BeEquivalentTo(new byte[0]);
            cmd.Le.Should().Be(0x00);
        }

        [TestMethod]
        public void SelectCommand_With_AID_Should_Build()
        {
            byte[] aid = Enumerable.Range(1, 8).Select(x => (byte)x).ToArray();

            var cmd = SelectCommand.Build
                .WithP1(SelectCommand.P1.SelectByName)
                .WithP2(SelectCommand.P2.FirstOrOnlyOccurrence)
                .Select(aid)
                .ToApdu();

            cmd.CLA.Should().Be(ApduClass.Iso7816);
            cmd.INS.Should().Be(ApduInstruction.Select);
            cmd.P1.Should().Be(SelectCommand.P1.SelectByName);
            cmd.P2.Should().Be(SelectCommand.P2.FirstOrOnlyOccurrence);
            cmd.Lc.Should().Be(aid.LengthChecked());
            cmd.CommandData.Should().BeEquivalentTo(aid);
            cmd.Le.Should().Be(0x00);
        }
    }
}