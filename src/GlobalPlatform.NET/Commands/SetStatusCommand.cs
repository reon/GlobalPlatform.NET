using System;
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

    /// <summary>
    /// The SET STATUS command shall be used to modify the card Life Cycle State or the Application
    /// Life Cycle State.
    /// <para> Based on section 11.10 of the v2.3 GlobalPlatform Card Specification. </para>
    /// </summary>
    public class SetStatusCommand : CommandBase<SetStatusCommand, ISetStatusScopePicker>,
        ISetStatusScopePicker,
        IIssuerSecurityDomainStatusPicker,
        ISecurityDomainStatusPicker,
        IApplicationStatusPicker,
        IApplicationPicker
    {
        public IIssuerSecurityDomainStatusPicker SetIssuerSecurityDomainStatus()
        {
            this.scope = Scope.IssuerSecurityDomain;
            this.application = new byte[0];

            return this;
        }

        public ISecurityDomainStatusPicker SetSecurityDomainStatus()
        {
            this.scope = Scope.SecurityDomain;

            return this;
        }

        public IApplicationStatusPicker SetApplicationStatus()
        {
            this.scope = Scope.Application;

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

        public override Apdu AsApdu()
        {
            this.P1 = (byte)this.scope;

            switch (this.scope)
            {
                case Scope.IssuerSecurityDomain:
                    return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.SetStatus, this.P1, this.P2);

                case Scope.SecurityDomain:
                case Scope.Application:
                    return Apdu.Build(ApduClass.GlobalPlatform, ApduInstruction.SetStatus, this.P1, this.P2, this.application);

                default:
                    throw new NotSupportedException("Scope not supported.");
            }
        }

        private enum Scope : byte
        {
            IssuerSecurityDomain = 0b10000000,
            SecurityDomain = 0b01000000,
            Application = 0b01100000
        }

        private Scope scope;
        private byte[] application;
    }
}
