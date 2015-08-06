using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Threading.Tasks;

namespace OctoAwesome
{
    public class ChunkLoader : IChunkLoader
    {
        private Index3 _center;
        private readonly int _range;

        private readonly IChunkCache _cache;

        private Task _loadingTask;

        public ChunkLoader(IChunkCache cache, Index3 center, int range)
        {
            _cache = cache;
            _center = center;
            _range = range;
        }

        public void Update(int i, int j, int k)
        {
            _center = new Index3(_center.X + i, _center.Y + j, _center.Z + k);

            if (_loadingTask != null && !_loadingTask.IsCompleted)
                _loadingTask = _loadingTask.ContinueWith(_ => Reload());

            _loadingTask = Task.Factory.StartNew(Reload);
        }

        private void Reload()
        {
            _cache.Ensure(_center);

            for (int range = 1; range < _range; range ++)
            for (int i = 0; i <= range; i++)
            for (int j = 0; j <= range; j++)
                // Ignore Z
            //for (int k = -_range; k < _range; k++)
            {
                if(i != range && j != range)
                    continue;

//                _cache.Ensure(new Index3(_center.X + i, _center.Y + j, _center.Z + k));
                _cache.Ensure(new Index3(_center.X + i, _center.Y + j, _center.Z));
                _cache.Ensure(new Index3(_center.X + i, _center.Y - j, _center.Z));
                _cache.Ensure(new Index3(_center.X - i, _center.Y + j, _center.Z));
                _cache.Ensure(new Index3(_center.X - i, _center.Y - j, _center.Z));

            }
        }


        public void Dispose()
        {
            for (int i = -_range; i < _range; i++)
            for (int j = -_range; j < _range; j++)
            for (int k = -_range; k < _range; k++)
            {
                _cache.Release(new Index3(_center.X + i, _center.Y + j, _center.Z +k));
            }
        }
    }

    public interface IChunkLoader : IDisposable
    {
        void Update(int i, int j, int k);
    }
}
