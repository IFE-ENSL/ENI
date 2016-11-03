using UnityEngine;
using System.Collections;

public class PersistenFromSceneToScene : MonoBehaviour
{

    public static PersistenFromSceneToScene Instance;

    public int[] competenceAmount;



	// Use this for initialization
	void Awake ()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}
