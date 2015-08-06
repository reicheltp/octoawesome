using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    public sealed class ChunkCache : IChunkCache
    {
        private readonly IChunk[] _values;

        private readonly Func<Index3, IChunk> _load;
        private readonly Action<Index3, IChunk> _unload;

        public ChunkCache(int size, Func<Index3, IChunk> load, Action<Index3, IChunk> unload)
        {
            _unload = unload;
            _load = load;
            Size = size;
            _values = new IChunk[32768];
        }

        public int Size { get; private set; }

        public IEnumerable<IChunk> Values
        {
            get { return _values; }
        }

        public IChunk Get(Index3 key)
        {
            return _values[Flat(key.X, key.Y, key.Z)];
        }

        public int Flat(int x, int y, int z)
        {
            return (z & 31) << 10 | (y & 31) << 5 | (x & 31);
        }

        public IChunk Get(int index)
        {
            return _values[index];
        }

        public void Ensure(Index3 key)
        {
            var idx = Flat(key.X, key.Y, key.Z);
            if (_values[idx] != null)
                return;

            _values[idx] = _load(key);
        }

        public void Release(Index3 key)
        {
            _values[Flat(key.X, key.Y, key.Z)] = null;
        }

        public void Release(int index)
        {
            _values[index] = null;
        }

        public void Flush()
        {
            for (int i = 0; i < _values.Length; i++)
            {
                var value = _values[i];
                if (value != null) _unload(value.Index, value);
            }
        }

        public void Ensure(int x, int y, int z)
        {
            var idx = Flat(x, y, z);
            if (_values[idx] != null)
                return;

            _values[idx] = _load(new Index3(x,y,z));
        }

        public void Release(int x, int y, int z)
        {
            _values[Flat(x, y, z)] = null;
        }
    }
}