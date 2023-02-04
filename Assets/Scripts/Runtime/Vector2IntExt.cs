using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Vector2IntExt
    {
        public static Vector3Int To3Int(this Vector2Int v, int z = 0) =>
            new Vector3Int(v.x, v.y, z);

        public static Vector3 To3(this Vector2Int v, float z = 0) =>
            new Vector3(v.x, v.y, z);
    }
}