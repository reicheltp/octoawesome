namespace OctoAwesome.Basics
{
    public sealed class WaterBlock : Block
    {
        public override bool CanReplace(IWorldManipulator manipulator, int planetId, Index3 pos)
        {
            return true;
        }
    }
}
