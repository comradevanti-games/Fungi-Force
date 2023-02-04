using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TeamShrimp.GGJ23
{
    public class CameraManager : MonoBehaviour
    {

        [SerializeField] private Camera mainCamera;

        [SerializeField] private float cameraSmoothing;

        [SerializeField] private bool pointyTop;

        [SerializeField] private float TESTING_MAP_SIZE;

        private Vector3 _previousMousePosition;

        private float _mouseChangeX, _mouseChangeY;

        public static CameraManager Instance;

        public Camera MainCamera => mainCamera;
        
        public float MapSize { get; set; }

        private float _mapWidth, _mapHeight;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            if (Blackboard.Game != null)
                MapSize = Blackboard.Game.MapSize;
            if (MapSize == 0f)
            {
                MapSize = TESTING_MAP_SIZE;
            }

            if (MainCamera == null)
            {
                mainCamera = Camera.main;
            }

                if (pointyTop)
            {
                this._mapWidth = (float)(MapSize * Math.Sqrt(3));
                this._mapHeight = 2 * MapSize;
            }
            else
            {
                this._mapWidth = 2 * MapSize;
                this._mapHeight = (float)(MapSize * Math.Sqrt(3));
            }
        }

        // Update is called once per frame
        void Update()
        {
            float horizontalMovement = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            if (Math.Abs(horizontalMovement) + Math.Abs(verticalMovement) > 0)
            {
                MainCamera.transform.position += MainCamera.transform.right * (horizontalMovement * cameraSmoothing) +
                                                 MainCamera.transform.up * (verticalMovement * cameraSmoothing);
            }

            if (Input.GetMouseButtonDown(1))
            {
                this._previousMousePosition = MainCamera.ScreenToWorldPoint(Input.mousePosition);
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 currentMousePositon = MainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector2 normalizedMouseChange = new Vector2(this._previousMousePosition.x - currentMousePositon.x,
                    this._previousMousePosition.y - currentMousePositon.y);
                this._mouseChangeX = normalizedMouseChange.x;
                this._mouseChangeY = normalizedMouseChange.y;

                if (Math.Abs(this._mouseChangeX) + Math.Abs(this._mouseChangeY) > 0)
                {
                    MainCamera.transform.position += MainCamera.transform.right * (this._mouseChangeX * cameraSmoothing) +
                                                     MainCamera.transform.up * (this._mouseChangeY * cameraSmoothing);
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                this._previousMousePosition = Vector3.zero;
                this._mouseChangeX = 0.0f;
                this._mouseChangeY = 0.0f;
            }

            MainCamera.transform.position = new Vector3(
                Mathf.Clamp(MainCamera.transform.position.x, -_mapWidth / 2, _mapWidth / 2),
                Mathf.Clamp(MainCamera.transform.position.y, -_mapHeight / 2, _mapHeight / 2),
                MainCamera.transform.position.z
            );
        }
    }
}
