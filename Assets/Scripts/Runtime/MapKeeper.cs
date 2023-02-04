using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeamShrimp.GGJ23
{
    public class MapKeeper : MonoBehaviour
    {
        private const string TileTypeResourcePath = "TileTypes";
        private const string StructureTypeResourcePath = "StructureTypes";

        [SerializeField] private Tilemap groundTilemap;
        private IReadOnlyDictionary<string, StructureType> structureTypesByName;

        private IReadOnlyDictionary<string, TileType> tileTypesByName;


        private void Awake()
        {
            LoadTileTypes();
            LoadStructureTypes();
        }

        private void Start()
        {
            InstantiateMapWith(10);
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
                Instantiate(structure.Type.Prefab, pos.To3(),
                    Quaternion.identity);
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