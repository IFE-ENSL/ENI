using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DummyBezier))]
public class DrawBezierHandleEditor : Editor
{
    void OnSceneGUI()
    {
        DummyBezier t = target as DummyBezier;

        Handles.DrawBezier(t.transform.position, Vector3.up, t.transform.position, -Vector3.up,
                            Color.white, null, HandleUtility.GetHandleSize(Vector3.zero));
    }
}