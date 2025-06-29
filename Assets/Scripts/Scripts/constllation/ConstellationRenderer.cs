
using System.Collections.Generic;
using UnityEngine;


    public class ConstellationRenderer : MonoBehaviour
    {
        [Header("Assets")]
        public TextAsset constellationLinesFile;
        public Material lineMaterial;
        public Font labelFont;


    [Header("Line Appearance")]
    public Color constellationLineColor;


    [Header("Settings")]
        public float lineWidth = 0.02f;
        public float skyRadius = 200f;

        private StarManager starManager;
        private Dictionary<string, List<int>> constellationConnections;

        [System.Obsolete]
        void Start()
        {
            starManager = FindObjectOfType<StarManager>();
            if (starManager == null)
            {
                Debug.LogError("StarManager not found in scene!");
                return;
            }

            if (constellationLinesFile == null)
                constellationLinesFile = Resources.Load<TextAsset>("ConstellationLines");

            constellationConnections = ConstellationLinesParser.ParseConstellationLines(constellationLinesFile);

            foreach (var kv in constellationConnections)
                RenderConstellation(kv.Key, kv.Value);
        }

        void RenderConstellation(string abbr, List<int> bscNumbers)
        {
            GameObject constellationGO = new GameObject(abbr);
            constellationGO.transform.SetParent(transform);
            List<Vector3> validPositions = new List<Vector3>();

            for (int i = 0; i < bscNumbers.Count - 1; i++)
            {
                int idStart = bscNumbers[i];
                int idEnd = bscNumbers[i + 1];

                if (!starManager.starDictionary.TryGetValue(idStart, out Star starStart) ||
                    !starManager.starDictionary.TryGetValue(idEnd, out Star starEnd))
                {
                    Debug.LogWarning($"Missing stars in {abbr}: {idStart} or {idEnd}");
                    continue;
                }

                Vector3 posStart = starStart.position.normalized * skyRadius;
                Vector3 posEnd = starEnd.position.normalized * skyRadius;

                validPositions.Add(posStart);
                validPositions.Add(posEnd);

                CreateLine(constellationGO.transform, posStart, posEnd, starStart, starEnd);
            }

            if (validPositions.Count > 0)
                CreateLabel(constellationGO.transform, abbr, CalculateCentroid(validPositions));
        }

        void CreateLine(Transform parent, Vector3 start, Vector3 end, Star star1, Star star2)
        {
            GameObject lineObj = new GameObject("Line");
            lineObj.transform.SetParent(parent);

            LineRenderer lr = lineObj.AddComponent<LineRenderer>();
            lr.material = lineMaterial;
            lr.widthMultiplier = lineWidth;
            lr.positionCount = 2;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);

            bool startVisible = star1.position.y >= 0;
            bool endVisible = star2.position.y >= 0;
            Color lineColor = constellationLineColor;
            lr.startColor = lineColor;
            lr.endColor = lineColor;
        }

        Vector3 CalculateCentroid(List<Vector3> positions)
        {
            Vector3 centroid = Vector3.zero;
            foreach (Vector3 pos in positions) centroid += pos;
            return centroid / positions.Count;
        }

        void CreateLabel(Transform parent, string abbr, Vector3 position)
        {
            GameObject labelGO = new GameObject("Label");
            labelGO.transform.SetParent(parent);
            labelGO.transform.position = position;

            TextMesh textMesh = labelGO.AddComponent<TextMesh>();
            textMesh.text = ConstellationNames.FullNames.TryGetValue(abbr, out string name) ? name : abbr;
            textMesh.font = labelFont;
            textMesh.color = Color.white;
            textMesh.fontSize = 224;
            textMesh.characterSize = 0.15f;
            textMesh.anchor = TextAnchor.MiddleCenter;

            labelGO.AddComponent<FaceCenter>();
        }
        public void UpdateConstellations()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject); // Remove old lines and labels
            }

            foreach (var kv in constellationConnections)
            {
                RenderConstellation(kv.Key, kv.Value);
            }
        }



        public void ToggleLines(bool visible)
        {
            int lineCount = 0;
            foreach (Transform constellation in transform)
            {
                foreach (Transform child in constellation)
                {
                    if (child.GetComponent<LineRenderer>())
                    {
                        child.gameObject.SetActive(visible);
                        lineCount++;
                    }
                }
            }
            Debug.Log($"Toggled {lineCount} lines to {visible}");
        }

        public void ToggleLabels(bool visible)
        {
            foreach (Transform constellation in transform)
            {
                TextMesh label = constellation.GetComponentInChildren<TextMesh>(true);
                if (label != null)
                    label.gameObject.SetActive(visible);
            }
        }
    }
