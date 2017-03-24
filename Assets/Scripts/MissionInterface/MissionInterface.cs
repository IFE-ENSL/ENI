using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Connexion;
using Assets.Scripts.Utility;
using UnityEngine.SceneManagement;

public class MissionInterface : MonoBehaviour {
    public GameObject FicheDescriptive;
    public GameObject planning;
    public GameObject autoEvaluation;
    public GameObject PlanningButtonPrefab;
    public GameObject PlanningGlobalLayout;
    public GameObject DialogBox;

    GameObject spider;
    JSONNode missionData;

    Text titre;
    Text description;
    Text objectif;
    Text KeyWords;

    private Waiter _waiter = new Waiter();

    CharacterSheetManager characterSheet;

    public GameObject ObjectifLinePrefab;

    List<ObjectifLine> displayedObjectives = new List<ObjectifLine>();
    Dictionary<int, int> uniqueENICompForDisplayedObjectives = new Dictionary<int, int>();

    bool InitSpiderNow = true;

    // Use this for initialization
    void Start ()
    {
        spider = GameObject.Find("SkillSpider");
        spider.SetActive(false);
        characterSheet = GameObject.FindObjectOfType<CharacterSheetManager>();
        titre = FicheDescriptive.transform.Find("Titre").GetComponentInChildren<Text>();
        description = FicheDescriptive.transform.Find("Description").GetComponentInChildren<Text>();
        objectif = FicheDescriptive.transform.Find("Objectif").GetComponentInChildren<Text>();
        KeyWords = FicheDescriptive.transform.Find("KeyWords").GetComponentInChildren<Text>();

        StartCoroutine(getMissionDatas(_waiter));

    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
	    if (InitSpiderNow && planning.activeInHierarchy)
        {
            GameObject.Find("SkillSpider").GetComponent<Spider_Skill_Displayer>().StartSpider();
            InitSpiderNow = false;
        }
	}

    void UpdateText ()
    {
        titre.text = missionData["slogan"].Value;
        description.text = missionData["descriptif"].Value;
        objectif.text = missionData["objectifs"].Value;
        KeyWords.text = missionData["keywords"].Value;
    }

    void SpawnObjectiveLine (int seanceNumber)
    {
        GameObject.Find("AlreadyValidatedText").GetComponent<Text>().enabled = false;

        bool alreadyValidatedOnce = false;
        int iterator = 0;
        foreach (JSONNode value in missionData["listeObjectifs"].Children)
        {
            if (value["seance"].AsInt == seanceNumber)
            {
                    if (value["point"].Value != "")
                {
                    GameObject.Find("AlreadyValidatedText").GetComponent<Text>().enabled = true;
                    alreadyValidatedOnce = true;
                }
            }
        }

        foreach (JSONNode value in missionData["listeObjectifs"].Children)
        {
            if (value["seance"].AsInt == seanceNumber)
            {
                Vector3 targetPosition = Vector3.zero;
                GameObject instantiatedLine = GameObject.Instantiate(ObjectifLinePrefab, targetPosition, Quaternion.identity) as GameObject;

                List<string> choiceList = new List<string>();
                choiceList.Add(value["TB"].Value);
                choiceList.Add(value["B"].Value);
                choiceList.Add(value["M"].Value);
                choiceList.Add(value["I"].Value);

                instantiatedLine.GetComponent<ObjectifLine>().SetObjectifLine(value["idUserObjMission"].AsInt, value["idObjMission"].AsInt, value["libelleObjMission"].Value, choiceList, value["point"].AsInt, alreadyValidatedOnce);

                foreach (JSONNode ENIComp in value["capacites"].Children)
                {
                    instantiatedLine.GetComponent<ObjectifLine>().linkedCompENI.Add(ENIComp.AsInt);
                }

                instantiatedLine.transform.SetParent(GameObject.Find("AllObjectives_GlobalLayout").transform);
                instantiatedLine.transform.localScale = Vector3.one;
                instantiatedLine.name = "ObjectiveLine" + iterator;

                displayedObjectives.Add(instantiatedLine.GetComponent<ObjectifLine>());
                iterator++;
            }
        }
    }

    //Used if we need to display a new autoEval page next
    void ClearAutoEvalInterface()
    {
        foreach (ObjectifLine line in displayedObjectives)
        {
            Destroy(line.gameObject);
        }

        displayedObjectives.Clear();
    }

    public void GoToPlanning (bool OnEvalValidation)
    {
        if (displayedObjectives.Count > 0)
            ClearAutoEvalInterface();

        spider.SetActive(true);
        FicheDescriptive.SetActive(false);
        autoEvaluation.SetActive(false);
        planning.SetActive(true);

        Transform[] planningButtons = PlanningGlobalLayout.GetComponentsInChildren<Transform>();

        foreach (Transform planningButton in planningButtons)
        {
            if (planningButton.name != "Planning_GlobalLayout")
                Destroy(planningButton.gameObject);
        }

        if (OnEvalValidation)
        {
            DialogBox.SetActive(true);
        }

        SpawnPlanning();
    }

    public void GoToAutoEval (int seanceNumber)
    {
        spider.SetActive(false);
        planning.transform.Find("ValidationText").GetComponent<Text>().enabled = false;
        planning.SetActive(false);
        autoEvaluation.SetActive(true);
        SpawnObjectiveLine(seanceNumber);
        GameObject.Find("TitreSeanceNumber").GetComponent<Text>().text = missionData["slogan"].Value + " - Séance " + seanceNumber;
    }

    public void GoToDescription ()
    {
        spider.SetActive(false);
        planning.transform.Find("ValidationText").GetComponent<Text>().enabled = false;
        planning.SetActive(false);
        FicheDescriptive.SetActive(true);
    }

    void SpawnPlanning ()
    {
        List<int> seanceNumbers = new List<int>();
        foreach (JSONNode value in missionData["listeObjectifs"].Children)
        {
            if (!seanceNumbers.Contains (value["seance"].AsInt))
            {
                seanceNumbers.Add(value["seance"].AsInt);
            }
        }

        Debug.Log("Finished first loop");

        foreach (int i in seanceNumbers)
        {
            GameObject instantiatedSeance = GameObject.Instantiate(PlanningButtonPrefab);
            instantiatedSeance.GetComponent<SeanceNumberForButton>().seanceNumber = i;
            instantiatedSeance.transform.name = "SeanceButton" + i;
            instantiatedSeance.GetComponentInChildren<Text>().text = "Seance " + (i);
            instantiatedSeance.transform.SetParent(PlanningGlobalLayout.transform);
            instantiatedSeance.transform.localScale = Vector3.one;
        }
    }

    public void ValidationObjectives ()
    {
        bool AllObjectivesActivated = true;
        foreach (ObjectifLine line in displayedObjectives)
        {
            if (!line.choseOneToggle) //if one of the objective is not toggled
            {
                Debug.LogWarning("All objectives should have at least one toggle selected!");
                GameObject.Find("ErrorText").GetComponent<Text>().enabled = true;
                GameObject.Find("ErrorText").GetComponent<Text>().text = "[!] Un ou plusieurs objectifs n'ont pas été renseignés avant validation, merci de vérifier que chaque objectif a été évalué.";
                GameObject.Find("ErrorText").GetComponent<Text>().color = Color.red;
                AllObjectivesActivated = false;
                break;
            }
            else
            {
                GameObject.Find("ErrorText").GetComponent<Text>().enabled = true;
                GameObject.Find("ErrorText").GetComponent<Text>().text = "Validation en cours...";
                GameObject.Find("ErrorText").GetComponent<Text>().color = Color.black;
                foreach (int CompENINumber in line.linkedCompENI)
                {
                    if (!uniqueENICompForDisplayedObjectives.ContainsKey(CompENINumber))
                        uniqueENICompForDisplayedObjectives.Add(CompENINumber, 0);

                    uniqueENICompForDisplayedObjectives[CompENINumber] = line._point;
                }

                StartCoroutine(SetPoint(line._idUserObjMission, line._point));
            }
        }

        if (AllObjectivesActivated)
        {
            foreach (KeyValuePair<int, int> keyValue in uniqueENICompForDisplayedObjectives)
            {
                StartCoroutine(characterSheet.PostPLayerStats(keyValue.Key, keyValue.Value));
            }

            StartCoroutine (getMissionUserStatsAtStart(_waiter));
            StartCoroutine(getMissionDatas(_waiter));
            GameObject.Find("ErrorText").GetComponent<Text>().enabled = false;
            autoEvaluation.SetActive(false);
            GoToPlanning(true);
        }
    }

    //When quitting the mission interface
    public void LoadMainBoard()
    {
        SceneManager.LoadScene("MainBoard");
    }

    //Updating the User skill points...
    public IEnumerator getMissionUserStatsAtStart(Waiter waiter)
    {
        yield return new WaitWhile(() => CharacterSheetManager.sendingDatas);
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
        CharacterSheetManager.PopulateCharacterSkills();
    }

   

    //Set point for the validated objectives
    public IEnumerator SetPoint (int idUserObjMission, int point)
    {
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        WWWForm hs_post = new WWWForm();
        hs_post.AddField("idUserObjMission", idUserObjMission);
        hs_post.AddField("point", point);
        hs_post.AddField("comments", "Ceci est un test. Bonjour Yvonnick. Ca va ? On se fait un petit café ?");

        WWW hs_get = new WWW(SQLCommonVars.baseURL + "/web/app.php/unity/missionPoint", hs_post.data, headers);
        yield return hs_get;

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

    //Get this specifif mission datas (Name, description, ...)
    public IEnumerator getMissionDatas(Waiter waiter)
    {
        yield return new WaitWhile(() => CharacterSheetManager.sendingDatas);
        Debug.Log("Attempting to retrieve the mission datas...");
        waiter.waiting = true;
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        string post_url = SQLCommonVars.getMissionDatasURL;
        WWWForm post_data = new WWWForm();
        post_data.AddField("idMission", GameObject.FindObjectOfType<PersistentFromSceneToScene>().missionId);
        WWW hs_get = new WWW(post_url, post_data.data, headers);
        yield return hs_get;
        if (hs_get.error != null)
        {
            Debug.Log("Error while retrieving all the Mission datas : " + hs_get.error);
            Debug.Log(hs_get.text);
            SceneManager.LoadScene(0);
        }
        waiter.data = hs_get.text;
        missionData = JSON.Parse(waiter.data);
        waiter.waiting = false;
        Debug.Log("Mission datas retrieved successfully =)");

        UpdateText();
    }

}
