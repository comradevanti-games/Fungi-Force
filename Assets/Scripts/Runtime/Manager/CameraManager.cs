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

        private Vector3 _previousMousePosition;

        private float _mouseChangeX, _mouseChangeY;

        public static CameraManager Instance;

        public Camera MainCamera => mainCamera;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
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
        }
    }
}
