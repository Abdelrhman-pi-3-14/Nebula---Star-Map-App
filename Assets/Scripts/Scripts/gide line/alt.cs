using UnityEngine;

public class AltitudeCircles : MonoBehaviour
{
    public int segments = 360;  // Number of points per circle
    public float maxRadius = 10f;  // Horizon radius (0° altitude)
    public float lineWidth = 0.02f;
    public int numCircles = 9; // 9 circles (every 10° from 0° to 90°)

    void Start()
    {
        for (int i = 1; i <= numCircles; i++)
        {
            float altitudeAngle = i * 10f; // 10° steps up to 90°
            float radius = maxRadius * Mathf.Cos(Mathf.Deg2Rad * altitudeAngle); // Shrinks with altitude
            float y = maxRadius * Mathf.Sin(Mathf.Deg2Rad * altitudeAngle); // Raises with altitude

            DrawCircle(radius, y, i);
        }

        for (int i = -9; i <= 9; i++) // Now from -90° to 90°
        {
            float altitudeAngle = i * 10f; // 10° steps up to 90°
            float radius = maxRadius * Mathf.Cos(Mathf.Deg2Rad * altitudeAngle); // Shrinks with altitude
            float y = maxRadius * Mathf.Sin(Mathf.Deg2Rad * altitudeAngle); // Raises with altitude

            DrawCircle(radius, y, i);

        }
    }

    void DrawCircle(float radius, float y, int index)
    {
        GameObject circleObj = new GameObject("AltitudeCircle_" + index);
        circleObj.transform.parent = transform; // Keep organized under this object

        LineRenderer lineRenderer = circleObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.white;
        lineRenderer.positionCount = segments + 1; // Looping circle needs an extra point
        lineRenderer.loop = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;

        float angleStep = 360f / segments;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);

            lineRenderer.SetPosition(i, new Vector3(x, y, z));
        }
    }
}
