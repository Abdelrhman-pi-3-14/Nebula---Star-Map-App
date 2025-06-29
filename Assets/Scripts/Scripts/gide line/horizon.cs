using UnityEngine;

namespace Ursa {

    [RequireComponent(typeof(LineRenderer))]
    public class Horizon : MonoBehaviour
    {
        public int segments = 3360;
        public float radius = 10f;
        public float lineWidth = 0.05f;

        private LineRenderer lineRenderer;
        private Font labelFont;

        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();

            lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
            lineRenderer.material.color = Color.yellow;
            lineRenderer.positionCount = segments + 1; // +1 to complete the loop
            lineRenderer.loop = true; // Connect the last point to the first
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;

            DrawCircle();
        }

        void Update()
        {
            transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
        }


        void DrawCircle()
        {
            float angleStep = 360f / segments;
            for (int i = 0; i <= segments; i++)
            {
                float angle = Mathf.Deg2Rad * (i * angleStep);
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;
                lineRenderer.SetPosition(i, new Vector3(x, 0, z)); // Y=0 to keep it flat on XZ plane
            }
        }



        void CreateLabel(Transform parent, string abbr, Vector3 position)
        {
            GameObject labelGO = new GameObject("Label");
            labelGO.transform.SetParent(parent);
            labelGO.transform.position = position;

            TextMesh textMesh = labelGO.AddComponent<TextMesh>();
            textMesh.text = abbr;
            textMesh.font = labelFont;
            textMesh.color = Color.yellow;
            textMesh.fontSize = 124;
            textMesh.characterSize = 0.15f;
            textMesh.anchor = TextAnchor.MiddleCenter;

            labelGO.AddComponent<FaceCamera>();
        }

    }

}



