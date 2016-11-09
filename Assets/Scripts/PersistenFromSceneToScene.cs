using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistenFromSceneToScene : MonoBehaviour
{

    public static PersistenFromSceneToScene DataPersistenceInstance;


    public List<int> competenceAmount;
    public List<QualityList> masterQualityList;

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
