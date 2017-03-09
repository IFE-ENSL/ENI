using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

//Classe utilisée afin d'afficher la scene pendant 10s
public class NextLevel : MonoBehaviour {


    void Start()
    {
        StartCoroutine(waitBeforeLeave());
    }

    private IEnumerator waitBeforeLeave()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("MainBoard");
    }
}
