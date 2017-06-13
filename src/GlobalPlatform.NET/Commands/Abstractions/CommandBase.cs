using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Commands.Interfaces;

namespace GlobalPlatform.NET.Commands.Abstractions
{
    public abstract class CommandBase<TCommand, TBuilder> : IApduBuilder
        where TCommand : TBuilder, new()
    {
        protected byte P1;

        protected byte P2;

        /// <summary>
        /// Starts building the command. 
        /// </summary>
        public static TBuilder Create => new TCommand();

        public abstract IEnumerable<Apdu> AsApdu();

        public IEnumerable<byte[]> AsBytes() => this.AsApdu().Select(x => x.Buffer);
    }
}
