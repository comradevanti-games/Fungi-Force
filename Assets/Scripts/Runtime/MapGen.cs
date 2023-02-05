using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamShrimp.GGJ23
{
    public static class MapGen
    {
        private static readonly Map Empty =
            new Map(ImmutableDictionary<Vector2Int, Tile>.Empty,
                ImmutableDictionary<Vector2Int, Structure>.Empty);


        private static IEnumerable<Vector2Int> PositionsInMapOfSize(int size)
        {
            var diameter = size * 2 - 1;
            var minY = -(size - 1);
            var maxY = size - 1;
            for (var y = minY; y <= maxY; y++)
            {
                var isOffset = y % 2 != 0;
                var width = diameter - Mathf.Abs(y);
                var minX = isOffset ? (-(width - 1) / 2) - 1 : -width / 2;
                var maxX = isOffset ? (width - 1) / 2 : width / 2;
                for (var x = minX; x <= maxX; x++)
                    yield return new Vector2Int(x, y);
            }
        }

        private static Map PlaceTileAt(Map map, Vector2Int pos, Tile tile) =>
            new Map(map.TilesByPosition.Add(pos, tile),
                map.StructuresByPosition);

        private static Map PlaceStructureAt(
            Map map, Vector2Int pos, Structure structure) =>
            new Map(map.TilesByPosition,
                map.StructuresByPosition.Add(pos, structure));


        public static Map GenerateMap(GenerationParams genGenerationParams)
        {
            Random.InitState(genGenerationParams.Seed);
            
            var variantCount = genGenerationParams.TileType.Variants.Count();
            var variantIndex = Random.Range(0, variantCount);
            var tile = new Tile(genGenerationParams.TileType, variantIndex);

            Map GeneratePosition(Map map, Vector2Int pos) =>
                PlaceTileAt(map, pos, tile);

            var tilesOnly = PositionsInMapOfSize(genGenerationParams.Size)
                .Aggregate(Empty, GeneratePosition);

            var redTeamPos = new Vector2Int(-(genGenerationParams.Size - 1), 0);
            var withRedTeam = PlaceStructureAt(tilesOnly, redTeamPos,
                new Structure(genGenerationParams.HomeStructure, Team.Red));

            var blueTeamPos = new Vector2Int(genGenerationParams.Size - 1, 0);
            var withBlueTeam = PlaceStructureAt(withRedTeam, blueTeamPos,
                new Structure(genGenerationParams.HomeStructure, Team.Blue));

            Random.InitState(DateTime.Now.GetHashCode());
            
            return withBlueTeam;
        }

        public record GenerationParams(
            int Seed,
            int Size,
            TileType TileType,
            StructureType HomeStructure);

        public record Tile(
            TileType Type,
            int VariantIndex);

        public record Structure(StructureType Type, Team Team);

        public record Map(
            IImmutableDictionary<Vector2Int, Tile> TilesByPosition,
            IImmutableDictionary<Vector2Int, Structure> StructuresByPosition);
    }
}