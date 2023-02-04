using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class MapGen
    {
        private static readonly Map Empty =
            new Map(ImmutableDictionary<Vector2Int, TileType>.Empty);


        private static IEnumerable<Vector2Int> PositionsInMapOfSize(int size)
        {
            for (var x = -(size - 1); x <= size - 1; x++)
                yield return new Vector2Int(x, 0);
        }

        private static Map PlaceTileAt(
            Map map, Vector2Int pos, TileType tile) =>
            new Map(map.TilesByPosition.Add(pos, tile));


        public static Map GenerateMap(GenerationParams genGenerationParams)
        {
            Map GenerateTile(Map map, Vector2Int pos) =>
                PlaceTileAt(map, pos, genGenerationParams.TileType);

            return PositionsInMapOfSize(genGenerationParams.Size)
                .Aggregate(Empty, GenerateTile);
        }

        public record GenerationParams(
            int Size,
            TileType TileType);

        public record Map(
            IImmutableDictionary<Vector2Int, TileType> TilesByPosition);
    }
}