using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ResourceManager : IResourceManager
    {
        public static int CacheSize = 10000;

        private readonly bool _disablePersistence = false;
        private readonly IMapGenerator _mapGenerator = null;
        private readonly IChunkPersistence _chunkPersistence = null;

        public Random Random { get; private set; }

        /// <summary>
        /// Planet Cache.
        /// </summary>
        private Cache<int, IPlanet> planetCache;

        /// <summary>
        /// Chunk Cache.
        /// </summary>
        private Cache<PlanetIndex3, IChunk> chunkCache;

        private IUniverse universeCache;

        #region Singleton

        private static IResourceManager instance = null;
        public static IResourceManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new ResourceManager();
                return instance;
            }
        }

        #endregion

        public IList<IChunk> ActiveChunks
        {
            get { return chunkCache.Values; }
        }

        private ResourceManager()
        {
            _mapGenerator = MapGeneratorManager.GetMapGenerators().First();
            _chunkPersistence = new ChunkDiskPersistence();

            Random = new Random();

            planetCache = new Cache<int, IPlanet>(1, loadPlanet, savePlanet);
            chunkCache = new Cache<PlanetIndex3, IChunk>(CacheSize, loadChunk, saveChunk);

            bool.TryParse(ConfigurationManager.AppSettings["DisablePersistence"], out _disablePersistence); 
        }

        public IUniverse GetUniverse(int id)
        {
            if (universeCache == null)
                universeCache = _mapGenerator.GenerateUniverse(id);

            return universeCache;
        }

        public IPlanet GetPlanet(int id)
        {
            return planetCache.Get(id);
        }

        /// <summary>
        /// Liefert den Chunk an der angegebenen Chunk-Koordinate zurück.
        /// </summary>
        /// <param name="index">Chunk Index</param>
        /// <returns>Instanz des Chunks</returns>
        public IChunk GetChunk(int planetId, Index3 index)
        {
            IPlanet planet = GetPlanet(planetId);

            if (index.X < 0 || index.X >= planet.Size.X ||
                index.Y < 0 || index.Y >= planet.Size.Y ||
                index.Z < 0 || index.Z >= planet.Size.Z)
                return null;

            return chunkCache.Get(new PlanetIndex3(planetId, index));
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Block oder null, falls dort kein Block existiert</returns>
        public IBlock GetBlock(int planetId, Index3 index)
        {
            IPlanet planet = GetPlanet(planetId);

            index.NormalizeXY(new Index2(
                planet.Size.X * Chunk.CHUNKSIZE_X,
                planet.Size.Y * Chunk.CHUNKSIZE_Y));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);

            // Betroffener Chunk ermitteln
            Index3 chunkIndex = coordinate.ChunkIndex;
            if (chunkIndex.X < 0 || chunkIndex.X >= planet.Size.X ||
                chunkIndex.Y < 0 || chunkIndex.Y >= planet.Size.Y ||
                chunkIndex.Z < 0 || chunkIndex.Z >= planet.Size.Z)
                return null;
            IChunk chunk = chunkCache.Get(new PlanetIndex3(planetId, chunkIndex));
            if (chunk == null)
                return null;

            return chunk.GetBlock(coordinate.LocalBlockIndex);
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        public void SetBlock(int planetId, Index3 index, IBlock block)
        {
            IPlanet planet = GetPlanet(planetId);

            index.NormalizeXYZ(new Index3(
                planet.Size.X * Chunk.CHUNKSIZE_X,
                planet.Size.Y * Chunk.CHUNKSIZE_Y,
                planet.Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = GetChunk(planetId, coordinate.ChunkIndex);
            chunk.SetBlock(coordinate.LocalBlockIndex, block);
        }

        public bool IsReplaceable(int planetId, Index3 pos)
        {
            var block = GetBlock(planetId, pos);
            return block == null || block.CanReplace(this, planetId, pos);
        }
        
        private IPlanet loadPlanet(int index)
        {
            IUniverse universe = GetUniverse(0);

            return _mapGenerator.GeneratePlanet(universe.Id, index);
        }

        private void savePlanet(int index, IPlanet value)
        {
        }

        private IChunk loadChunk(PlanetIndex3 index)
        {
            IUniverse universe = GetUniverse(0);
            IPlanet planet = GetPlanet(index.Planet);

            // Load from disk
            IChunk first = _chunkPersistence.Load(universe.Id, index.Planet, index.ChunkIndex);
            if (first != null)
                return first;

            IChunk[] result = _mapGenerator.GenerateChunk(planet, new Index2(index.ChunkIndex.X, index.ChunkIndex.Y));
            if (result != null && result.Length > index.ChunkIndex.Z)
            {
                result[index.ChunkIndex.Z].ChangeCounter = 0;
                return result[index.ChunkIndex.Z];
            }

            return null;
        }

        private void saveChunk(PlanetIndex3 index, IChunk value)
        {
            IUniverse universe = GetUniverse(0);

            if (!_disablePersistence && value.ChangeCounter > 0)
            {
                _chunkPersistence.Save(universe.Id, index.Planet, value);
                value.ChangeCounter = 0;
            }
        }

        /// <summary>
        /// Persistiert den Planeten.
        /// </summary>
        public void Save()
        {
            chunkCache.Flush();
        }
    }
}
