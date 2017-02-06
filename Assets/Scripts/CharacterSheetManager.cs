using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterSheetManager : MonoBehaviour {

    public const string baseURL = "http://vm-web7.ens-lyon.fr/eni"; //Prod
    //public const string baseURL = "http://127.0.0.1/eni"; //Local
    //private const string baseURL = "http://vm-web-qualif.pun.ens-lyon.fr/eni/"; //Preprod

    public bool GenerateCharacterSheetDisplay = true;

    Camera SheetCamera;
    GameObject gameCanvas;
    Canvas sheetCanvas;

    static public int game1ID;
    static public int game2ID;

    public Dictionary <int, CompetenceENI> competencesList = new Dictionary<int, CompetenceENI>();
    public Dictionary<int, int> correspondenceUserCompENIAndMiniGame = new Dictionary<int, int>();

    void Update ()
    {
        PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences = competencesList; //Saving for this playsession
    }

    void Start ()
    {
        if (GenerateCharacterSheetDisplay)
        {
            try
            {
                SheetCamera = transform.Find("CharacterSheetCamera").GetComponent<Camera>();
                gameCanvas = GameObject.Find("GameUI");
                sheetCanvas = transform.Find("CharacterSheetCanvas").GetComponent<Canvas>();
            }
            catch (System.Exception)
            {
                Debug.LogError("Character sheet Manager is set to display the sheet, but one or more display component are missing from the scene! Please check that the CharacterSheet Object contains the CharacterSheetCamera, Canvas, and that a general Game UI Exists!");
                GenerateCharacterSheetDisplay = false;
            }
        }

        if (PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences.Count > 0) //Loading for this playsession
            competencesList = PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences;
    }

    public void AddQualityStep (int[] compGenId, int stepIncrementation, string gameLabel)
    {
        int gameID = 0;

        if (gameLabel == "mini-jeu 01")
                gameID = game1ID;

        if (gameLabel == "mini-jeu 02")
            gameID = game2ID;

        foreach (int id in compGenId)
        {
            foreach (KeyValuePair<int, CompetenceENI> keyValue in competencesList)
            {
                if (keyValue.Value._MainSkillNumber == id )
                {
                    Debug.Log("Comp Gen Number is okay...");
                    if (keyValue.Value._idJM == gameID)
                    {
                        Debug.Log("And idJM too !");
                        competencesList[keyValue.Key]._nbPointsCompetence = stepIncrementation;
                        Debug.Log("Calling coroutine for ending player stats...");
                        StartCoroutine(PostPLayerStats(keyValue.Key, competencesList[keyValue.Key]._nbPointsCompetence));
                    }
                    else
                        Debug.Log("But not idJM ='(");
                }
            }
        }
    }

    public void ToggleDisplaySheet ()
    {
        //Basically, we just deactivate any Game UI to replace it with the character sheet.
        //As the spider skill is not rendered by the Unity UI System, it is displayed through
        //another Camera hidden in the game scene.
        if (gameCanvas != null)
        {
            if (gameCanvas.activeSelf)
                gameCanvas.SetActive(false);
            else
                gameCanvas.SetActive(true);
        }

        if (sheetCanvas.enabled)
            sheetCanvas.enabled = false;
        else
            sheetCanvas.enabled = true;

        if (SheetCamera.depth == -2)
        {
            SheetCamera.depth = 0;
        }
        else if (SheetCamera.depth == 0)
        {
            SheetCamera.depth = -2;
        }

        Debug.Log("Toggled Player Sheet");

        //Let's add some controls constraints according to the scene we're in
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "MainBoard")
            ToggleMainBoardConstraints();
    }

    //Sending the skill points to the SQL DB
    public IEnumerator PostPLayerStats(int idCompEni, int pointCompEni)
    {
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        WWWForm hs_post = new WWWForm();
        hs_post.AddField("idCompEni", idCompEni);
        hs_post.AddField("point", pointCompEni);

        Debug.Log("Envoi d'un log au serveur");

        WWW hs_get = new WWW(baseURL + "/web/app_dev.php/unity/compEniPoint", hs_post.data, headers);
        yield return hs_get;
        Debug.Log("Sent new player skill datas");

        if (hs_get.error != null)
        {
            print("Erreur lors de l'envoi des points de skills : " + hs_get.error);
            print(hs_get.text);
            SceneManager.LoadScene(0);
        }
        else if (hs_get.text != "1")
        {
            print("Une erreur est survenue : " + hs_get.text);
            SceneManager.LoadScene(0);
        }
    }

    //This method will prevent the pawn from moving if we're clicking anywhere inside the character sheet
    void ToggleMainBoardConstraints()
    {
        if (BoardManager.preventPlayerControl == true)
            BoardManager.preventPlayerControl = false;
        else if (BoardManager.preventPlayerControl == false)
            BoardManager.preventPlayerControl = true;
    }

}
