using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ComradeVanti.CSharpTools;
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


        private static bool IsOnTeamSide(Vector2Int pos, Team team) =>
            team == Team.Red
                ? pos.x > 0
                : pos.x < 0;

        public static Map GenerateMap(GenerationParams genParams)
        {
            return WithSeededRandom(genParams.Seed, () =>
            {
                Vector2Int GenerateFreeTeamPosition(Map map, Team team) =>
                    map.TilesByPosition
                        .Keys
                        // Not taken
                        .Where(it =>
                            !map.StructuresByPosition.ContainsKey(it))
                        // Is on the correct side
                        .Where(it => IsOnTeamSide(it, team))
                        .ToArray()
                        // Take a random one
                        .Then(possible =>
                            possible[Random.Range(0, possible.Length)]);

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
                        new Structure(genParams.HomeStructure, Opt.Some(team)));
                }

                Map PlaceTeamTree(Map map, Team team)
                {
                    var treePos = GenerateFreeTeamPosition(map, team);
                    var structure = new Structure(genParams.TreeStructure,
                        Opt.None<Team>());
                    return PlaceStructureAt(map, treePos, structure);
                }

                Map PlaceTeamTrees(Map map, Team team, int count)
                {
                    for (var i = 0; i < count; i++)
                        map = PlaceTeamTree(map, team);

                    return map;
                }

                Map PlaceTrees(Map map)
                {
                    var treeCountPerTeam = Mathf.Max(genParams.Size / 4, 1);
                    return map
                        .Then(it =>
                            PlaceTeamTrees(it, Team.Red, treeCountPerTeam))
                        .Then(it =>
                            PlaceTeamTrees(it, Team.Blue, treeCountPerTeam));
                }

                return PositionsInMapOfSize(genParams.Size)
                    .Aggregate(EmptyMap, GenerateTileAt)
                    .Then(map => PlaceHome(map, Team.Red))
                    .Then(map => PlaceHome(map, Team.Blue))
                    .Then(PlaceTrees);
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

        public record Structure(StructureType Type, IOpt<Team> Team);

        public record Map(
            IImmutableDictionary<Vector2Int, Tile> TilesByPosition,
            IImmutableDictionary<Vector2Int, Structure> StructuresByPosition);
    }
}