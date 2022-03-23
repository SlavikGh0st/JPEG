using System.Collections.Generic;

namespace JPEG.NT.Packers
{
    public interface IPacker<T>
    {
        IEnumerable<byte> Pack(T[,] input);
        T[,] Unpack(IReadOnlyList<byte> input);
    }
}