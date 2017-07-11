using System.Collections.Generic;
using System.Linq;
using GlobalPlatform.NET.Commands.Interfaces;

namespace GlobalPlatform.NET.Commands.Abstractions
{
    public abstract class MultiCommandBase<TCommand, TBuilder> : IMultiApduBuilder
        where TCommand : TBuilder, new()
    {
        protected byte P1;

        protected byte P2;

        /// <summary>
        /// Starts building the command. 
        /// </summary>
        public static TBuilder Build => new TCommand();

        public abstract IEnumerable<Apdu> AsApdus();

        public IEnumerable<byte[]> AsBytes() => this.AsApdus().Select(apdu => apdu.Buffer);
    }
}
