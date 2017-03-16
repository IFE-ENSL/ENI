using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ConfirmationWindow : MonoBehaviour {

    [HideInInspector]
    public string SceneToLoadNext;
    public string defaultText = "Veux-tu lancer <Content> ?";
    Text ContentTextDisplay;

    void Start ()
    {
        ContentTextDisplay = transform.FindChild("ConfirmationText").GetComponent<Text>();
        ContentTextDisplay.text = defaultText;
        gameObject.SetActive(false); //Hide the confirmation window
    }    
      
    //Display the loading text in the corner and start loading the correct scene when a launch is confirmed
    //Make sure the scene field is correctly set up in the inspector in order to load a new scene with the confirmation button      
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



    //Display the correct content name for the launch confirmation of a mini-game or mission
    public void UpdateContentName(string contentName)
    {
            string previousText = ContentTextDisplay.text;
            previousText = previousText.Replace("<Content>", contentName);
            ContentTextDisplay.text = previousText;
            Debug.Log(transform.Find("ConfirmationText").GetComponent<Text>().text);
    }

    //This version of UpdateContentName replace the whole text instead of just the "<Content>" string when replaceAll is set to true
    public void UpdateContentName(bool replaceAll, string contentName)
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
