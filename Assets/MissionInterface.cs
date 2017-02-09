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

    PersistentFromSceneToScene persistentData;

    public GameObject ObjectifLinePrefab;

    // Use this for initialization
    void Start ()
    {
        titre = transform.Find("Titre").GetComponentInChildren<Text>();
        description = transform.Find("Description").GetComponentInChildren<Text>();
        objectif = transform.Find("Objectif").GetComponentInChildren<Text>();
        KeyWords = transform.Find("KeyWords").GetComponentInChildren<Text>();


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
        foreach (JSONNode value in missionData["listeObjectifs"].Children)
        {
            if (value["seance"].AsInt == seanceNumber)
            {
                GameObject instatiatedLine = GameObject.Instantiate(ObjectifLinePrefab);
                instatiatedLine.GetComponent<ObjectifLine>().SetObjectifLine(value["idUserObjMission"].AsInt, value["idObjMission"].AsInt, value["libelleObjMission"].Value, value["TB"].Value, value["B"].Value, value["M"].Value, value["I"].Value, value["point"].AsInt);
            }
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
        SpawnObjectiveLine(numeroSeance); //TODO : I was here.
    }

}
