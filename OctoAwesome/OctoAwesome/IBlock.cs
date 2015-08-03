using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IBlock
    {
        OrientationFlags Orientation { get; set; }

        BoundingBox[] GetCollisionBoxes();

        float? Intersect(Index3 boxPosition, Ray ray, out Axis? collisionAxis);

        float? Intersect(Index3 boxPosition, BoundingBox position, Vector3 move, out Axis? collisionAxis);

        /// <summary>
        /// Wird aufgerufen, wenn der Block an der übergebenen Position plaziert werden soll.
        /// </summary>
        /// <param name="manipulator"></param>
        /// <param name="planetId"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        bool CanPlaceAt(IWorldManipulator manipulator, int planetId, Index3 position);

        /// <summary>
        /// Wird aufgerufen, wenn der Block an der übergebenen Position plaziert werden soll.
        /// </summary>
        /// <param name="manipulator"></param>
        /// <param name="planetId"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        bool CanReplace(IWorldManipulator manipulator, int planetId, Index3 pos);
    }
}
