using System.Collections.Generic;
using Unity.Physics;

namespace Common
{
    public struct DistanceHitDistanceComparer : IComparer<DistanceHit>
    {
        public int Compare(DistanceHit lhs, DistanceHit rhs)
        {
            return lhs.Distance.CompareTo(rhs.Distance);
        }
    }
}
