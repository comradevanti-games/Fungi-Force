using System.Collections.Generic;
using System.Collections.Immutable;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public static class MapGen
    {
        private static readonly Map Empty =
            new Map(ImmutableDictionary<Vector2Int, TileType>.Empty);


        public static Map GenerateMap(GenerationParams genGenerationParams) =>
            Empty;

        public record GenerationParams(
            int Seed,
            int Size,
            TileType TileType);

        public record Map(
            IReadOnlyDictionary<Vector2Int, TileType> TilesByPosition);
    }
}