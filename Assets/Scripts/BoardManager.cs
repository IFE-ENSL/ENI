﻿using UnityEngine;
using System.Collections;
using Assets.Scripts.Connexion;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Assets.Scripts.Utility;

public class BoardManager : MonoBehaviour {

    public static bool preventPlayerControl = false;

    GameObject[] Zones;
    ConnexionController connexionController;
    private Waiter _waiter = new Waiter();
    //public const string baseURL = "http://vm-web7.ens-lyon.fr/eni"; //Prod
    public const string baseURL = "http://127.0.0.1/eni"; //Local
    private const string getUserStats = baseURL + "/web/app_dev.php/unity/management/initJeu";

    public List<string> ActivitiesNames = new List<string>();

    JSONNode userStats;

    void Start ()
    {
        connexionController = GameObject.Find("ConnexionController").GetComponent<ConnexionController>();
        Zones = GameObject.FindGameObjectsWithTag("BoardZone");

        StartCoroutine(getUserStatsAtLogin(_waiter));
    }

    void PopulateMainBoard ()
    {
        foreach (GameObject zone in Zones)
        {
            BoardStep[] steps = zone.GetComponentsInChildren<BoardStep>();

            int gameListIterator = 0;
            foreach (JSONNode value in userStats["listeJeux"].Children)
            {
                if (zone.name == "Zone" + value["idZone"].Value)
                {
                    steps[gameListIterator].SceneToLoad = value["jeuNom"].Value;
                    //TODO : Prevent the case where there's more MiniGames in this zone that there is of steps.
                    Debug.Log(steps[gameListIterator].SceneToLoad);
                    gameListIterator++;
                }
            }
        }
    }

    public IEnumerator getUserStatsAtLogin(Waiter waiter)
    {
        Debug.Log("Attempting to retrieve the player's stats...");
        waiter.waiting = true;
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        string post_url = getUserStats;
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

        foreach (JSONNode activityName in userStats["listeJeux"].Children)
        {
            ActivitiesNames.Add(activityName["jeuNom"].Value);
        }

        waiter.waiting = false;
        Debug.Log("Player stats retrieved successfully =)");

        PopulateMainBoard();
    }

}
