using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Vector2IntExt
    {
        public static Vector3Int To3(this Vector2Int v, int z = 0) =>
            new Vector3Int(v.x, v.y, z);
    }
}