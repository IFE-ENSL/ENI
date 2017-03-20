using UnityEngine;
using System.Collections;

public class CustomizeLineRenderer : MonoBehaviour
{
    LineRenderer lineRenderer;
    public Vector3[] linePositions;

    public void RoughCurve () //Used to display the lines on the spider
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
