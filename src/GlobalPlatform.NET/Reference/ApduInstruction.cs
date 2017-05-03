namespace GlobalPlatform.NET.Reference
{
    public enum ApduInstruction : byte
    {
        Delete = 0xE4,
        GetData = 0xCA,
        GetStatus = 0xF2,
        Install = 0xE6,
        Load = 0xE8,
        ManageChannel = 0x70,
        PutKey = 0xD8,
        Select = 0xA4,
        SetStatus = 0XF0,
        StoreData = 0xE2,
        InitializeUpdate = 0x50,
        ExternalAuthenticate = 0x82,
    }
}
