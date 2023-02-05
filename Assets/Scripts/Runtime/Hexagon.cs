using System.Collections.Generic;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class Hexagon
    {
        private static int WidthAt(int y, int size)
        {
            var diameter = size * 2 - 1;
            return diameter - Mathf.Abs(y);
        }

        private static (int, int) ColumnStartStop(int size)
        {
            var minY = -(size - 1);
            var maxY = size - 1;
            return (minY, maxY);
        }

        private static (int, int) RowStartStop(int y, int size)
        {
            var isOffset = y % 2 != 0;
            var width = WidthAt(y, size);
            var minX = isOffset ? -(width - 1) / 2 - 1 : -width / 2;
            var maxX = isOffset ? (width - 1) / 2 : width / 2;
            return (minX, maxX);
        }

        public static IEnumerable<Vector2Int> GeneratePositions(int size)
        {
            var (minY, maxY) = ColumnStartStop(size);
            for (var y = minY; y <= maxY; y++)
            {
                var (minX, maxX) = RowStartStop(y, size);
                for (var x = minX; x <= maxX; x++)
                    yield return new Vector2Int(x, y);
            }
        }

        public static bool Contains(int size, Vector2Int pos)
        {
            var (minY, maxY) = ColumnStartStop(size);
            if (pos.y < minY || pos.y > maxY) return false;
            var (minX, maxX) = RowStartStop(pos.y, size);
            return pos.x >= minX && pos.x <= maxX;
        }

        public static int TileCount(int size)
        {
            var sum = 0;
            var (minY, maxY) = ColumnStartStop(size);
            for (var y = minY; y <= maxY; y++)
            {
                var (minX, maxX) = RowStartStop(y, size);
                for (var x = minX; x <= maxX; x++)
                    sum++;
            }

            return sum;
        }
    }
}