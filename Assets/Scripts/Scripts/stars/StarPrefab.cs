using UnityEngine;



    public class StarPrefab : MonoBehaviour
    {
        // Store a reference to the Star data for later updates
        public Star starData;

        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>(); // Using SpriteRenderer for 2D display

            if (spriteRenderer == null)
            {
                Debug.LogError("?? No SpriteRenderer found on StarPrefab! Make sure your prefab has a SpriteRenderer.");
            }
        }

        // Initialize the prefab with data from the Star class
        public void Initialize(Star star)
        {
            starData = star; // store for later updates
            transform.position = star.position;
            spriteRenderer.color = star.colour; // Set star color
            float minSize = 0.4f; // Prevent stars from being too small
            transform.localScale = Vector3.one * Mathf.Max(star.size, minSize);
            LateUpdate();
        }

        void LateUpdate()
        {
            transform.LookAt(Camera.main.transform);
            transform.Rotate(0, 180, 0); // Flip to face the camera correctly
        }
    }
