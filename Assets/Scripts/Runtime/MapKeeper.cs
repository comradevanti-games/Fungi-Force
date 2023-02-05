using System;
using System.Collections.Generic;
using System.Linq;
using ComradeVanti.CSharpTools;
using Dev.ComradeVanti;
using TeamShrimp.GGJ23.Runtime.Util;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TeamShrimp.GGJ23
{
    public class MapKeeper : MonoBehaviour
    {
        private const string TileTypeResourcePath = "TileTypes";
        private const string StructureTypeResourcePath = "StructureTypes";

        [SerializeField] private Tilemap groundTilemap;
        [SerializeField] private bool debug;

        private readonly Dictionary<Vector2Int, ShroomBase> shroomsByPosition =
            new Dictionary<Vector2Int, ShroomBase>();

        private readonly Dictionary<Vector2Int, MapGen.Structure> structuresByPosition =
            new Dictionary<Vector2Int, MapGen.Structure>();

        private readonly Dictionary<Vector2Int, GameObject> gameObjectsByPosition =
            new Dictionary<Vector2Int, GameObject>();

        private IReadOnlyDictionary<string, StructureType> structureTypesByName;
        private IReadOnlyDictionary<string, TileType> tileTypesByName;

        public IEnumerable<ShroomBase> AllShrooms => shroomsByPosition.Values;

        private void Awake()
        {
            if (debug)
            {
                var shroom = GameObject.FindWithTag("Shroom")
                    .GetComponent<ShroomBase>();
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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftControl) ||
                Input.GetMouseButtonUp(2))
            {
                // --Debug.Log("Strategic View");
                var color = groundTilemap.color;
                color.a = color.a < 0.95f ? 0.95f : 0.25f;
                groundTilemap.color = color;
            }
        }

        public IOpt<ShroomBase> TryFindShroom(Vector2Int pos) =>
            shroomsByPosition.TryGet(pos);

        public IOpt<MapGen.Structure> TryFindStructure(Vector2Int pos) => structuresByPosition.TryGet(pos);

        public IOpt<GameObject> TryFindObject(Vector2Int pos) => gameObjectsByPosition.TryGet(pos);

        public List<ShroomBase> FindAllShroomsOfTypeAndOwner(StructureType type, Team owner)
        {
            return AllShrooms.Where(shroom => shroom.ShroomType == type && shroom.Owner == owner).ToList();
        }

        public void AddShroom(ShroomBase shroom)
        {
            var pos = WorldToGridPos(shroom.transform.position);
            shroomsByPosition.Add((Vector2Int) pos, shroom);
            // gameObjectsByPosition.Add((Vector2Int) pos, shroom.gameObject);
            groundTilemap.SetTileFlags(pos, TileFlags.None);
            groundTilemap.SetColor(pos, shroom.Owner.ToColor());
        }

        public void DeleteObject(Vector2Int pos)
        {
            GameObject toDelete = null;
            TryFindObject(pos).Iter(obj => toDelete = obj);
            if (!toDelete)
                return;
            if (gameObjectsByPosition.Remove(pos))
                Destroy(toDelete);
        }

        public bool CanPlace(StructureType type, Vector2Int pos) =>
            TryFindShroom(pos).IsNone();

        public Vector3 SnapToGridPos(Vector3 worldPos)
        {
            var cell = groundTilemap.WorldToCell(worldPos);
            cell.z = 1;
            return groundTilemap.CellToWorld(cell);
        }

        public Vector3Int WorldToGridPos(Vector2 worldPos) =>
            groundTilemap.WorldToCell(worldPos);

        public Vector3 GridToWorldPos(Vector3Int gridPos) =>
            groundTilemap.CellToWorld(gridPos);

        public Vector3 GridToWorldPos(Vector2Int gridPos)
        {
            var pos = new Vector3Int(gridPos.x, gridPos.y,
                (int) groundTilemap.transform.position.z);
            return GridToWorldPos(pos);
        }

        public StructureType GetStructureType(string name) =>
            structureTypesByName[name];

        public List<Vector3Int> GetLerpPathCubed(
            Vector3Int startPosition, Vector3Int endPosition)
        {
            var startCube = startPosition.OffsetToCube();
            var endCube = endPosition.OffsetToCube();
            var cubeDistance = startCube.CubeDistance(endCube);
            var results = new List<Vector3Int>();

            for (var i = 0; i <= cubeDistance; i++)
            {
                var x = Mathf.Lerp(startCube.x, endCube.x,
                    1.0f / cubeDistance * i);
                var y = Mathf.Lerp(startCube.y, endCube.y,
                    1.0f / cubeDistance * i);
                var z = Mathf.Lerp(startCube.z, endCube.z,
                    1.0f / cubeDistance * i);

                var cube = new Vector3(x, y, z).CubeRound();
                results.Add(cube);
            }

            return results;
        }

        public List<ShroomBase> FindOwnedShroomsInRange(
            ShroomBase root, float distance)
        {
            Debug.Log("Finding owned Shrooms in Range for " + root);
            var result = new List<ShroomBase>();
            var cubedStart = root.ShroomPosition.To3Int().OffsetToCube();
            var cubesToCheck = new Stack<Vector3Int>();
            var checkedCubes = new List<Vector3Int>();
            checkedCubes.Add(cubedStart);
            cubedStart.CubeNeighbours().ToList()
                .ForEach(cube => cubesToCheck.Push(cube));

            Vector3Int cubeToCheck;
            while (cubesToCheck.TryPop(out cubeToCheck))
            {
                if (cubeToCheck.CubeDistance(cubedStart) > distance)
                    continue;
                checkedCubes.Add(cubeToCheck);
                cubeToCheck.CubeNeighbours().ToList().ForEach(cube =>
                {
                    if (!checkedCubes.Contains(cube))
                        cubesToCheck.Push(cube);
                });

                cubeToCheck = cubeToCheck.CubeToOffset();
                var shroom = TryFindShroom((Vector2Int) cubeToCheck);
                shroom.Iter(value =>
                {
                    Debug.Log("Found " + value);
                    if (value.Owner == root.Owner)
                        result.Add(value);
                });
            }

            return result;
        }

        public void ClaimArea(ShroomBase root)
        {
            
        }

        public void RemoveAtPosition(Vector2Int pos)
        {
            shroomsByPosition.Remove(pos);
            //Debug.Log("REMOVING SHROOM AT POSITION " + pos + " FROM MAP " + String.Join(",",
            //    shroomsByPosition.Values));
        }

        private void InstantiateGameMap()
        {
            InstantiateMapWith(
                Blackboard.Game.MapSeed,
                Blackboard.Game.MapSize);
        }

        private void InstantiateMapWith(int seed, int size)
        {
            var genParams =
                new MapGen.GenerationParams(seed, size,
                    tileTypesByName,
                    structureTypesByName["Home"],
                    structureTypesByName["Tree"]);
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
                var worldPos = groundTilemap.CellToWorld(pos.To3Int());
                var go = Instantiate(structure.Type.Prefab, worldPos,
                    structure.Type.Prefab.transform.rotation);
                go.TryGetComponent<ShroomBase>()
                    .Iter(shroom =>
                    {
                        structure.Team.Iter(it => shroom.Owner = it);
                        AddShroom(shroom);
                    });
                structuresByPosition.Add(pos, structure);
                gameObjectsByPosition.Add(pos, go);
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