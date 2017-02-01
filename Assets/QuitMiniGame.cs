using UnityEngine;
using System.Collections;

public class QuitMiniGame : MonoBehaviour {

    public GameObject ConfirmationWindow;

	// Update is called once per frame
	public void Quit ()
    {
        Board_LaunchConfirmation confirmationWindowScript = ConfirmationWindow.GetComponent<Board_LaunchConfirmation>();
        confirmationWindowScript.UpdateContentName(true, "Veux-tu vraiment quitter le mini-jeu et revenir au plateau ? Tu devras recommencer une partie la prochaine fois que tu rejoue à ce mni-jeu.");
        ConfirmationWindow.SetActive(true);
    }
}
