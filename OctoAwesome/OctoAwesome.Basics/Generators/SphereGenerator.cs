using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Generators
{
    public class SphereGenerator : IStructureGenerator
    {
        private readonly double _radius;
        private readonly IBlockDefinition _blockDefinition;

        public SphereGenerator(double radius, IBlockDefinition blockDefinition, bool filled = true)
        {
            _radius = radius;
            _blockDefinition = blockDefinition;
        }

        public bool TryGenerate(IWorldManipulator manipulator, int planetId, Index3 startPosition)
        {
            GenerateCircle(_radius, manipulator, planetId, startPosition.X, startPosition.Y, startPosition.Z);

            return true;
        }

        private void GenerateCircle(double radius, IWorldManipulator manipulator, int planetId, int xS, int yS, int zS)
        {
//            int x = radius;
//            int y = 0;
//            int decisionOver2 = 1 - x; // Decision criterion divided by 2 evaluated at x=r, y=0

            var setBlock = new Func<int, int, int, bool>((x1, y1, z1) => manipulator.TrySetBlock(planetId,
                new Index3(x1 + xS, y1 + yS, z1 + zS),
                _blockDefinition.GetInstance(OrientationFlags.None)));

            var sqrt = Enumerable.Range(0, (int) (radius + 1)).Select(val => val*val).ToArray();
            
            var sqrtRadius = radius*radius;
            for (int x = (int) -radius; x <= radius; x ++)
                for (int y = (int) -radius; y <= radius; y++)
                    for (int z = (int) -radius; z <= radius; z++)
                    {
                        if (sqrt[Math.Abs(x)] + sqrt[Math.Abs(y)] + sqrt[Math.Abs(z)] <= sqrtRadius)
                            setBlock(x, y, z);
                    }
        }
    }
}
