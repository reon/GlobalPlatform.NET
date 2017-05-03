using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPlatform.NET.Commands
{
    public interface IApduBuilder
    {
        byte[] ToApdu();
    }
}
