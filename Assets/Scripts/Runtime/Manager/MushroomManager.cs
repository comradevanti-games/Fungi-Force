using System.Collections.Generic;
using System.Linq;
using ComradeVanti.CSharpTools;
using TeamShrimp.GGJ23.Networking;
using TeamShrimp.GGJ23.Runtime;
using UnityEngine;
using UnityEngine.Events;

namespace TeamShrimp.GGJ23
{
    public class MushroomManager : MonoBehaviour
    {
        public bool debug;
        
        public static MushroomManager Instance;

        // private List<ShroomBase> _shroomsInGame;
        
        private ShroomBase _selectedShroom;

        private GameObject _selectedShroomPrefab;

        private Camera _activeCamera;

        private List<ShroomConnection> _shroomConnections = new List<ShroomConnection>();

        [SerializeField] private float cellLength;

        [SerializeField] private GameObject initialPrefab;

        [SerializeField] private GhostShroom ghostShroom;
        [SerializeField] private NetworkManager networkManager;

        [SerializeField] private float maxDistanceAllowed;
        
        [SerializeField] private MapKeeper map;
        [SerializeField] private UnityEvent onShroomPlaced;

        [SerializeField] private ShroomConnection connectionPrefab;

        public GameObject SelectedShroomPrefab
        {
            set => _selectedShroomPrefab = value;
        }

        private MushroomManager() { }

        void Awake()
        {
            Instance = this;
        }

        
        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            List<ShroomBase> existingShrooms = map.AllShrooms;
            
            foreach (ShroomBase shroom in existingShrooms)
            {
                shroom.Initialize();
            }
            
            _selectedShroomPrefab = initialPrefab;
            _activeCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mousePosition = Input.mousePosition;
            // mousePosition.z = Mathf.Abs(CameraManager.Instance.MainCamera.transform.position.z) + 1;
            mousePosition = CameraManager.Instance.MainCamera.ScreenToWorldPoint(mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (_selectedShroom == null)
                {
                    Vector3Int gridPosition = map.WorldToGridPos(mousePosition);
                    _selectedShroom = GetMushroomAtPosition((Vector2Int) gridPosition);
                    if (debug)
                        Debug.Log("Found Shroom: " + _selectedShroom);
                }
            }
            else if (Input.GetMouseButton(0))
            {
                if (_selectedShroom != null && !ghostShroom.gameObject.activeSelf)
                {
                    ghostShroom.gameObject.SetActive(true);
                    ghostShroom.Parent = _selectedShroom;
                }

                if (ghostShroom.gameObject.activeSelf)
                {
                    Vector3 gridPosition = map.SnapToGridPos(mousePosition);
                    ghostShroom.WorldPosition = gridPosition;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_selectedShroom != null)
                {
                    Vector3Int gridPosition = map.WorldToGridPos(mousePosition);
                    ShroomBase toAdd = null;
                    toAdd = PlaceMushroom((Vector2Int) gridPosition);
                    if (debug)
                        Debug.Log("I added " + toAdd);
                    if (toAdd)
                        this.map.AddShroom(toAdd);
                    ghostShroom.gameObject.SetActive(false);
                    _selectedShroom = null; 
                }
            }
        }

        public ShroomBase TryGetShroomAtPositon(Vector2Int gridPosition)
        {
            if(debug)
                Debug.Log("TryGetShroomAtPosition(" + gridPosition + ")");
            // TODO Change to Map request
            RaycastHit2D hit = Physics2D.CircleCast(gridPosition, cellLength, Vector2.zero);
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
            if (map.AllShrooms.Count == 0)
            {
                return 1;
            }

            return map.AllShrooms.Max(shroom => shroom.ShroomId) + 1;
        }

        public ShroomBase PlaceMushroom(Vector2Int gridPosition)
        {
            if (debug)
                Debug.Log("Placing Shroom at: " + gridPosition + ", selected Shroom is: " + _selectedShroom);
            if (_selectedShroom == null || !PositionsInRange(_selectedShroom.ShroomPosition, gridPosition)
                || !map.CanPlace(_selectedShroomPrefab.GetComponentInChildren<ShroomBase>().ShroomType, gridPosition))
            {
                return null;
            }
            if (debug)
                Debug.Log("Is free");
            ShroomBase placedShroom = Instantiate(_selectedShroomPrefab).GetComponentInChildren<ShroomBase>();
            placedShroom.WorldPosition = map.GridToWorldPos(gridPosition);
            
            Debug.Log(placedShroom);
            
            CheckForConnections(placedShroom);
            
            Debug.Log("Checked for Connections");
            
            placedShroom.Initialize();
            onShroomPlaced.Invoke();
            
            Debug.Log("Invoked Event");
            
            PlaceCommand placeCommand = new PlaceCommand(placedShroom.ShroomType.name, placedShroom.ShroomId, placedShroom.ShroomPosition, _selectedShroom.ShroomPosition);
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
            ShroomConnection connection = Instantiate(connectionPrefab);
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

        public bool ShroomsInRange(ShroomBase shroomOne, ShroomBase shroomTwo)
        {
            return PositionsInRange(shroomOne.ShroomPosition, shroomTwo.ShroomPosition);
        }

        public bool PositionsInRange(Vector2Int posOne, Vector2Int posTwo)
        {
            float dist = posOne.CellDistance(posTwo);
            // Debug.Log("Distance from Parent: " + dist);
            return dist <= maxDistanceAllowed;
        }

        public Vector3Int GetCellPositionForMush(Vector3 worldPos)
        {
            return map.WorldToGridPos(worldPos);
        }

        public Vector3 GetWorldPositionForShroomPosition(Vector2Int shroomPosition)
        {
            return map.GridToWorldPos(shroomPosition);
        }

        public void CheckForConnections(ShroomBase placedShroom)
        {
            List<ShroomBase> shroomsToCheck = map.FindShroomsInRange(placedShroom, maxDistanceAllowed);
            shroomsToCheck.ForEach(shroom =>
            {
                if (!ConnectionExists(placedShroom, shroom))
                    ConnectShrooms(placedShroom, shroom);
            });
        }

        private bool ConnectionExists(ShroomBase start, ShroomBase end)
        {
            return _shroomConnections.Find(connection => (connection.StartShroom.ShroomId == start.ShroomId &&
                                                          connection.EndShroom.ShroomId == end.ShroomId) ||
                                                         (connection.StartShroom.ShroomId == end.ShroomId &&
                                                          connection.EndShroom.ShroomId == start.ShroomId)) != null;
        }

        public void ReadPlaceCommand(BaseCommand baseCommand)
        {
            PlaceCommand placeCommand = (PlaceCommand) baseCommand;
            if (placeCommand.ValidPackage)
            {
                Vector2Int shroomPosition = placeCommand.pos;
                Vector2Int parentPosition = placeCommand.sourcePos;
                StructureType mushType = map.GetStructureType(placeCommand.mushType);
                
                SyncShroom(shroomPosition, parentPosition, mushType);
            }
        }

        public void SyncShroom(Vector2Int shroomPosition, Vector2Int parentPosition, StructureType mushtype)
        {
            ShroomBase shroom = Instantiate(mushtype.Prefab).GetComponentInChildren<ShroomBase>();
            shroom.WorldPosition = GetWorldPositionForShroomPosition(shroomPosition);
            shroom.Parent = GetMushroomAtPosition(parentPosition);
            shroom.Initialize();
        }
    }
}
