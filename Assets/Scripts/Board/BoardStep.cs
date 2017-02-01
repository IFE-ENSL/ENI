using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BoardStep : MonoBehaviour {

    static bool oneStepAlreadyHighlighted = false;

    public enum StepType { Empty, MiniGame, Mission, JobSheet };
    public StepType currentStepType = StepType.Empty;
    public Canvas GameUI;
    PersistentFromSceneToScene persistentDatas;

    public string SceneToLoad;
    public int OptionalAltSceneLayout = 0;

    SpriteRenderer spriteRenderer;
    [HideInInspector]
    public bool PawnOverThis = false;
    [HideInInspector]
    public bool MouseOverThis = false;

    [SerializeField]
    bool DebugThis = false;

    public TextMesh stepContentText;

	// Use this for initialization
	void Start ()
    {
        persistentDatas = GameObject.Find("PersistentSceneDatas").GetComponent<PersistentFromSceneToScene>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        GameUI = GameObject.Find("Canvas").GetComponent<Canvas>();

        stepContentText = transform.FindChild("ContentName").GetComponent<TextMesh>();

        if (currentStepType != StepType.Empty)
            DescriptiveTextSetUp();
        else
            stepContentText.text = "";
	}

    void DescriptiveTextSetUp ()
    {
        /*switch (currentStepType)
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
        }*/

        if (DebugThis)
            stepContentText.text += " " + SceneToLoad;

        stepContentText.GetComponent<MeshRenderer>().enabled = false;
    }
	
	// Update is called once per frame
	void Update () 
    {
	    if (MouseOverThis && Input.GetMouseButtonDown(0))
        {
            //Display launch confirmation according to type


            if (currentStepType != StepType.Empty)
            {
                persistentDatas.alternativeSceneId = OptionalAltSceneLayout;

                if (persistentDatas.alternativeSceneId <= 0)
                    persistentDatas.alternativeSceneId = 1;

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
        spriteRenderer.color = Color.red;
        stepContentText.GetComponent<MeshRenderer>().enabled = true;
        oneStepAlreadyHighlighted = true;

        MouseOverThis = true;
    }

    void OnMouseExit ()
    {
        spriteRenderer.color = Color.white;
        stepContentText.GetComponent<MeshRenderer>().enabled = false;
        oneStepAlreadyHighlighted = false;

        MouseOverThis = false;
    }

    void OnTriggerStay2D (Collider2D hit)
    { 
        if (hit.name == "Pawn" && !oneStepAlreadyHighlighted)
        {

        }
    }

    void OnTriggerExit2D (Collider2D hit)
    {
        if (hit.name == "Pawn")
        {

        }
    }
}
