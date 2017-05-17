using System;
using GlobalPlatform.NET.Commands.Interfaces;

namespace GlobalPlatform.NET.Commands.Abstractions
{
    public abstract class CommandBase<TCommand, TP1, TP2, TBuilder> :
        IP1Picker<TP1, IP2Picker<TP2, TBuilder>>,
        IP2Picker<TP2, TBuilder>,
        IApduBuilder
        where TCommand : class, IP1Picker<TP1, IP2Picker<TP2, TBuilder>>, new()
    {
        protected byte p1;

        protected byte p2;

        /// <summary>
        /// Starts building the command.
        /// </summary>
        public static IP1Picker<TP1, IP2Picker<TP2, TBuilder>> Create => new TCommand();

        public abstract IP2Picker<TP2, TBuilder> WithP1(byte p1);

        public abstract IP2Picker<TP2, TBuilder> WithP1(TP1 p1);

        public abstract TBuilder WithP2(byte p1);

        public abstract TBuilder WithP2(TP2 p1);

        public abstract Apdu Build();
    }
}
