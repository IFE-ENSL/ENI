using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public class CheckStartScene {

    static CheckStartScene ()
    {
        Debug.Log("Scene check listening...");
        EditorApplication.playmodeStateChanged += CheckScene;
    }

    [MenuItem("Custom Tools/ Toggle force Login before entering PlayMode")]
    public static void ToggleCheckScene ()
    {
        if (EditorPrefs.GetBool ("enableCheckStartScene") )
        {
            EditorPrefs.SetBool ("enableCheckStartScene", false);
            Debug.Log("Force Login now deactivated");
        }
        else
        {
            EditorPrefs.SetBool("enableCheckStartScene", true);
            Debug.Log("Force Login now activated");
        }
    }

    private static void CheckScene()
    {
        Debug.Log("Changed play state");

        if(EditorPrefs.GetBool("enableCheckStartScene") && SceneManager.GetActiveScene().name != "Login" && EditorApplication.isPlaying)
        {
            SceneManager.LoadScene("Login");
            Debug.ClearDeveloperConsole();
        }
    }
}
