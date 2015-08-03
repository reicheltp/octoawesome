using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Generators
{
    public class SphereGenerator : IStructureGenerator
    {
        private readonly int _radius;
        private readonly IBlockDefinition _blockDefinition;

        public SphereGenerator(int radius, IBlockDefinition blockDefinition, bool filled = true)
        {
            _radius = radius;
            _blockDefinition = blockDefinition;
        }

        public bool TryGenerate(IWorldManipulator manipulator, int planetId, Index3 startPosition)
        {
            GenerateCircle(_radius, manipulator, planetId, startPosition.X, startPosition.Y, startPosition.Z);

            for (int i = 1; i < _radius; i++)
            {
                GenerateCircle(_radius - i, manipulator, planetId, startPosition.X, startPosition.Y, startPosition.Z+ i);
                GenerateCircle(_radius - i, manipulator, planetId, startPosition.X, startPosition.Y, startPosition.Z -i);
            }

            return true;

        }

        private void GenerateCircle(int radius, IWorldManipulator manipulator, int planetId, int xS, int yS, int zS)
        {
            int x = radius;
            int y = 0;
            int decisionOver2 = 1 - x; // Decision criterion divided by 2 evaluated at x=r, y=0

            var setBlock = new Func<int, int, int, bool>((x1, y1, z1) => manipulator.TrySetBlock(planetId,
                new Index3(x1 + xS, y1 + yS, z1 + zS),
                _blockDefinition.GetInstance(OrientationFlags.None)));

            while (x >= y)
            {
                setBlock(x, y, 0);
                setBlock(y, x, 0);
                setBlock(-x, y, 0);
                setBlock(y, -x, 0);
                setBlock(x, -y, 0);
                setBlock(y, -x, 0);
                setBlock(-x, -y, 0);
                setBlock(-y, -x, 0);

                y++;
                if (decisionOver2 <= 0)
                {
                    decisionOver2 += 2*y + 1; // Change in decision criterion for y -> y+1
                }
                else
                {
                    x--;
                    decisionOver2 += 2*(y - x) + 1; // Change for y -> y+1, x -> x-1
                }
            }

        }
    }
}
