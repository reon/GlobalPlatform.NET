using GlobalPlatform.NET.Commands.Interfaces;
using System.Collections.Generic;

namespace GlobalPlatform.NET.Commands.Abstractions
{
    public abstract class CommandBase<TCommand, TBuilder> :
        IApduBuilder
        where TCommand : class, TBuilder, new()
    {
        protected byte p1;

        protected byte p2;

        /// <summary>
        /// Starts building the command.
        /// </summary>
        public static TBuilder Create => new TCommand();

        public abstract IEnumerable<Apdu> Build();
    }
}
