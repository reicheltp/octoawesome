using System.Collections.Generic;

namespace OctoAwesome.Runtime
{
    public interface IResourceManager : IWorldManipulator
    {
        IList<IChunk> ActiveChunks { get; }

        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
        IChunk GetChunk(int planetId, Index3 index);


        /// <summary>
        /// Persistiert den Planeten.
        /// </summary>
        void Save();
    }
}