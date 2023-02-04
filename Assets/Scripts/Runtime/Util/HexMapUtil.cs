using System;
using UnityEngine;

namespace TeamShrimp.GGJ23.Runtime.Util
{
    public static class HexMapUtil
    {
        public static Vector3Int CubeToOffset(this Vector3Int cube)
        {
            var col = cube.x + (cube.z - (cube.z & 1)) / 2;
            var row = cube.z;
            return new Vector3Int(col, row, 0);
        }

        public static Vector3Int OffsetToCube(this Vector3Int offset)
        {
            var x = offset.x - (offset.y - (offset.y & 1)) / 2;
            var z = offset.y;
            var y = -x - z;
            if (x + y + z != 0)
            {
                Debug.LogError("the sum of cube vectors must always be zero!");
            }
            return new Vector3Int(x, y, z);
        }

        public static int CubeDistance(this Vector3Int self, Vector3Int other)
        {
            var vec = self-other;
            return Math.Max(Math.Max(Math.Abs(vec.x), Math.Abs(vec.y)), Math.Abs(vec.z));
        }
    }
}