using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public interface IChunkCache
    {
        int Size { get; }
        IEnumerable<IChunk> Values { get; }
        IChunk Get(Index3 key);
        IChunk Get(int index);

        void Ensure(Index3 key);

        void Release(Index3 key);

        void Flush();
        void Ensure(int x, int y, int z);
        void Release(int x, int y, int z);
    }
}