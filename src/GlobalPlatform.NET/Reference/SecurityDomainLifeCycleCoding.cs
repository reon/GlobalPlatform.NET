namespace GlobalPlatform.NET.Reference
{
    public enum SecurityDomainLifeCycleCoding : byte
    {
        Installed = 0b00000011,
        Selectable = 0b00000111,
        Personalized = 0b00001111,
        Locked = 0b10000011
    }
}
