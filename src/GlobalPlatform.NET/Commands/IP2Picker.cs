using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPlatform.NET.Commands
{
    public interface IP2Picker<TBuilder>
    {
        TBuilder WithP2(byte p2);
    }
}
