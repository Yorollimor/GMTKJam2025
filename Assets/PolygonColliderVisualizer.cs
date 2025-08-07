using UnityEngine;

[ExecuteAlways] // Draws in edit mode and play mode
public class PolygonColliderVisualizer : MonoBehaviour
{
    public PolygonCollider2D polygonCollider;
    public Color lineColor = Color.red;

    void OnDrawGizmos()
    {
        if (polygonCollider == null) return;

        Gizmos.color = lineColor;

        // PolygonCollider2D can have multiple separate paths
        for (int p = 0; p < polygonCollider.pathCount; p++)
        {
            Vector2[] points = polygonCollider.GetPath(p);

            for (int i = 0; i < points.Length; i++)
            {
                // Convert local point to world position
                Vector3 current = polygonCollider.transform.TransformPoint(points[i]);
                Vector3 next = polygonCollider.transform.TransformPoint(points[(i + 1) % points.Length]);

                Gizmos.DrawLine(current, next);
            }
        }
    }
}
