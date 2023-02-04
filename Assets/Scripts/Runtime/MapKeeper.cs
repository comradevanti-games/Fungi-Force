using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeamShrimp.GGJ23
{
    public class MapKeeper : MonoBehaviour
    {
        private const string TileTileResourcePath = "TileTypes";

        [SerializeField] private Tilemap groundTilemap;

        private IReadOnlyDictionary<string, TileType> tileTypesByName;


        private void Awake()
        {
            LoadTileTypes();
        }

        private void Start()
        {
            InstantiateMapWith(10);
        }

        private void InstantiateMapWith(int size)
        {
            var defaultTile = tileTypesByName.Values.First();
            var genParams = new MapGen.GenerationParams(size, defaultTile);
            var map = MapGen.GenerateMap(genParams);
            InstantiateMap(map);
        }

        private void InstantiateMap(MapGen.Map map)
        {
            foreach (var (pos, tile) in map.TilesByPosition)
            {
                var variant = tile.Type.Variants.ElementAt(tile.VariantIndex);
                groundTilemap.SetTile(pos.To3(), variant);
            }
        }

        private void LoadTileTypes()
        {
            tileTypesByName = Resources.LoadAll<TileType>(TileTileResourcePath)
                .ToDictionary(t => t.name, t => t);
            if (tileTypesByName.Count == 0)
                Debug.LogWarning("No tile-types found in resources!");
        }
    }
}