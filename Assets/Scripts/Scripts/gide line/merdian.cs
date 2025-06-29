using UnityEngine;

namespace Ursa
{
    [RequireComponent(typeof(LineRenderer))]
    public class Meridian : MonoBehaviour
    {
        public int segments = 5000;
        public float radius = 10f;
        public float lineWidth = 0.05f;

        private LineRenderer lineRenderer;
        private UrsaSensorHandler sensorHandler;

        [System.Obsolete]
        void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            lineRenderer = GetComponent<LineRenderer>();
            sensorHandler = FindObjectOfType<UrsaSensorHandler>();

            if (sensorHandler == null)
            {
                Debug.LogError("UrsaSensorHandler not found!");
                return;
            }

            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.green;
            lineRenderer.positionCount = segments + 1;
            lineRenderer.loop = true;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            DrawMeridian();
        }

        void DrawMeridian()
        {
            float angleStep = 360f / segments;
            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.Deg2Rad * (i * angleStep);
                float x = Mathf.Cos(angle) * radius;
                float y = Mathf.Sin(angle) * radius;
                lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            }
        }

        void Update()
        {
            if (sensorHandler != null)
            {
                transform.rotation = Quaternion.Euler(0, -sensorHandler.CurrentHeading, 0);
            }
        }
    }

}