using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Assets.Scripts.Connexion;
using Assets.Scripts.Utility;
using UnityEngine.SceneManagement;

public class MissionInterface : MonoBehaviour {

    public int numeroSeance = 6;

    JSONNode missionData;

    Text titre;
    Text description;
    Text objectif;
    Text KeyWords;

    private Waiter _waiter = new Waiter();
    public const string baseURL = "http://vm-web7.ens-lyon.fr/eni"; //Prod
    private const string getMissionDatasURL = baseURL + "/web/app_dev.php/unity/management/initMission";
    Canvas canvas;

    PersistentFromSceneToScene persistentData;

    public GameObject ObjectifLinePrefab;

    // Use this for initialization
    void Start ()
    {
        titre = transform.Find("Titre").GetComponentInChildren<Text>();
        description = transform.Find("Description").GetComponentInChildren<Text>();
        objectif = transform.Find("Objectif").GetComponentInChildren<Text>();
        KeyWords = transform.Find("KeyWords").GetComponentInChildren<Text>();

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
        GameObject previouslySpawnedLine = null;
        foreach (JSONNode value in missionData["listeObjectifs"].Children)
        {
            if (value["seance"].AsInt == seanceNumber)
            {
                Vector3 targetPosition = Vector3.zero;

                /*if (iterator > 0)
                    targetPosition = new Vector3(previouslySpawnedLine.transform.localPosition.x,
                                                    previouslySpawnedLine.transform.localPosition.y - (previouslySpawnedLine.GetComponentInChildren<Text>().rectTransform.rect.height),
                                                    previouslySpawnedLine.transform.localPosition.z);
                */

                GameObject instantiatedLine = GameObject.Instantiate(ObjectifLinePrefab, targetPosition, Quaternion.identity) as GameObject;


                List<string> choiceList = new List<string>();
                choiceList.Add(value["TB"].Value);
                choiceList.Add(value["B"].Value);
                choiceList.Add(value["M"].Value);
                choiceList.Add(value["I"].Value);

                instantiatedLine.GetComponent<ObjectifLine>().SetObjectifLine(value["idUserObjMission"].AsInt, value["idObjMission"].AsInt, value["libelleObjMission"].Value, choiceList, value["point"].AsInt);

                instantiatedLine.transform.SetParent(GameObject.Find("GlobalLayoutTest").transform);
                instantiatedLine.transform.localScale = Vector3.one;
                instantiatedLine.name = "ObjectiveLine" + iterator;

                previouslySpawnedLine = instantiatedLine;

                iterator++;
            }
        }

        //GameObject.Find ("AllObjectiveLines").GetComponent<RectTransform>().off

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
        SpawnObjectiveLine(numeroSeance);
    }

}
