using System.Collections.Generic;
using System.Linq;
using Networking;
using TeamShrimp.GGJ23.Runtime.Networking;
using UnityEngine;

namespace TeamShrimp.GGJ23
{
    public class MushroomManager : MonoBehaviour
    {
        public bool debug;
        
        public static MushroomManager Instance;

        private List<ShroomBase> _shroomsInGame;
        
        private ShroomBase _selectedShroom;

        private GameObject _selectedShroomPrefab;

        private Camera _activeCamera;

        [SerializeField] private float cellLength;

        [SerializeField] private GameObject initialPrefab;

        [SerializeField] private GhostShroom ghostShroom;

        [SerializeField] private float maxDistanceAllowed;

        public GameObject SelectedShroomPrefab
        {
            set => _selectedShroomPrefab = value;
        }

        private MushroomManager() { }

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            GameObject[] existingShrooms = GameObject.FindGameObjectsWithTag("Shroom");
            _shroomsInGame = new List<ShroomBase>();
            
            foreach (GameObject shroom in existingShrooms)
            {
                ShroomBase shroomBase = shroom.GetComponent<ShroomBase>();
                shroomBase.ShroomPosition = Vector2Int.RoundToInt(shroomBase.transform.position);
                shroomBase.Initialize();
                _shroomsInGame.Add(shroomBase);
            }
            
            _selectedShroomPrefab = initialPrefab;
            _activeCamera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            Vector3 mousePosition = _activeCamera.ScreenToWorldPoint(Input.mousePosition);
            if (Input.GetMouseButtonDown(0))
            {
                if (_selectedShroom == null)
                {
                    _selectedShroom = TryGetShroomAtPositon(Vector2Int.RoundToInt(mousePosition));
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
                    ghostShroom.ShroomPosition = Vector2Int.RoundToInt(mousePosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_selectedShroom != null)
                {
                    ShroomBase toAdd = PlaceMushroom(Vector2Int.RoundToInt(mousePosition));
                    if (toAdd)
                        _shroomsInGame.Add(toAdd);
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
            if (debug)
                Debug.Log("Raycast Hit: " + hit.collider.tag);
            if (hit && hit.collider.CompareTag("Shroom"))
            {
                return GetMushroomAtPosition(Vector2Int.CeilToInt(hit.point));
            }

            return null;
        }

        public long GenerateUniqueId()
        {
            if (_shroomsInGame.Count == 0)
            {
                return 1;
            }

            return _shroomsInGame.Max(shroom => shroom.ShroomId) + 1;
        }

        public ShroomBase PlaceMushroom(Vector2Int gridPosition)
        {
            if (_selectedShroom == null || !PositionsInRange(_selectedShroom.ShroomPosition, gridPosition) || TryGetShroomAtPositon(gridPosition) != null)
            {
                return null;
            }
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
            return _shroomsInGame.Find(shroom => shroom.ShroomPosition.Equals(gridPosition));
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
