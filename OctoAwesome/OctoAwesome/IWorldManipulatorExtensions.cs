using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;

namespace OctoAwesome
{
    // ReSharper disable once InconsistentNaming
    public static class IWorldManipulatorExtensions
    {
        public static void SetBlockToAir(this IWorldManipulator manipulator, int planetId, Index3 position)
        {
            manipulator.SetBlock(planetId, position, null);
        }

        public static bool TrySetBlock(this IWorldManipulator manipulator, int planetId, Index3 index, IBlock block)
        {
            if (!manipulator.IsReplaceable(planetId, index))
                return false;
            
            manipulator.SetBlock(planetId, index, block);
            return true;
        }
    }
}
