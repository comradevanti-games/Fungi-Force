using System;
using TreeEditor;
using UnityEngine;

namespace TeamShrimp.GGJ23.Runtime
{
    public static class DistanceExtension
    {
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