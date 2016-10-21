using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BoardStep : MonoBehaviour {

    static bool oneStepAlreadyHighlighted = false;

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
        GameUI = GameObject.Find("Canvas").GetComponent<Canvas>();

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
            //Display launch confirmation according to type
            playerPawnComponent.abortMove = true;

            if (currentStepType != StepType.Empty)
            {
                BoardManager.preventPlayerControl = true;
                GameObject ConfirmationWindow = GameUI.transform.FindChild("ConfirmationWindow").gameObject;
                Board_LaunchConfirmation confirmationWindowScript = ConfirmationWindow.GetComponent<Board_LaunchConfirmation>();
                ConfirmationWindow.SetActive(true);
                confirmationWindowScript.SceneToLoadNext = SceneToLoad;
                confirmationWindowScript.UpdateContentName (stepContentText.text);
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
        if (hit.name == "Pawn" && !oneStepAlreadyHighlighted)
        {
            spriteRenderer.color = Color.red;
            stepContentText.GetComponent<MeshRenderer>().enabled = true;
            PawnOverThis = true;
            oneStepAlreadyHighlighted = true;
        }
    }

    void OnTriggerExit2D (Collider2D hit)
    {
        if (hit.name == "Pawn")
        {
            spriteRenderer.color = Color.white;
            stepContentText.GetComponent<MeshRenderer>().enabled = false;
            PawnOverThis = false;
            oneStepAlreadyHighlighted = false;
        }
    }
}
