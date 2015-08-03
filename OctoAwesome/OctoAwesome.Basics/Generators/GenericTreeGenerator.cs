using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Generators
{
     

    public class GenericTreeGenerator : IStructureGenerator
    {
        public int MinHeight { get; set; }
        public int VariableHeight { get; set; }

        private readonly IBlockDefinition _trunk;
        private readonly IBlockDefinition _leave;

        public GenericTreeGenerator(IBlockDefinition trunk, IBlockDefinition leave)
        {
            _trunk = trunk;
            _leave = leave;

            MinHeight = 3;
            VariableHeight = 5;
        }

        public bool TryGenerate(IWorldManipulator manipulator, int planetId, Index3 startPosition)
        {
            var treeHeight = manipulator.Random.Next(VariableHeight) + MinHeight;

            for (int i = 0; i < treeHeight; i++)
            {
                var trunkPosition = new Index3(startPosition.X, startPosition.Y, startPosition.Z + i);
                if (i != 0 && !manipulator.IsReplaceable(planetId, trunkPosition))
                {
                    // Baum kann nur so hoch wachsen, wie Platz ist.
                    treeHeight = i + 1;
                    break;
                }

                manipulator.SetBlock(planetId, trunkPosition, _trunk.GetInstance(OrientationFlags.SideTop));
            }

            var treetopGenerator = new SphereGenerator(treeHeight / 2.0 - 0.5, _leave, true);

            treetopGenerator.TryGenerate(manipulator, planetId,
                new Index3(startPosition.X, startPosition.Y, startPosition.Z + treeHeight - 1));
            
            return true;
        }
    }

    public interface IStructureGenerator
    {
        bool TryGenerate(IWorldManipulator manipulator, int planetId, Index3 startPosition);
    }
}
