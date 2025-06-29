using UnityEngine;


    public class FaceCamera : MonoBehaviour
    {
        private Camera mainCamera;
        void Start()
        {
            mainCamera = Camera.main;
        }
        void Update()
        {
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                                    mainCamera.transform.rotation * Vector3.up);
            }
        }
    }


public class FaceCenter : MonoBehaviour
    {
        private Vector3 center = Vector3.zero;

        void LateUpdate()
        {
            transform.LookAt(center);
            transform.Rotate(0f, 180f, 0f);
        }
    }


