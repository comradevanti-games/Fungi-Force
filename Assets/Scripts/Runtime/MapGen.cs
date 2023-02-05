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

        private static Map PlaceTileAt(Map map, Vector2Int pos, Tile tile) =>
            map with {TilesByPosition = map.TilesByPosition.SetItem(pos, tile)};

        private static Map PlaceStructureAt(
            Map map, Vector2Int pos, Structure structure) =>
            map with
            {
                StructuresByPosition =
                map.StructuresByPosition.SetItem(pos, structure)
            };

        private static Vector2Int HomePosition(Team team, int mapSize) =>
            new Vector2Int((mapSize - 2) * (team == Team.Red ? 1 : -1), 0);


        private static bool IsOnTeamSide(Vector2Int pos, Team team) =>
            team == Team.Red
                ? pos.x > 0
                : pos.x < 0;

        public static Map GenerateMap(GenerationParams genParams)
        {
            var groundTileType = genParams.TileTypesByName["Forest"];

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
                        // Is not water
                        .Where(it =>
                            map.TilesByPosition[it].Type.name != "Water")
                        // Take a random one
                        .Random();

                TileType ChooseTileType() =>
                    genParams.TileTypesByName.Values.WeightedRandom(it =>
                        it.Weight);

                Tile GenerateTileOfType(TileType type)
                {
                    var variantCount = type.Variants.Count();
                    var variantIndex = Random.Range(0, variantCount);
                    return new Tile(type, variantIndex);
                }

                Map GenerateTileAt(Map map, Vector2Int pos)
                {
                    var tileType = ChooseTileType();
                    var tile = GenerateTileOfType(tileType);
                    return PlaceTileAt(map, pos, tile);
                }

                Map PlaceGroundAt(Map map, Vector2Int pos)
                {
                    var tile = GenerateTileOfType(groundTileType);
                    return PlaceTileAt(map, pos, tile);
                }

                Map PlaceHome(Map map, Team team)
                {
                    var homePos = HomePosition(team, genParams.Size);
                    var home = new Structure(genParams.HomeStructure,
                        Opt.Some(team));
                    return map
                        .Then(it => PlaceGroundAt(it, homePos))
                        .Then(it => PlaceStructureAt(it, homePos, home));
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

                return Hexagon.GeneratePositions(genParams.Size)
                    .Aggregate(EmptyMap, GenerateTileAt)
                    .Then(map => PlaceHome(map, Team.Red))
                    .Then(map => PlaceHome(map, Team.Blue))
                    .Then(PlaceTrees);
            });
        }

        public record GenerationParams(
            int Seed,
            int Size,
            IReadOnlyDictionary<string, TileType> TileTypesByName,
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