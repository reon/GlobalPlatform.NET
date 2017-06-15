namespace GlobalPlatform.NET.Reference
{
    public enum CardLifeCycleCoding : byte
    {
        OpReady = 0b00000001,
        Initialized = 0b00000111,
        Secured = 0b00001111,
        CardLocked = 0b01111111,
        Terminated = 0b11111111
    }
}
