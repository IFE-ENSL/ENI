using UnityEngine;
using System.Collections;

public class SeanceNumberForButton : MonoBehaviour {

    // Use this for initialization
    public int seanceNumber = 0;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CallAutoEvalOnClick ()
    {
        GameObject.FindObjectOfType<MissionInterface>().GoToAutoEval(seanceNumber);
    }
}
