using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class BoardStep : MonoBehaviour {

    #region Step States
    [HideInInspector]
    public bool MouseOverThis = false;

    public enum StepType { Empty, MiniGame, Mission, JobSheet };
    public StepType currentStepType = StepType.Empty;
    public TextMesh stepContentText;
    #endregion

    #region External Objects
    public Canvas GameUI;
    PersistentFromSceneToScene persistentDatas;
    #endregion

    #region Misc
    public string SceneToLoad;
    public int OptionalAltSceneLayout = 0;
    public int MissionId = 0;
    SpriteRenderer spriteRenderer;
    #endregion

	// Use this for initialization
	void Start ()
    {
        persistentDatas = GameObject.Find("PersistentSceneDatas").GetComponent<PersistentFromSceneToScene>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        GameUI = GameObject.Find("Canvas").GetComponent<Canvas>();
        stepContentText = transform.FindChild("ContentName").GetComponent<TextMesh>();

        if (currentStepType != StepType.Empty)
            stepContentText.GetComponent<MeshRenderer>().enabled = false;
        else
            stepContentText.text = "";
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

                if(currentStepType == StepType.Mission)
                    persistentDatas.missionId = MissionId;

                GameObject ConfirmationWindow = GameUI.transform.FindChild("ConfirmationWindow").gameObject;
                ConfirmationWindow confirmationWindowScript = ConfirmationWindow.GetComponent<ConfirmationWindow>();
                ConfirmationWindow.SetActive(true);

                //Depending if what we're launching is a mission or a game, we set this...
                if (currentStepType == StepType.MiniGame)
                    confirmationWindowScript.SceneToLoadNext = SceneToLoad;
                else
                    confirmationWindowScript.SceneToLoadNext = "MissionInterface";

                confirmationWindowScript.UpdateContentName (stepContentText.text);
            }
        }
	}

    void OnMouseOver ()
    {
        spriteRenderer.color = Color.red;
        stepContentText.GetComponent<MeshRenderer>().enabled = true;

        MouseOverThis = true;
    }

    void OnMouseExit ()
    {
        spriteRenderer.color = Color.white;
        stepContentText.GetComponent<MeshRenderer>().enabled = false;

        MouseOverThis = false;
    }
}
