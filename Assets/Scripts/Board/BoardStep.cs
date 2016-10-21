using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BoardStep : MonoBehaviour {

    GameObject playerPawn;
    Board_PlayerPawn playerPawnComponent;
    public enum StepType { Empty, MiniGame, Mission, JobSheet };
    public StepType currentStepType = StepType.Empty;
    public Canvas GameUI;

    public string SceneToLoad;

    SpriteRenderer spriteRenderer;
    [HideInInspector]
    public bool PawnOverThis = false;
    [HideInInspector]
    public bool MouseOverThis = false;

    TextMesh stepContentText;

	// Use this for initialization
	void Start ()
    {
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        GameUI = transform.Find("Canvas").GetComponent<Canvas>();

        stepContentText = transform.FindChild("ContentName").GetComponent<TextMesh>();

        playerPawn = GameObject.Find("Pawn");
        playerPawnComponent = playerPawn.GetComponent<Board_PlayerPawn>();

        if (currentStepType != StepType.Empty)
            DescriptiveTextSetUp();
        else
            stepContentText.text = "";
	}

    void DescriptiveTextSetUp ()
    {
        switch (currentStepType)
        {
            case StepType.JobSheet:
                stepContentText.text = "Fiche Métier";
                break;
            case StepType.Mission:
                stepContentText.text = "Mission";
                break;
            case StepType.MiniGame:
                stepContentText.text = "Mini-Jeu";
                break;
        }

        stepContentText.GetComponent<MeshRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (MouseOverThis && PawnOverThis && Input.GetMouseButtonDown(0))
        {
            Debug.Log("Display launch confirmation according to type & load method");
            playerPawnComponent.abortMove = true;

            if (currentStepType != StepType.Empty)
            {
                GameUI.transform.FindChild("Loading").GetComponent<Text>().enabled = true;
                SceneManager.LoadSceneAsync(SceneToLoad);
            }
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
            stepContentText.GetComponent<MeshRenderer>().enabled = true;
            PawnOverThis = true;
        }
    }

    void OnTriggerExit2D (Collider2D hit)
    {
        if (hit.name == "Pawn")
        {
            spriteRenderer.color = Color.white;
            stepContentText.GetComponent<MeshRenderer>().enabled = false;
            PawnOverThis = false;
        }
    }
}
