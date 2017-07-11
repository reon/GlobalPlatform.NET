using System.Collections.Generic;

namespace GlobalPlatform.NET.Commands.Interfaces
{
    public interface IMultiApduBuilder
    {
        IEnumerable<Apdu> AsApdus();

        IEnumerable<byte[]> AsBytes();
    }
}
