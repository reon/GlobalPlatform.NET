namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IApduBuilder
    {
        Apdu AsApdu();

        byte[] AsBytes();
    }
}
