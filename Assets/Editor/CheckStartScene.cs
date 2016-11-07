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

    private static void CheckScene()
    {
        Debug.Log("Changed play state");

        if(SceneManager.GetActiveScene().name != "Login" && EditorApplication.isPlaying)
        {
            SceneManager.LoadScene("Login");
            Debug.ClearDeveloperConsole();
        }
    }
}
