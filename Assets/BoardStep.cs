using UnityEngine;
using System.Collections;

public class BoardStep : MonoBehaviour {

    GameObject playerPawn;
    Board_PlayerPawn playerPawnComponent;

    SpriteRenderer spriteRenderer;
    //[HideInInspector]
    public bool PawnOverThis = false;
    //[HideInInspector]
    public bool MouseOverThis = false;
	// Use this for initialization
	void Start ()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();

        playerPawn = GameObject.Find("Pawn");
        playerPawnComponent = playerPawn.GetComponent<Board_PlayerPawn>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	    if (MouseOverThis && PawnOverThis && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Display launch confirmation according to type & load method");
            playerPawnComponent.abortMove = true;
        }
	}

    void OnMouseOver ()
    {
        if(!PawnOverThis)
            spriteRenderer.color = Color.yellow;

        MouseOverThis = true;
    }

    void OnMouseExit ()
    {
        if(!PawnOverThis)
            spriteRenderer.color = Color.white;

        MouseOverThis = false;
    }

    void OnTriggerStay2D (Collider2D hit)
    { 
        if (hit.name == "Pawn")
        {
            spriteRenderer.color = Color.red;
            PawnOverThis = true;
        }
    }

    void OnTriggerExit2D (Collider2D hit)
    {
        if (hit.name == "Pawn")
        {
            spriteRenderer.color = Color.white;
            PawnOverThis = false;
        }
    }
}
