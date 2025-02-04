using UnityEngine;

namespace Switchgrass.Utils
{
    public static class MathExtensions
    {
        public static Vector2 Flatten(this Vector3 v3)
        {
            return new Vector2(v3.x, v3.z);
        }
        
        // a.k.a. the 'Wedge' product
        public static float Cross(this Vector2 lhs, Vector2 rhs)
        {
            return lhs.x * rhs.y - lhs.y * rhs.x;
        }
    }
}