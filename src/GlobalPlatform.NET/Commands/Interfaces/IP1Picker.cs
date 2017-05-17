namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IP1Picker<in TP1, out TP2Picker>
    {
        TP2Picker WithP1(byte p1);

        TP2Picker WithP1(TP1 p1);
    }
}
