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
    public const string baseURL = "http://vm-web7.ens-lyon.fr/eni"; //Prod
    private const string getMissionDatasURL = baseURL + "/web/app.php/unity/management/initMission";
    Canvas canvas;
    CharacterSheetManager characterSheet;

    PersistentFromSceneToScene persistentData;

    public GameObject ObjectifLinePrefab;

    List<ObjectifLine> displayedObjectives = new List<ObjectifLine>();
    Dictionary<int, int> uniqueENICompForDisplayedObjectives = new Dictionary<int, int>();
    int seanceCount = 0;

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

        canvas = GameObject.Find("MissionCanvas").GetComponent<Canvas>();

        persistentData = GameObject.Find("PersistentSceneDatas").GetComponent<PersistentFromSceneToScene>();

        StartCoroutine(getMissionDatas(_waiter));

    }
	
	// Update is called once per frame
	void LateUpdate ()
    {
        if(_waiter.waiting)
        {
            Debug.Log("Iiiiii'm waitiiiiing");
        }

	    if (InitSpiderNow)
        {
            GameObject.Find("SkillSpider").GetComponent<Spider_Skill_Displayer>().InitSpider();
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
                    if (value["point"].AsInt > 0)
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
            instantiatedSeance.GetComponent<Button>().onClick.AddListener(() => GoToAutoEval(i));
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
            if (!line.choseOneToggle)
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

            StartCoroutine (getUserStatsAtLogin(_waiter));
            StartCoroutine(getMissionDatas(_waiter));
            GameObject.Find("ErrorText").GetComponent<Text>().enabled = false;
            autoEvaluation.SetActive(false);
            GoToPlanning(true);
        }
    }

    public void LoadMainBoard()
    {
        SceneManager.LoadScene("MainBoard");
    }

    JSONNode userStats;

    public IEnumerator getUserStatsAtLogin(Waiter waiter)
    {
        yield return new WaitWhile(() => CharacterSheetManager.sendingDatas);
        Debug.Log("Attempting to retrieve the player's stats...");
        waiter.waiting = true;
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        string post_url = "http://vm-web7.ens-lyon.fr/eni/web/app.php/unity/management/initJeu";


        WWW hs_get = new WWW(post_url, null, headers);
        yield return hs_get;
        if (hs_get.error != null)
        {
            Debug.Log("Error while retrieving all the player's stats : " + hs_get.error);
            Debug.Log(hs_get.text);
            SceneManager.LoadScene(0);
        }
        waiter.data = hs_get.text;
        userStats = JSON.Parse(waiter.data);

        waiter.waiting = false;
        Debug.Log("Player stats retrieved successfully =)");
        PopulateCharacterSkills();
    }

    void PopulateCharacterSkills()
    {

        CharacterSheetManager charSheet = GameObject.Find("CharacterSheet").GetComponent<CharacterSheetManager>();

        charSheet.competencesList.Clear();
        //charSheet.correspondenceUserCompENIAndMiniGame.Clear();

        Dictionary<int, string> SkillTags = new Dictionary<int, string>();

        foreach (JSONNode value in userStats["listeCompetences"].Children)
        {
            if (!SkillTags.ContainsKey(value["idCompGen"].AsInt))
                SkillTags.Add(value["idCompGen"].AsInt, value["LibCompGen"].Value);
        }

        foreach (JSONNode value in userStats["listeCriteres"].Children)
        {
            charSheet.competencesList.Add(value["idCompEni"].AsInt, new CompetenceENI(SkillTags[value["idCompGen"].AsInt], value["idCompGen"].AsInt, value["point"].AsInt, value["idCritere"].AsInt, value["idJM"].AsInt)); //TODO : Replace RandomName by the real skill name
            //charSheet.correspondenceUserCompENIAndMiniGame.Add(value["idCompENI"].AsInt, value["idJM"].AsInt);
        }

        foreach (JSONNode value in userStats["listeJeux"].Children)
        {
            if (value["jeuNom"].Value == "mini-jeu 01")
                CharacterSheetManager.game1ID = value["idJeu"].AsInt;
            else if (value["jeuNom"].Value == "mini-jeu 02")
                CharacterSheetManager.game2ID = value["idJeu"].AsInt;
        }

        GameObject.Find("SkillSpider").GetComponent<Spider_Skill_Displayer>().UpdateGeneralSkillPoints();
    }


    public IEnumerator SetPoint (int idUserObjMission, int point)
    {
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        WWWForm hs_post = new WWWForm();
        hs_post.AddField("idUserObjMission", idUserObjMission);
        hs_post.AddField("point", point);
        hs_post.AddField("comments", "Ceci est un test. Bonjour Yvonnick. Ca va ? On se fait un petit café ?");

        WWW hs_get = new WWW(baseURL + "/web/app.php/unity/missionPoint", hs_post.data, headers);
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

    public IEnumerator getMissionDatas(Waiter waiter)
    {
        yield return new WaitWhile(() => CharacterSheetManager.sendingDatas);
        Debug.Log("Attempting to retrieve the mission datas...");
        waiter.waiting = true;
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        string post_url = getMissionDatasURL;
        WWWForm post_data = new WWWForm();
        post_data.AddField("idMission", 6);
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
