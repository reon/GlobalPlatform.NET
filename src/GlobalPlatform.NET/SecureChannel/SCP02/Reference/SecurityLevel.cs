namespace GlobalPlatform.NET.SecureChannel.SCP02.Reference
{
    public enum SecurityLevel : byte
    {
        Authenticated = 0b00000000,
        CMac = 0b00000001,
        CDecryption = 0b00000011,
        RMac = 0b00010000,
        CMacRMac = 0b00010001,
        CDecryptionCMacRMac = 0b00010011
    }
}
