using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

namespace TeamShrimp.GGJ23.Runtime.Util
{
    public static class HexMapUtil
    {
        public static Vector3Int CubeToOffset(this Vector3Int cube)
        {
            var col = cube.x + (cube.y - (cube.y & 1)) / 2;
            var row = cube.y;
            return new Vector3Int(col, row, 0);
        }

        public static Vector3Int OffsetToCube(this Vector3Int offset)
        {
            var x = offset.x - (offset.y - (offset.y & 1)) / 2;
            var y = offset.y;
            var z = -x - y;
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

        public static Vector3Int CubeRound(this Vector3 fCube)
        {
            var x = (int) Math.Round(fCube.x, 0);
            var y = (int) Math.Round(fCube.y, 0);
            var z = (int) Math.Round(fCube.z, 0);

            var xDiff = Math.Abs(x - fCube.x);
            var yDiff = Math.Abs(y - fCube.y);
            var zDiff = Math.Abs(z - fCube.z);

            if (xDiff > yDiff && xDiff > zDiff)
            {
                x = -y - z;
            }
            else if (yDiff > zDiff)
            {
                y = -x - z;
            }
            else
            {
                z = -x - y;
            }

            return new Vector3Int(x, y, z);
        }

        public static Vector3Int[] CubeNeighbours(this Vector3Int cubeStart)
        {
            Vector3Int[] neighbours = new[]
            {
                cubeStart + new Vector3Int(1, 0, -1),
                cubeStart + new Vector3Int(-1, 0, 1),
                cubeStart + new Vector3Int(1, -1, 0),
                cubeStart + new Vector3Int(-1, 1, 0),
                cubeStart + new Vector3Int(0, -1, 1),
                cubeStart + new Vector3Int(0, 1, -1)
            };

            return neighbours;
        }
        
        public static List<Vector3> AsVec3List(this List<Vector3Int> vector3Ints)
        {
            List<Vector3> vector3s = new List<Vector3>();
            foreach (Vector3Int vector3Int in vector3Ints)
            {
                vector3s.Add(new Vector3(vector3Int.x, vector3Int.y, vector3Int.z));
            }

            return vector3s;
        }
    }
}