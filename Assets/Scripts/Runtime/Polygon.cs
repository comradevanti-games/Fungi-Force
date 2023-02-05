using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Polygon
    {
        public static float CalcArea(IReadOnlyCollection<Vector2Int> points)
        {
            var area = 0f;
            var j = points.Count - 1;

            for (var i = 0; i < points.Count; i++)
            {
                var iPoint = points.ElementAt(i);
                var jPoint = points.ElementAt(j);
                area += (jPoint.x + iPoint.x) * (jPoint.y - iPoint.y);
                j = i;
            }

            return area / 2f;
        }
    }
}