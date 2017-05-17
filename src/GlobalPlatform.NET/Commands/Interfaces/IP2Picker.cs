namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IP2Picker<in TP2, out TBuilder>
    {
        TBuilder WithP2(byte p1);

        TBuilder WithP2(TP2 p1);
    }
}
