using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using OctoAwesome.Basics.Generators;

namespace OctoAwesome.Basics
{
    public class SaplingBlock : Block, IUpdateable, IGrowable
    {
        private const int TimeToGrow = 5000;

        // TODO: Speichern wir die grow time im Block oder wie bei MC in einer MetaData Tabelle wegen besserer Performance ?
        private double _growTime = 0d;

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

            _growTime += gameTime.ElapsedGameTime.Milliseconds * manipulator.Random.NextDouble();

            if (_growTime > TimeToGrow && CanGrow(manipulator, planetId, position))
                TryGrow(manipulator, planetId, position);
        }

        private void CheckAndDropBlock(IWorldManipulator manipulator, int planetId, Index3 position)
        {
            if (!IsValidUnderground(manipulator, planetId, position))
            {
                //TODO: Sapling als Item droppen, sobald Items implementiert sind. 
                manipulator.SetBlockToAir(planetId, position);
            }
        }

        public bool CanGrow(IWorldManipulator manipulator, int planetId, Index3 position)
        {
            // Der kleinst mögliche Baum ist 3 Blöcke hoch.
            return manipulator.IsReplaceable(planetId, new Index3(position.X, position.Y, position.Z + 1))
                   && manipulator.IsReplaceable(planetId, new Index3(position.X, position.Y, position.Z + 2));
        }

        public bool TryGrow(IWorldManipulator manipulator, int planetId, Index3 position)
        {
            //TODO: Durch Block manager ersetzen.
            var treeGenerator = new GenericTreeGenerator(new WoodBlockDefinition(), new LeaveBlockDefinition());

            return treeGenerator.TryGenerate(manipulator, planetId, position);
        }
    }
}
