using UnityEngine;
using System.Collections;

public class CustomizeLineRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;
    public Vector3[] linePositions;

    public void RoughCurve () //This is mainly used in debug to have a clear, rough view of the curve.
    {
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.SetVertexCount(linePositions.Length);
        int iterator = 0;
        foreach (Vector3 position in linePositions)
        {
            lineRenderer.SetPosition(iterator, position);
            iterator++;
        }
    }
}
