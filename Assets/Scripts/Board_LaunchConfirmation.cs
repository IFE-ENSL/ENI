using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Board_LaunchConfirmation : MonoBehaviour {

    [HideInInspector]
    public string SceneToLoadNext;
    public string defaultText = "Veux-tu lancer <Content> ?";
    Text ContentTextDisplay;

    void Start ()
    {
        ContentTextDisplay = transform.FindChild("ConfirmationText").GetComponent<Text>();
        ContentTextDisplay.text = defaultText;
        gameObject.SetActive(false);
    }    
            
    public void ConfirmLaunch ()
    {
        transform.parent.transform.FindChild("Loading").GetComponent<Text>().enabled = true;

        if (SceneToLoadNext != "")
            SceneManager.LoadSceneAsync(SceneToLoadNext);
        else
        {
            Debug.LogError("The scene field is empty. No scene to load.");
            transform.parent.transform.FindChild("Loading").GetComponent<Text>().enabled = false;
        }
    }

    public void UpdateContentName (bool replaceAll, string contentName)
    {
        if (!replaceAll)
        {
            UpdateContentName(contentName);
        }
        else
        {
            ContentTextDisplay.text = contentName;
        }
    }

    public void UpdateContentName(string contentName)
    {
            string previousText = ContentTextDisplay.text;
            previousText = previousText.Replace("<Content>", contentName);
            ContentTextDisplay.text = previousText;
            Debug.Log(transform.Find("ConfirmationText").GetComponent<Text>().text);
    }

    public void CloseWindow ()
    {
        BoardManager.preventPlayerControl = false;
        Text ContentTextDisplay = transform.FindChild("ConfirmationText").GetComponent<Text>();
        ContentTextDisplay.text = defaultText;
        gameObject.SetActive(false);
    }

    public void QuitMiniGame ()
    {
            SceneManager.LoadSceneAsync("MainBoard");
    }
}
