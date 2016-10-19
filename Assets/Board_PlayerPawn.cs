using UnityEngine;
using System.Collections;

public class Board_PlayerPawn : MonoBehaviour {

    public GameObject startStep;

    Vector3 targetPosition;

	// Use this for initialization
	void Start ()
    {
        if (startStep == null)
            Debug.LogError("First Step is missing in the pawn script! Please link the first step GameObject to the pawn.");
        else
        {
            //Place the pawn on the first step
            transform.position = startStep.transform.position;
        }

        targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update ()
    {
	    if (Input.GetMouseButtonDown (0))
        {
            targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        targetPosition.z = 0; //Kill the Z to avoid weird stuff in 2D
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3);
    }
}
