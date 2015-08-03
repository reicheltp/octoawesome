using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Generators
{
    public class SphereGenerator : IStructureGenerator
    {
        private double _radius;

        public SphereGenerator(double radius)
        {
            _radius = radius;
        }

        public bool TryGenerate(IWorldManipulator manipulator, int planetId, Index3 startPosition)
        {
            return true;
        }
    }
}
