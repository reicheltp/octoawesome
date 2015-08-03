using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace OctoAwesome.Basics
{
    public class SaplingBlock : Block, IUpdateable
    {
        public override bool CanPlaceAt(IWorldManipulator manipulator, int planetId, Index3 position)
        {
            return manipulator.IsReplaceable(planetId, position) &&
                   this.IsValidUnderground(manipulator, planetId, position);
        }

        private bool IsValidUnderground(IWorldManipulator manipulator, int planetId, Index3 position)
        {
            var block = manipulator.GetBlock(planetId, new Index3(position.X, position.Y, position.Z -1 ));
            return block is GrassBlock || block is GroundBlock;
        }


        public void Update(GameTime gameTime, IWorldManipulator manipulator, int planetId, Index3 position)
        {
            CheckAndDropBlock(manipulator, planetId, position);
        }

        private void CheckAndDropBlock(IWorldManipulator manipulator, int planetId, Index3 position)
        {
            if (!IsValidUnderground(manipulator, planetId, position))
            {
                //TODO: Sapling als Item droppen, sobald Items implementiert sind. 
                manipulator.SetBlockToAir(planetId, position);
            }
        }
    }
}
