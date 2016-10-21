using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Board_LaunchConfirmation : BoardManager {

    [HideInInspector]
    public string SceneToLoadNext;

    public void ConfirmLaunch ()
    {
        transform.parent.transform.FindChild("Loading").GetComponent<Text>().enabled = true;
        SceneManager.LoadSceneAsync(SceneToLoadNext);
    }

    public void UpdateContentName (string contentName)
    {
        Text ContentTextDisplay = transform.FindChild("ConfirmationText").GetComponent<Text>();
        string previousText = ContentTextDisplay.text;
        previousText = previousText.Replace("<Content>", contentName);
        ContentTextDisplay.text = previousText;
        Debug.Log(transform.Find("ConfirmationText").GetComponent<Text>().text);
    }

    public void CloseWindow ()
    {
        preventPlayerControl = false;
        gameObject.SetActive(false);
    }

    public void GiveBackControl ()
    {

    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	}
}
