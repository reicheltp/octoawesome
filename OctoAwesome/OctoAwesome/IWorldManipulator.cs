using System;
using System.Security.Cryptography.X509Certificates;

namespace OctoAwesome
{
    /// <summary>
    /// Basis interface um mit der Welt zu interagieren
    /// </summary>
    public interface IWorldManipulator
    {
        Random Random { get; }
        IUniverse GetUniverse(int id);
        IPlanet GetPlanet(int id);

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="planetId">Planeten Identifier</param>
        /// <param name="index">Block Index</param>
        /// <returns>Block oder null, falls dort kein Block existiert</returns>
        IBlock GetBlock(int planetId, Index3 index);

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="planetId">Planeten Identifier</param>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        void SetBlock(int planetId, Index3 index, IBlock block);

        bool IsReplaceable(int planetId, Index3 index);
    }
}