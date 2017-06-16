namespace GlobalPlatform.NET.SecureChannel.Interfaces
{
    public interface ISecureChannelSession
    {
        Apdu SecureApdu(Apdu apdu);
    }
}
