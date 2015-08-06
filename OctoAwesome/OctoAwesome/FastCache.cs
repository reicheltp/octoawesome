using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public sealed class FastCache<TKey, TValue> : ICache<TKey, TValue> where TValue : class
    {
        private readonly TValue[] _values;
        private readonly IDictionary<TKey, int> _indices;

        private int _stored = 0;
        private readonly Func<TKey, TValue> _load;
        private readonly Action<TKey, TValue> _unload;

        public FastCache(int size, Func<TKey, TValue> load, Action<TKey, TValue> unload)
        {
            _unload = unload;
            _load = load;
            Size = size;
            _values = new TValue[size];
            _indices = new Dictionary<TKey, int>(size);
        }

        public int Size { get; private set; }

        public IEnumerable<TValue> Values
        {
            get { return _values; }
        }

        public TValue Get(TKey key)
        {
            if(_indices.ContainsKey(key))
                return Get(_indices[key]);

            return null;

            if (_stored < Size)
            {
                _indices.Add(key, _stored);

                var value = _values[_stored] = _load(key);
                _stored++;
                return value;
            }
            else
            {
                var first = _indices.First();
                _indices.Remove(first);
                _unload(first.Key, _values[first.Value]);

                _indices.Add(key, first.Value);
                return _values[first.Value] = _load(key);
            }

        }

        public TValue Get(int index)
        {
            return _values[index];
        }

        public void Flush()
        {
            var enumerator = _indices.GetEnumerator();
            for (int i = 0; i < _stored; i++)
            {
                var pair = enumerator.Current;
                _unload(pair.Key, _values[pair.Value]);

                enumerator.MoveNext();
            }
        }
    }
}