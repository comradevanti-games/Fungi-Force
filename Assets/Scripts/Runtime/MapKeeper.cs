using System;
using System.Collections.Generic;
using System.Linq;
using ComradeVanti.CSharpTools;
using Dev.ComradeVanti;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeamShrimp.GGJ23
{
    public class MapKeeper : MonoBehaviour
    {
        private const string TileTypeResourcePath = "TileTypes";
        private const string StructureTypeResourcePath = "StructureTypes";

        [SerializeField] private Tilemap groundTilemap;

        private readonly Dictionary<Vector2Int, ShroomBase> shroomsByPosition =
            new Dictionary<Vector2Int, ShroomBase>();

        private IReadOnlyDictionary<string, StructureType> structureTypesByName;
        private IReadOnlyDictionary<string, TileType> tileTypesByName;

        public List<ShroomBase> AllShrooms =>
            new List<ShroomBase>(shroomsByPosition.Values);

        [SerializeField] private bool debug;

        private void Awake()
        {
            if (debug)
            {
                ShroomBase shroom = GameObject.FindWithTag("Shroom").GetComponent<ShroomBase>();
                shroomsByPosition.Add(shroom.ShroomPosition, shroom);
                return;
            }
            LoadTileTypes();
            LoadStructureTypes();
        }

        private void Start()
        {
            InstantiateGameMap();
        }

        public IOpt<ShroomBase> TryFindShroom(Vector2Int pos) =>
            shroomsByPosition.TryGet(pos);

        public void AddShroom(ShroomBase shroom)
        {
            var pos = shroom.ShroomPosition;
            shroomsByPosition.Add(pos, shroom);
        }

        public bool CanPlace(ShroomType type, Vector2Int pos) =>
            TryFindShroom(pos).IsNone();

        public Vector3 SnapToGridPos(Vector3 worldPos)
        {
            Vector3Int cell = groundTilemap.WorldToCell(worldPos);
            cell.z = 1;
            return groundTilemap.CellToWorld(cell);
        }

        public Vector3Int WorldToGridPos(Vector3 worldPos)
        {
            return groundTilemap.WorldToCell(worldPos);
        }

        public Vector3 GridToWorldPos(Vector3Int gridPos)
        {
            return groundTilemap.CellToWorld(gridPos);
        }

        public Vector3 GridToWorldPos(Vector2Int gridPos)
        {
            Vector3Int pos = new Vector3Int(gridPos.x, gridPos.y, (int) groundTilemap.transform.position.z);
            return GridToWorldPos(pos);
        }

        private void InstantiateGameMap()
        {
            InstantiateMapWith(Blackboard.Game.MapSize);
        }

        private void InstantiateMapWith(int size)
        {
            var defaultTile = tileTypesByName.Values.First();
            var homeStructure = structureTypesByName["Home"];
            var genParams =
                new MapGen.GenerationParams(size, defaultTile, homeStructure);
            var map = MapGen.GenerateMap(genParams);
            InstantiateMap(map);
        }

        private void InstantiateMap(MapGen.Map map)
        {
            foreach (var (pos, tile) in map.TilesByPosition)
            {
                var variant = tile.Type.Variants.ElementAt(tile.VariantIndex);
                groundTilemap.SetTile(pos.To3Int(), variant);
            }

            foreach (var (pos, structure) in map.StructuresByPosition)
            {
                var go = Instantiate(structure.Type.Prefab, pos.To3(),
                    Quaternion.identity);
                go.TryGetComponent<ShroomBase>()
                    .Iter(shroom =>
                    {
                        shroom.WorldPosition = pos.To3();
                        AddShroom(shroom);
                    });
            }
        }

        private void LoadTileTypes()
        {
            tileTypesByName = Resources.LoadAll<TileType>(TileTypeResourcePath)
                .ToDictionary(t => t.name, t => t);
            if (tileTypesByName.Count == 0)
                Debug.LogWarning("No tile-types found in resources!");
        }

        private void LoadStructureTypes()
        {
            structureTypesByName = Resources
                .LoadAll<StructureType>(StructureTypeResourcePath)
                .ToDictionary(t => t.name, t => t);
            if (structureTypesByName.Count == 0)
                Debug.LogWarning("No structure-types found in resources!");
        }
    }
}