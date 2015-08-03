using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace OctoAwesome
{
    public interface IGrowable
    {
        /// <summary>
        /// Gibt an, ob der Block unter den gegebenen Bedingungen wachsen kann.
        /// </summary>
        /// <param name="manipulator"></param>
        /// <param name="planetId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        bool CanGrow(IWorldManipulator manipulator, int planetId, Index3 position);

        /// <summary>
        /// Fordert den Block auf zu wachsen.
        /// </summary>
        /// <param name="manipulator"></param>
        /// <param name="planetId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        bool TryGrow(IWorldManipulator manipulator, int planetId, Index3 position);
    }
}
