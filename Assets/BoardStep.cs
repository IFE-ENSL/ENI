using UnityEngine;
using System.Collections;

public class BoardStep : MonoBehaviour {

    SpriteRenderer spriteRenderer;
    bool PawnOverThis = false;

	// Use this for initialization
	void Start ()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnMouseOver ()
    {
        if(!PawnOverThis)
            spriteRenderer.color = Color.yellow;
    }

    void OnMouseExit ()
    {
        if(!PawnOverThis)
            spriteRenderer.color = Color.white;
    }

    void OnTriggerStay2D (Collider2D hit)
    { 
        Debug.Log ("Something's on me, SEND HELP!!! " + hit.name);
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
