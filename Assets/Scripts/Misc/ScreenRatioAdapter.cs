using UnityEngine;
using System.Collections;

//Attach this and parameters it to an object which need to see its size and/or position changed according to the screen aspect ratio
public class ScreenRatioAdapter : MonoBehaviour {

    public Vector3 LargeScreenPos;
    public Vector3 LargeScreenScale;

    public Vector3 SmallScreenPos;
    public Vector3 SmallScreenScale;

	// Use this for initialization
	void Start ()
    {
        if (Camera.main.aspect > 1f && Camera.main.aspect < 1.4f) //4/3 Resolution
        {
                transform.position = SmallScreenPos;
                transform.localScale = SmallScreenScale;
        }
        else if (Camera.main.aspect > 1.4f && Camera.main.aspect < 1.8f) //16/9 Resolution
        {
                transform.position = LargeScreenPos;
                transform.localScale = LargeScreenScale;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}
}
