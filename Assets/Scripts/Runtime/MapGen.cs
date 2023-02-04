using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class MapGen
    {
        private static readonly Map Empty =
            new Map(ImmutableDictionary<Vector2Int, Tile>.Empty);


        private static IEnumerable<Vector2Int> PositionsInMapOfSize(int size)
        {
            for (var x = -(size - 1); x <= size - 1; x++)
                yield return new Vector2Int(x, 0);
        }

        private static Map PlaceTileAt(Map map, Vector2Int pos, Tile tile) =>
            new Map(map.TilesByPosition.Add(pos, tile));


        public static Map GenerateMap(GenerationParams genGenerationParams)
        {
            var tile = new Tile(genGenerationParams.TileType, 0);

            Map GenerateTile(Map map, Vector2Int pos) =>
                PlaceTileAt(map, pos, tile);

            return PositionsInMapOfSize(genGenerationParams.Size)
                .Aggregate(Empty, GenerateTile);
        }

        public record GenerationParams(
            int Size,
            TileType TileType);

        public record Tile(
            TileType Type,
            int VariantIndex);

        public record Map(
            IImmutableDictionary<Vector2Int, Tile> TilesByPosition);
    }
}