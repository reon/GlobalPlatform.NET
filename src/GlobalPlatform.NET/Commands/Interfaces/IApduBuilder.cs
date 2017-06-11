using System.Collections.Generic;

namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IApduBuilder
    {
        IEnumerable<Apdu> Build();
    }
}
