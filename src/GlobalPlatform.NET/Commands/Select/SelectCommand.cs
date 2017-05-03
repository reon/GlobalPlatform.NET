using System;

namespace GlobalPlatform.NET.Commands
{
    public class SelectCommand : CommandBase<SelectCommand>, IApduBuilder, ISelectCommandApplicationPicker
    {
        public byte[] Application { get; set; }

        public static IP1Picker<SelectCommand> Build => new SelectCommand();

        public IApduBuilder ApplicationWithAID(byte[] application)
        {
            this.Application = application;

            return this;
        }

        public byte[] ToApdu() => throw new NotImplementedException();
    }

    public interface ISelectCommandApplicationPicker
    {
        IApduBuilder ApplicationWithAID(byte[] application);
    }
}