using System.Collections.Generic;
using System.Linq;
using ComradeVanti.CSharpTools;
using TeamShrimp.GGJ23.Networking;
using UnityEngine;

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

        [SerializeField] private float cellLength;

        [SerializeField] private GameObject initialPrefab;

        [SerializeField] private GhostShroom ghostShroom;

        [SerializeField] private float maxDistanceAllowed;
        
        [SerializeField] private MapKeeper map;

        public GameObject SelectedShroomPrefab
        {
            set => _selectedShroomPrefab = value;
        }

        private MushroomManager() { }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            List<ShroomBase> existingShrooms = map.AllShrooms;
            
            foreach (ShroomBase shroom in existingShrooms)
            {
                shroom.ShroomPosition = Vector2Int.FloorToInt(shroom.transform.position);
                shroom.Initialize();
            }
            
            _selectedShroomPrefab = initialPrefab;
            _activeCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 11;
            mousePosition = _activeCamera.ScreenToWorldPoint(mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (_selectedShroom == null)
                {
                    Vector2Int? gridPosition = map.GetClosestMapPoint(mousePosition);
                    if (gridPosition.HasValue)
                        _selectedShroom = GetMushroomAtPosition(gridPosition.Value);
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
                    Vector2Int? gridPosition = map.GetClosestMapPoint(mousePosition);
                    ghostShroom.ShroomPosition = gridPosition.HasValue ? gridPosition.Value : Vector2Int.FloorToInt(mousePosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_selectedShroom != null)
                {
                    Vector2Int? gridPosition = map.GetClosestMapPoint(mousePosition);
                    ShroomBase toAdd = null;
                    if (gridPosition.HasValue)
                        toAdd = PlaceMushroom(gridPosition.Value);
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
                || !map.CanPlace(_selectedShroomPrefab.GetComponent<ShroomBase>().ShroomType, gridPosition))
            {
                return null;
            }
            if (debug)
                Debug.Log("Is free");
            ShroomBase placedShroom = Instantiate(_selectedShroomPrefab).GetComponent<ShroomBase>();
            placedShroom.ShroomPosition = gridPosition;

            if (_selectedShroom != null)
            {
                _selectedShroom.ConnectChild(placedShroom);
            }
            
            placedShroom.Initialize();

            PlaceCommand placeCommand = new PlaceCommand(placedShroom.ShroomType.name, placedShroom.ShroomId, placedShroom.ShroomPosition, _selectedShroom.ShroomPosition);
            NetworkManager.client.SendCommand(placeCommand);
            if (debug)
            {
                Debug.Log("SENDING MUSHROOM WITH COMMAND " + placeCommand);
                Debug.Log("BYTE TO BIT STRING: " + 0b101);
            }
            return placedShroom;
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
            return Vector2Int.Distance(posOne, posTwo) <= maxDistanceAllowed;
        }
    }
}
