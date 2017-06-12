using GlobalPlatform.NET.Commands.Interfaces;

namespace GlobalPlatform.NET.Commands.Abstractions
{
    public abstract class CommandP1P2Base<TCommand, TP1, TP2, TBuilder> :
        CommandBase<TCommand, IP1Picker<TP1, IP2Picker<TP2, TBuilder>>>,
        IP1Picker<TP1, IP2Picker<TP2, TBuilder>>,
        IP2Picker<TP2, TBuilder>
        where TCommand : class, IP1Picker<TP1, IP2Picker<TP2, TBuilder>>, TBuilder, new()
    {
        public abstract IP2Picker<TP2, TBuilder> WithP1(byte p1);

        public abstract IP2Picker<TP2, TBuilder> WithP1(TP1 p1);

        public abstract TBuilder WithP2(byte p2);

        public abstract TBuilder WithP2(TP2 p2);
    }
}
