using UnityEngine;
using System.Collections;

public class PersistenFromSceneToScene : MonoBehaviour
{

    public static PersistenFromSceneToScene DataPersistenceInstance;


    public int[] competenceAmount;



	// Use this for initialization
	void Awake ()
    {
        if (DataPersistenceInstance == null)
        {
            DontDestroyOnLoad(gameObject);
            DataPersistenceInstance = this;
        }
        else if (DataPersistenceInstance != this)
        {
            Destroy(gameObject);
        }
    }
}
