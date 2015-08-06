using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    public sealed class ChunkCache : IChunkCache
    {
        private readonly IChunk[] _values;
        private readonly IDictionary<Index3, int> _indices;

        private readonly Func<Index3, IChunk> _load;
        private readonly Action<Index3, IChunk> _unload;

        public ChunkCache(int size, Func<Index3, IChunk> load, Action<Index3, IChunk> unload)
        {
            _unload = unload;
            _load = load;
            Size = size;
            _values = new IChunk[size];
            _indices = new Dictionary<Index3, int>(size);
        }

        public int Size { get; private set; }

        public IEnumerable<IChunk> Values
        {
            get { return _values; }
        }

        public IChunk Get(Index3 key)
        {
            int index;
            if(_indices.TryGetValue(key, out index))
                return Get(index);

            return null;
        }

        public IChunk Get(int index)
        {
            return _values[index];
        }

        public void Ensure(Index3 key)
        {
            if (_indices.ContainsKey(key))
                return;

            for (int i = 0; i < _values.Length; i++)
            {
                if (_values[i] != null)
                    continue;

                _values[i] = _load(key);
                _indices[key] = i;
                break;
            }
        }

        public void Release(Index3 key)
        {
            int index;
            if (_indices.TryGetValue(key, out index))
            {
                Release(index);
                _indices.Remove(key);
            }
        }

        public void Release(int index)
        {
            _values[index] = null;
        }

        public void Flush()
        {
            var enumerator = _indices.GetEnumerator();
            do
            {
                var pair = enumerator.Current;
                _unload(pair.Key, _values[pair.Value]);
            } while (enumerator.MoveNext());
        }
    }
}