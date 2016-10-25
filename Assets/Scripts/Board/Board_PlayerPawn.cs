using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Board_PlayerPawn : MonoBehaviour {

    public GameObject startStep;

    public LayerMask boardWallsLayer;
    Vector3 targetPosition;
    float distanceFromTargetPos = 0f;
    bool moving = false;
    public bool abortMove = false;

    GameObject currentStep;

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

        targetPosition = transform.position; //Make sure the pawn won't move at start
        BoardManager.preventPlayerControl = false; //If we're coming back from a mini-game, for example, we have to let player have control again
	}
	
	// Update is called once per frame
	void Update ()
    {
        Debug.Log(BoardManager.preventPlayerControl);

        if (!BoardManager.preventPlayerControl)
        {
            if (Input.GetMouseButtonDown(0))
            {
                targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Set where the player clicked before sending the pawn to this position

                //Let's check if where we clicked is a wall already near to the pawn, so we avoid jitter
                if (Physics2D.Raycast(transform.position, Vector3.Normalize(targetPosition - transform.position), 1f, boardWallsLayer))
                {
                    Debug.DrawRay(transform.position, Vector3.Normalize(targetPosition - transform.position), Color.blue, 5F);
                    abortMove = true;
                }

                //Or if we just clicked a UI Button
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    abortMove = true;
                }
            }

            targetPosition.z = 0; //Kill the Z to avoid weird stuff in 2D

            if (!abortMove) // If this bool isn't marked true, we can move the pawn to the target position without troubles
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 3);
            else
            {
                targetPosition = transform.position; //Else, it stays where it is.
                abortMove = false;
            }

            //Next we check if the pawn is still moving by checking if it is near to its target position
            //We Will reuse this for some method, for example to stop the move if we encounter a wall during a movement
            distanceFromTargetPos = Vector3.SqrMagnitude(transform.position - targetPosition);

            if (distanceFromTargetPos < .2f)
            {
                moving = false;
            }
            else
                moving = true;
        }
    }

    void OnCollisionEnter2D (Collision2D hit)
    {
        if (moving) //Stop the movement if the pawn is already moving and just hit a wall
            abortMove = true;
    }

    void OnCollisionStay2D(Collision2D hit)
    {
        if (moving) //Stop the movement if the pawn is already moving and just hit a wall
            abortMove = true;
    }

    void OnTriggerStay2D (Collider2D hit)
    {
        if (hit.CompareTag("BoardStep"))
            currentStep = hit.transform.gameObject;
    }

    void PawnAndMouseOverStep ()
    {
        BoardStep currentStepScript = currentStep.GetComponent<BoardStep>();
        currentStepScript.MouseOverThis = true;
        currentStepScript.PawnOverThis = true;
    }

    void MouseNotOverPawn()
    {
        BoardStep currentStepScript = currentStep.GetComponent<BoardStep>();
        currentStepScript.MouseOverThis = false;
    }

    void OnMouseOver()
    {
        if (currentStep != null)
        {
            PawnAndMouseOverStep();
        }
    }

    void OnMouseExit ()
    {
        if (currentStep != null)
        {
            MouseNotOverPawn();
        }
    }

}
