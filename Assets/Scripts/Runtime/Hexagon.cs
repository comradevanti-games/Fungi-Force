using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Hexagon
    {
        public static IEnumerable<Vector2Int> GeneratePositions(int size)
        {
            var diameter = size * 2 - 1;
            var minY = -(size - 1);
            var maxY = size - 1;
            for (var y = minY; y <= maxY; y++)
            {
                var isOffset = y % 2 != 0;
                var width = diameter - Mathf.Abs(y);
                var minX = isOffset ? -(width - 1) / 2 - 1 : -width / 2;
                var maxX = isOffset ? (width - 1) / 2 : width / 2;
                for (var x = minX; x <= maxX; x++)
                    yield return new Vector2Int(x, y);
            }
        }
    }
}