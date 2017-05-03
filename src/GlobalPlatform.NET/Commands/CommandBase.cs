namespace GlobalPlatform.NET.Commands
{
    public abstract class CommandBase<TBuilder> : IP1Picker<TBuilder>, IP2Picker<TBuilder>, TBuilder
    {
        public byte P1 { get; set; }
        public byte P2 { get; set; }

        public IP2Picker<TBuilder> WithP1(byte p1)
        {
            this.P1 = p1;

            return this;
        }

        public TBuilder WithP2(byte p2)
        {
            this.P2 = p2;

            return this;
        }
    }
}