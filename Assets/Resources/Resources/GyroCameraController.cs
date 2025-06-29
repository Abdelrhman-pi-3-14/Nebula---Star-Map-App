using UnityEngine;


    public class GyroCameraController : MonoBehaviour
    {
        [Header("Rotation Settings")]
        public float sensitivity = 1f;
        public float smoothness = 5f;
        public bool invertPitch = false;
        public bool invertYaw = false;

        private Quaternion initialRotation;
        private Quaternion targetRotation;
        private Quaternion gyroInitialRotation;
        private UrsaSensorHandler sensorHandler;

        void Start()
        {
            sensorHandler = FindObjectOfType<UrsaSensorHandler>();
            if(sensorHandler == null)
            {
                Debug.LogError("UrsaSensorHandler not found in scene!");
            }
            Input.gyro.enabled = true;
            InitializeRotation();
        }

        void InitializeRotation()
        {
            initialRotation = transform.rotation;
            targetRotation = initialRotation;
            gyroInitialRotation = Quaternion.Inverse(GetGyroRotation());
        }

        void Update()
        {
        Quaternion rawRotation = sensorHandler != null ? sensorHandler.CurrentGyroRotation : GetGyroRotation();
            Quaternion currentGyroRotation = gyroInitialRotation * rawRotation;
            targetRotation = Quaternion.Slerp(transform.rotation, currentGyroRotation, smoothness * Time.deltaTime);
            transform.rotation = targetRotation;
        }

        Quaternion GetGyroRotation()
        {
            Quaternion raw = Input.gyro.attitude;
            return new Quaternion(raw.x, raw.y, -raw.z, -raw.w);
        }

        public void Calibrate()
        {
            InitializeRotation();
        }
    }

