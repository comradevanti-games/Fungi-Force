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
        private static readonly Map EmptyMap =
            new Map(ImmutableDictionary<Vector2Int, Tile>.Empty,
                ImmutableDictionary<Vector2Int, Structure>.Empty);


        private static T WithSeededRandom<T>(int seed, Func<T> f)
        {
            Random.InitState(seed);
            var result = f();
            Random.InitState(DateTime.Now.GetHashCode());
            return result;
        }

        private static IEnumerable<Vector2Int> PositionsInMapOfSize(int size)
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

        private static Map PlaceTileAt(Map map, Vector2Int pos, Tile tile) =>
            new Map(map.TilesByPosition.Add(pos, tile),
                map.StructuresByPosition);

        private static Map PlaceStructureAt(
            Map map, Vector2Int pos, Structure structure) =>
            new Map(map.TilesByPosition,
                map.StructuresByPosition.Add(pos, structure));

        private static Vector2Int HomePosition(Team team, int mapSize) =>
            new Vector2Int((mapSize - 2) * (team == Team.Red ? 1 : -1), 0);


        public static Map GenerateMap(GenerationParams genParams)
        {
            return WithSeededRandom(genParams.Seed, () =>
            {
                Map GenerateTileAt(Map map, Vector2Int pos)
                {
                    var variantCount =
                        genParams.TileType.Variants.Count();
                    var variantIndex = Random.Range(0, variantCount);
                    var tile = new Tile(genParams.TileType,
                        variantIndex);
                    return PlaceTileAt(map, pos, tile);
                }

                Map PlaceHome(Map map, Team team)
                {
                    var homePos = HomePosition(team, genParams.Size);
                    return PlaceStructureAt(map, homePos,
                        new Structure(genParams.HomeStructure, team));
                }

                return PositionsInMapOfSize(genParams.Size)
                    .Aggregate(EmptyMap, GenerateTileAt)
                    .Then(map => PlaceHome(map, Team.Red))
                    .Then(map => PlaceHome(map, Team.Blue));
            });
        }

        public record GenerationParams(
            int Seed,
            int Size,
            TileType TileType,
            StructureType HomeStructure,
            StructureType TreeStructure);

        public record Tile(
            TileType Type,
            int VariantIndex);

        public record Structure(StructureType Type, Team Team);

        public record Map(
            IImmutableDictionary<Vector2Int, Tile> TilesByPosition,
            IImmutableDictionary<Vector2Int, Structure> StructuresByPosition);
    }
}