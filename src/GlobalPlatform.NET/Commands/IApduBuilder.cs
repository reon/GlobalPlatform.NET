using System;

namespace GlobalPlatform.NET.Commands
{
    public interface IApduBuilder
    {
        Apdu ToApdu();
    }
}
