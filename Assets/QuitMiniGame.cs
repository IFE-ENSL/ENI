using UnityEngine;
using System.Collections;

public class QuitMiniGame : MonoBehaviour {

    public GameObject ConfirmationWindow;

	// Update is called once per frame
	public void Quit ()
    {
        Board_LaunchConfirmation confirmationWindowScript = ConfirmationWindow.GetComponent<Board_LaunchConfirmation>();
        confirmationWindowScript.UpdateContentName(true, "Veux-tu vraiment quitter ? Ta progression sera perdue !");
        ConfirmationWindow.SetActive(true);
    }
}
