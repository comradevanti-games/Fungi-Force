using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Vector2IntExt
    {
        public static Vector3Int To3Int(this Vector2Int v, int z = 0) =>
            new Vector3Int(v.x, v.y, z);

        public static Vector3 To3(this Vector2Int v, float z = 0) =>
            new Vector3(v.x, v.y, z);
        
        public static float CellDistance(this Vector2Int first, Vector2Int other)
        {
            int dx = other.x - first.x; // signed deltas
            int dy = other.y - first.y;
            int x = Mathf.Abs(dx); // absolute deltas
            int y = Mathf.Abs(dy);
            // special case if we start on an odd row or if we move into negative x direction
            if ((dx < 0) ^ ((first.y & 1) == 1))
                x = Mathf.Max(0, x - (y + 1) / 2);
            else
                x = Mathf.Max(0, x - (y) / 2);
            return x + y;
        }
    }
}