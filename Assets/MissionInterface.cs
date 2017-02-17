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
    JSONNode missionData;

    Text titre;
    Text description;
    Text objectif;
    Text KeyWords;

    private Waiter _waiter = new Waiter();
    public const string baseURL = "http://vm-web7.ens-lyon.fr/eni"; //Prod
    private const string getMissionDatasURL = baseURL + "/web/app_dev.php/unity/management/initMission";
    Canvas canvas;
    CharacterSheetManager characterSheet;

    PersistentFromSceneToScene persistentData;

    public GameObject ObjectifLinePrefab;

    List<ObjectifLine> displayedObjectives = new List<ObjectifLine>();
    Dictionary<int, int> uniqueENICompForDisplayedObjectives = new Dictionary<int, int>();
    int seanceCount = 0;

    // Use this for initialization
    void Start ()
    {
        characterSheet = GameObject.FindObjectOfType<CharacterSheetManager>();
        titre = FicheDescriptive.transform.Find("Titre").GetComponentInChildren<Text>();
        description = FicheDescriptive.transform.Find("Description").GetComponentInChildren<Text>();
        objectif = FicheDescriptive.transform.Find("Objectif").GetComponentInChildren<Text>();
        KeyWords = FicheDescriptive.transform.Find("KeyWords").GetComponentInChildren<Text>();

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

        persistentData = GameObject.Find("PersistentSceneDatas").GetComponent<PersistentFromSceneToScene>();
        StartCoroutine(getMissionDatas(_waiter));
	}
	
	// Update is called once per frame
	void Update ()
    {
	
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
        int iterator = 0;
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

                instantiatedLine.GetComponent<ObjectifLine>().SetObjectifLine(value["idUserObjMission"].AsInt, value["idObjMission"].AsInt, value["libelleObjMission"].Value, choiceList, value["point"].AsInt);

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

    public void GoToPlanning ()
    {
        FicheDescriptive.SetActive(false);
        autoEvaluation.SetActive(false);
        planning.SetActive(true);
        SpawnPlanning();
    }

    public void GoToAutoEval (int seanceNumber)
    {
        planning.SetActive(false);
        autoEvaluation.SetActive(true);
        SpawnObjectiveLine(seanceNumber);
    }

    public void GoToDescription ()
    {
        planning.SetActive(false);
        FicheDescriptive.SetActive(true);
    }

    void SpawnPlanning ()
    {
        foreach (JSONNode value in missionData["listeObjectifs"].Children)
        {
            if (value["seance"].AsInt > seanceCount)
            {
                seanceCount = value["seance"].AsInt;
            }
        }

        Debug.Log("Finished first loop");

        for (int i = 0; i < seanceCount; i++)
        {
            GameObject instantiatedSeance = GameObject.Instantiate(PlanningButtonPrefab);
            int tempInt = i + 1;
            instantiatedSeance.GetComponent<Button>().onClick.AddListener(() => GoToAutoEval(tempInt));
            instantiatedSeance.transform.name = "SeanceButton" + i;
            instantiatedSeance.GetComponentInChildren<Text>().text = "Seance " + (i + 1);
            instantiatedSeance.transform.SetParent(planning.transform);
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

            autoEvaluation.SetActive(false);
            planning.SetActive(true);
        }
    }

    public void LoadMainBoard()
    {
        SceneManager.LoadScene("MainBoard");
    }

    public IEnumerator SetPoint (int idUserObjMission, int point)
    {
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        WWWForm hs_post = new WWWForm();
        hs_post.AddField("idUserObjMission", idUserObjMission);
        hs_post.AddField("point", point);

        WWW hs_get = new WWW(baseURL + "/web/app_dev.php/unity/missionPoint", hs_post.data, headers);
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
