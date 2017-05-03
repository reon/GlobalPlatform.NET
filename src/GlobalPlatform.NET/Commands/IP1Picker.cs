using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalPlatform.NET.Commands
{
    public interface IP1Picker<TBuilder>
    {
        IP2Picker<TBuilder> WithP1(byte p1);
    }
}
