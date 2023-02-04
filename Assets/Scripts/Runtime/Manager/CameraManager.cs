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
            float horizontalMovemnt = Input.GetAxis("Horizontal");
            float verticalMovement = Input.GetAxis("Vertical");

            MainCamera.transform.position += MainCamera.transform.forward * (horizontalMovemnt * cameraSmoothing) +
                                             MainCamera.transform.up * (verticalMovement * cameraSmoothing);
        }
    }
}
