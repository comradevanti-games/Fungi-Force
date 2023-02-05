using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Vector3IntExt
    {
        public static Vector2Int To2(this Vector3Int v) =>
            new Vector2Int(v.x, v.y);
    }
}