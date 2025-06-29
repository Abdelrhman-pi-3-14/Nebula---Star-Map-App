using UnityEngine;

namespace Ursa
{
    public class DirectionLabels : MonoBehaviour
    {
        public GameObject labelPrefab;
        public float radius = 10f;

        void Start()
        {
            string[] labels = { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            int count = labels.Length;

            for (int i = 0; i < count; i++)
            {
                float angle = i * (360f / count) * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;

                GameObject label = Instantiate(labelPrefab, pos, Quaternion.identity, transform);
                label.name = "Label_" + labels[i];

                TextMesh tm = label.GetComponent<TextMesh>();
                if (tm != null)
                {
                    tm.text = labels[i];
                }

                if (Camera.main != null)
                {
                    label.transform.LookAt(Camera.main.transform);
                    label.transform.Rotate(0, 180, 0);
                }
            }
        }
    }
}