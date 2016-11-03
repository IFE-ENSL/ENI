using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSheetManager : MonoBehaviour {

    Camera SheetCamera;
    GameObject gameCanvas;

    public int[] CompetenceAmount;

    void Start ()
    {
        SheetCamera = transform.Find("CharacterSheetCamera").GetComponent<Camera>();
        gameCanvas = GameObject.Find("GameUI");
    }

    public void ToggleDisplaySheet ()
    {
        if (gameCanvas != null)
        {
            if (gameCanvas.activeSelf)
                gameCanvas.SetActive(false);
            else
                gameCanvas.SetActive(true);
        }

        if (SheetCamera.depth == -2)
        {
            SheetCamera.depth = 0;
        }
        else if (SheetCamera.depth == 0)
        {
            SheetCamera.depth = -2;
        }

        Debug.Log("Toggled Player Sheet");

        //Let's add some controls constraints according to the scene we're in
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "MainBoard")
            ToggleMainBoardConstraints();
    }

    void ToggleMainBoardConstraints()
    {
        if (BoardManager.preventPlayerControl == true)
            BoardManager.preventPlayerControl = false;
        else if (BoardManager.preventPlayerControl == false)
            BoardManager.preventPlayerControl = true;
    }

}
