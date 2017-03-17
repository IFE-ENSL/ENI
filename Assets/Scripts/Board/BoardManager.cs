using UnityEngine;
using System.Collections;
using Assets.Scripts.Connexion;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Scripts.Utility;

public class BoardManager : MonoBehaviour
{
    #region SQL
    private Waiter _waiter = new Waiter();
    #endregion

    #region External objects & Components
    public Camera mainCamera;
    public Camera LoadingCamera;
    public GameObject mainCanvas;
    GameObject[] Zones;
    #endregion

    #region Misc
    bool InitSpiderNow = true;
    public List<string> ActivitiesNames = new List<string>();
    #endregion

    void Start ()
    {
        //Objects init
        Zones = GameObject.FindGameObjectsWithTag("BoardZone");

        //Init Methods
        StartCoroutine(getUserStatsAtLogin(_waiter));
    }

    void Update()
    {
        ManageLoadingScreen();
    }

    void LateUpdate()
    {
        if (InitSpiderNow) //To avoid getting nullRefExceptions, the spider is actually initialized last in a frame loop
        {
            GameObject.Find("SkillSpider").GetComponent<Spider_Skill_Displayer>().InitSpider();
            InitSpiderNow = false;
        }
    }

    //Called right after the mainBoard is loaded, this loads the player stats (Spider skill,...) AND the board content (Steps names,...)
    public IEnumerator getUserStatsAtLogin(Waiter waiter)
    {
        Debug.Log("Attempting to retrieve the player's stats...");

        waiter.waiting = true;
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        string post_url = SQLCommonVars.getUserStats;
        WWW hs_get = new WWW(post_url, null, headers);
        yield return hs_get;

        if (hs_get.error != null)
        {
            Debug.Log("Error while retrieving all the player's stats : " + hs_get.error);
            Debug.Log(hs_get.text);
            SceneManager.LoadScene(0);
        }

        waiter.data = hs_get.text;
        CharacterSheetManager.userStats = JSON.Parse(waiter.data);
        waiter.waiting = false;

        Debug.Log("Player stats retrieved successfully =)");

        PopulateMainBoard();
        CharacterSheetManager.PopulateCharacterSkills();
    }

    //If the datas are being loaded, we display the loading screen (Which is attached to another camera inside the Unity scene)
    void ManageLoadingScreen ()
    {
        if (_waiter.waiting)
        {
            mainCamera.depth = -1;
            LoadingCamera.depth = 0;
            mainCanvas.SetActive(false);
        }
        else
        {
            mainCamera.depth = 0;
            LoadingCamera.depth = -1;
            mainCanvas.SetActive(true);
        }
    }

    //Uses the previously retrieved server datas to put up all the activities names on the steps and link them to the scenes they will redirect to, etc...
    void PopulateMainBoard ()
    {
        foreach (GameObject zone in Zones)
        {
            BoardStep[] steps = zone.GetComponentsInChildren<BoardStep>();

            int gameListIterator = 0;
            foreach (JSONNode value in CharacterSheetManager.userStats["listeJeux"].Children)
            {
                if (zone.name == "Zone" + value["idZone"].Value)
                {

                    if (gameListIterator < steps.Length)
                    {
                        steps[gameListIterator].SceneToLoad = value["jeuNom"].Value;
                        steps[gameListIterator].stepContentText.text = value["slogan"].Value;

                        if (value["typeJM"].Value == "Jeux")
                            steps[gameListIterator].currentStepType = BoardStep.StepType.MiniGame;
                        else if (value["typeJM"].Value == "Mission")
                        {
                            steps[gameListIterator].currentStepType = BoardStep.StepType.Mission;
                            steps[gameListIterator].MissionId = value["idJeu"].AsInt;
                        }

                        if (steps[gameListIterator].SceneToLoad == "mini-jeu 01")
                            steps[gameListIterator].SceneToLoad = "IntroLabyrinthe";

                        if (steps[gameListIterator].SceneToLoad == "mini-jeu 02")
                            steps[gameListIterator].SceneToLoad = "Management";
                    }
                    else
                    {
                        Debug.LogError("There's more mini-games referenced in the SQL base than there is steps in " + zone.name + "!");
                        break;
                    }
                    Debug.Log(steps[gameListIterator].SceneToLoad);
                    gameListIterator++;
                }
            }

            if (gameListIterator < steps.Length) //If we have more steps on the board that there is content on the server for this zone
            {
                for (int i = gameListIterator; i < steps.Length; i++)
                {
                    steps[i].gameObject.SetActive(false); //We deactivate the steps that are 
                }
            }
        }
    }
}
