using System.Collections.Generic;
using System.Linq;
using ComradeVanti.CSharpTools;
using TeamShrimp.GGJ23.Networking;
using UnityEngine;
using UnityEngine.Events;

namespace TeamShrimp.GGJ23
{
    public class MushroomManager : MonoBehaviour
    {
        public static MushroomManager Instance;
        public bool debug;

        [SerializeField] private float cellLength;

        [SerializeField] private GameObject initialPrefab;

        [SerializeField] private GameManager gameManager;
        [SerializeField] private GhostShroom ghostShroom;
        [SerializeField] private NetworkManager networkManager;

        [SerializeField] private float maxDistanceAllowed;

        [SerializeField] private MapKeeper map;
        [SerializeField] private UnityEvent onShroomPlaced;

        [SerializeField] private ShroomConnection connectionPrefab;

        private readonly List<ShroomConnection> _shroomConnections =
            new List<ShroomConnection>();

        private Camera _activeCamera;

        // private List<ShroomBase> _shroomsInGame;

        private ShroomBase _selectedShroom;

        private GameObject _selectedShroomPrefab;

        public GameObject SelectedShroomPrefab
        {
            set => _selectedShroomPrefab = value;
        }

        private void Awake()
        {
            Instance = this;
        }


        // Start is called before the first frame update
        private void Start()
        {
            Instance = this;
            var existingShrooms = map.AllShrooms;

            foreach (var shroom in existingShrooms)
                shroom.Initialize(shroom.Owner);

            _selectedShroomPrefab = initialPrefab;
            _activeCamera = Camera.main;
        }

        // Update is called once per frame
        private void Update()
        {
            if (!gameManager.IsMyTurn) return;

            var mousePosition = Input.mousePosition;
            // mousePosition.z = Mathf.Abs(CameraManager.Instance.MainCamera.transform.position.z) + 1;
            mousePosition =
                CameraManager.Instance.MainCamera.ScreenToWorldPoint(
                    mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (_selectedShroom == null)
                {
                    var gridPosition = map.WorldToGridPos(mousePosition);
                    var shroomAsPos =
                        GetMushroomAtPosition((Vector2Int) gridPosition);
                    if (shroomAsPos && shroomAsPos.Owner == gameManager.MyTeam)
                        _selectedShroom = shroomAsPos;
                    if (debug)
                        Debug.Log("Found Shroom: " + _selectedShroom);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (_selectedShroom != null &&
                    !ghostShroom.gameObject.activeSelf)
                {
                    ghostShroom.gameObject.SetActive(true);
                    ghostShroom.Parent = _selectedShroom;
                }

                if (ghostShroom.gameObject.activeSelf)
                {
                    var gridPosition = map.SnapToGridPos(mousePosition);
                    ghostShroom.WorldPosition = gridPosition;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_selectedShroom != null)
                {
                    var gridPosition = map.WorldToGridPos(mousePosition);
                    ShroomBase toAdd = null;
                    toAdd = PlaceMushroom((Vector2Int) gridPosition);
                    if (debug)
                        Debug.Log("I added " + toAdd);
                    if (toAdd)
                        map.AddShroom(toAdd);
                    ghostShroom.gameObject.SetActive(false);
                    _selectedShroom = null;
                }
            }
        }

        public ShroomBase TryGetShroomAtPosition(Vector2Int gridPosition)
        {
            if (debug)
                Debug.Log("TryGetShroomAtPosition(" + gridPosition + ")");
            // TODO Change to Map request
            var hit =
                Physics2D.CircleCast(gridPosition, cellLength, Vector2.zero);
            if (hit && hit.collider.CompareTag("Shroom"))
            {
                if (debug)
                    Debug.Log("Raycast Hit: " + hit.collider.tag);
                return GetMushroomAtPosition(Vector2Int.CeilToInt(hit.point));
            }

            return null;
        }

        public long GenerateUniqueId()
        {
            if (!map.AllShrooms.Any()) return 1;

            return map.AllShrooms.Max(shroom => shroom.ShroomId) + 1;
        }


        public List<ShroomConnection> FindAllShroomConnectionsInvolving(
            ShroomBase shroomBase)
        {
            return _shroomConnections.FindAll(connection =>
                shroomBase.ShroomId == connection.StartShroom.ShroomId ||
                shroomBase.ShroomId == connection.EndShroom.ShroomId);
        }

        public void KillConnection(ShroomConnection mushroomConnection)
        {
            _shroomConnections.Remove(mushroomConnection);
            Destroy(mushroomConnection);
        }

        public int RemoveAllShroomConnectionsInvolving(ShroomBase shroomBase)
        {
            var i = 0;
            foreach (var shroomConnection in _shroomConnections.Where(
                         connection =>
                             shroomBase.ShroomId ==
                             connection.StartShroom.ShroomId ||
                             shroomBase.ShroomId ==
                             connection.EndShroom.ShroomId))
            {
                KillConnection(shroomConnection);
                i++;
            }

            return i;
        }

        public void RemoveMushroom(ShroomBase shroomBase)
        {
            map.RemoveAtPosition(shroomBase.ShroomPosition);
        }

        public ShroomBase PlaceMushroom(Vector2Int gridPosition)
        {
            var prefabBase =
                _selectedShroomPrefab.GetComponentInChildren<ShroomBase>();
            if (debug)
                Debug.Log("Placing Shroom at: " + gridPosition +
                          ", selected Shroom is: " + _selectedShroom);
            if (_selectedShroom == null || !PositionsInRange(
                                            _selectedShroom.ShroomPosition,
                                            gridPosition)
                                        || !map.CanPlace(prefabBase.ShroomType,
                                            gridPosition))
                return null;

            if (!prefabBase.Pay())
                return null;
            if (debug)
                Debug.Log("Is free");
            var placedShroom = Instantiate(_selectedShroomPrefab)
                .GetComponentInChildren<ShroomBase>();
            placedShroom.WorldPosition = map.GridToWorldPos(gridPosition);

            Debug.Log(placedShroom);

            CheckForConnections(placedShroom);

            Debug.Log("Checked for Connections");

            placedShroom.Initialize(gameManager.CurrentTeam);
            onShroomPlaced.Invoke();

            Debug.Log("Invoked Event");

            var placeCommand = new PlaceCommand(placedShroom.ShroomType.name,
                placedShroom.ShroomId, placedShroom.ShroomPosition,
                _selectedShroom.ShroomPosition);
            networkManager.SendCommand(placeCommand);
            if (debug)
            {
                Debug.Log("SENDING MUSHROOM WITH COMMAND " + placeCommand);
                Debug.Log("BYTE TO BIT STRING: " + 0b101);
            }

            return placedShroom;
        }

        private void ConnectShrooms(ShroomBase start, ShroomBase end)
        {
            var connection = Instantiate(connectionPrefab);
            connection.Initialize(start, end, map);
            _shroomConnections.Add(connection);
        }

        public ShroomBase GetMushroomAtPosition(Vector2Int gridPosition)
        {
            if (debug)
                Debug.Log("GetMushroomAtPosition(" + gridPosition + ")");
            ShroomBase result = null;
            map.TryFindShroom(gridPosition).Iter(shroom => result = shroom);
            return result;
        }

        public bool
            ShroomsInRange(ShroomBase shroomOne, ShroomBase shroomTwo) =>
            PositionsInRange(shroomOne.ShroomPosition,
                shroomTwo.ShroomPosition);

        public bool PositionsInRange(Vector2Int posOne, Vector2Int posTwo)
        {
            var dist = posOne.CellDistance(posTwo);
            // Debug.Log("Distance from Parent: " + dist);
            return dist <= maxDistanceAllowed;
        }

        public Vector3Int GetCellPositionForMush(Vector3 worldPos) =>
            map.WorldToGridPos(worldPos);

        public Vector3 GetWorldPositionForShroomPosition(
            Vector2Int shroomPosition) => map.GridToWorldPos(shroomPosition);

        public void CheckForConnections(ShroomBase placedShroom)
        {
            var shroomsToCheck =
                map.FindOwnedShroomsInRange(placedShroom, maxDistanceAllowed);
            shroomsToCheck.ForEach(shroom =>
            {
                if (!ConnectionExists(placedShroom, shroom))
                    ConnectShrooms(placedShroom, shroom);
            });
        }

        private bool ConnectionExists(ShroomBase start, ShroomBase end)
        {
            return _shroomConnections.Find(connection =>
                (connection.StartShroom.ShroomId == start.ShroomId &&
                 connection.EndShroom.ShroomId == end.ShroomId) ||
                (connection.StartShroom.ShroomId == end.ShroomId &&
                 connection.EndShroom.ShroomId == start.ShroomId)) != null;
        }

        public void ReadPlaceCommand(BaseCommand baseCommand)
        {
            var placeCommand = (PlaceCommand) baseCommand;
            if (placeCommand.ValidPackage)
            {
                var shroomPosition = placeCommand.pos;
                var parentPosition = placeCommand.sourcePos;
                var mushType = map.GetStructureType(placeCommand.mushType);

                SyncShroom(shroomPosition, parentPosition, mushType);
            }
        }

        public void SyncShroom(
            Vector2Int shroomPosition, Vector2Int parentPosition,
            StructureType mushtype)
        {
            var shroom = Instantiate(mushtype.Prefab)
                .GetComponentInChildren<ShroomBase>();
            shroom.WorldPosition =
                GetWorldPositionForShroomPosition(shroomPosition);
            shroom.Parent = GetMushroomAtPosition(parentPosition);
            shroom.Initialize(Blackboard.IsHost ? Team.Blue : Team.Red);
            CheckForConnections(shroom);
            map.AddShroom(shroom);
        }
    }
}