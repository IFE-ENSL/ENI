using UnityEngine;
using System.Collections;

public class QuitMiniGame : MonoBehaviour {

    public GameObject ConfirmationWindow;

	// Update is called once per frame
	public void Quit ()
    {
        ConfirmationWindow confirmationWindowScript = ConfirmationWindow.GetComponent<ConfirmationWindow>();
        confirmationWindowScript.UpdateContentName(true, "Veux-tu vraiment quitter ? Ta progression sera perdue !");
        ConfirmationWindow.SetActive(true);
    }
}
