using UnityEngine;

namespace Ursa
{
    public class AzimuthLines : MonoBehaviour
    {
        public int numLines = 12;
        public float sphereRadius = 10f;
        public float lineWidth = 0.02f;

        private UrsaSensorHandler sensorHandler;

        [System.Obsolete]
        void Start()
        {
            sensorHandler = FindObjectOfType<UrsaSensorHandler>();
            if (sensorHandler == null)
            {
                Debug.LogError("UrsaSensorHandler not found!");
                return;
            }

            float angleStep = 360f / numLines;

            for (int i = 0; i < numLines; i++)
            {
                float azimuthAngle = i * angleStep + 90f;
                DrawRALine(azimuthAngle, i);
            }
        }

        void Update()
        {
            if (sensorHandler != null)
            {
                transform.rotation = Quaternion.Euler(0, -sensorHandler.CurrentHeading, 0);
            }
        }

        void DrawRALine(float azimuthAngle, int index)
        {
            GameObject lineObj = new GameObject("RALine_" + index);
            lineObj.transform.parent = transform;

            lineObj.layer = LayerMask.NameToLayer("Ignore Raycast");

            LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.white;
            lineRenderer.positionCount = 360;
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.useWorldSpace = false;

            float step = 360f / (lineRenderer.positionCount - 1);
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                float theta = Mathf.Deg2Rad * (i * step);
                float x = sphereRadius * Mathf.Cos(theta) * Mathf.Cos(Mathf.Deg2Rad * azimuthAngle);
                float y = sphereRadius * Mathf.Sin(theta);
                float z = sphereRadius * Mathf.Cos(theta) * Mathf.Sin(Mathf.Deg2Rad * azimuthAngle);
                lineRenderer.SetPosition(i, new Vector3(x, y, z));
            }
        }
    }
}
