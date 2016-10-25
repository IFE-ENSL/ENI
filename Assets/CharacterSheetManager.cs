using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSheetManager : MonoBehaviour {

    Camera SheetCamera;

    void Start ()
    {
        SheetCamera = transform.Find("CharacterSheetCamera").GetComponent<Camera>();
    }

    public void ToggleDisplaySheet () //Should deactivate characterMoves when clicking on a button, else, the pawn goes crazy...
    {
        if (SheetCamera.depth == -2)
        {
            SheetCamera.depth = 0;
        }
        else if (SheetCamera.depth == 0)
        {
            SheetCamera.depth = -2;
        }

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
