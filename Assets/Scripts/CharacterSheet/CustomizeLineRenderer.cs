using UnityEngine;
using System.Collections;

public class CustomizeLineRenderer : MonoBehaviour {

   

    LineRenderer lineRenderer;
    public Vector3[] linePositions;


	// Use this for initialization
	void Start ()
    {
    }

    public void ClearCurve ()
    {
        //linePositions = null;
    }
	
    public void SmoothCurve ()
    {
        ClearCurve(); //In case a smooth curve was calculated before, we must clear it.

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        linePositions = Curver.MakeSmoothCurve(linePositions, 5);

        lineRenderer.SetVertexCount(linePositions.Length);
        int iterator = 0;
        foreach (Vector3 position in linePositions)
        {
            lineRenderer.SetPosition(iterator, position);
            iterator++;
        }
    }

    public void RoughCurve () //This is mainly used in debug to have a clear, rough view of the curve.
    {
        ClearCurve();

        lineRenderer = gameObject.GetComponent<LineRenderer>();

        lineRenderer.SetVertexCount(linePositions.Length);
        int iterator = 0;
        foreach (Vector3 position in linePositions)
        {
            lineRenderer.SetPosition(iterator, position);
            iterator++;
        }
    }

	// Update is called once per frame
	void Update ()
    {
       
	}


}
