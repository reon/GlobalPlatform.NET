using System.Collections.Generic;
using GlobalPlatform.NET.Commands.Abstractions;
using GlobalPlatform.NET.Commands.Interfaces;
using GlobalPlatform.NET.Reference;

namespace GlobalPlatform.NET.Commands
{
    public interface ISetStatusScopePicker
    {
        IIssuerSecurityDomainStatusPicker SetIssuerSecurityDomainStatus();

        ISecurityDomainStatusPicker SetSecurityDomainStatus();

        IApplicationStatusPicker SetApplicationStatus();
    }

    public interface IIssuerSecurityDomainStatusPicker
    {
        IApduBuilder To(CardLifeCycleCoding status);
    }

    public interface ISecurityDomainStatusPicker
    {
        IApplicationPicker To(SecurityDomainLifeCycleCoding status);
    }

    public interface IApplicationStatusPicker
    {
        IApplicationPicker To(ApplicationLifeCycleCoding status);
    }

    public interface IApplicationPicker
    {
        IApduBuilder For(byte[] application);
    }

    public class SetStatusCommand : CommandBase<SetStatusCommand, ISetStatusScopePicker>,
        ISetStatusScopePicker,
        IIssuerSecurityDomainStatusPicker,
        ISecurityDomainStatusPicker,
        IApplicationStatusPicker,
        IApplicationPicker
    {
        public IIssuerSecurityDomainStatusPicker SetIssuerSecurityDomainStatus()
        {
            this.P1 = (byte)Scope.SecurityDomain;
            this.application = new byte[0];

            return this;
        }

        public ISecurityDomainStatusPicker SetSecurityDomainStatus()
        {
            this.P1 = (byte)Scope.Application;

            return this;
        }

        public IApplicationStatusPicker SetApplicationStatus()
        {
            this.P1 = (byte)Scope.IssuerSecurityDomain;

            return this;
        }

        public IApduBuilder To(CardLifeCycleCoding status)
        {
            this.P2 = (byte)status;

            return this;
        }

        public IApplicationPicker To(SecurityDomainLifeCycleCoding status)
        {
            this.P2 = (byte)status;

            return this;
        }

        public IApplicationPicker To(ApplicationLifeCycleCoding status)
        {
            this.P2 = (byte)status;

            return this;
        }

        public IApduBuilder For(byte[] application)
        {
            this.application = application;

            return this;
        }

        public override IEnumerable<Apdu> AsApdu()
        {
            yield return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.SetStatus, this.P1, this.P2, this.application);
        }

        private enum Scope : byte
        {
            IssuerSecurityDomain = 0b10000000,
            SecurityDomain = 0b01000000,
            Application = 0b01100000
        }

        private byte[] application;
    }
}
