using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentFromSceneToScene : MonoBehaviour
{

    public static PersistentFromSceneToScene DataPersistenceInstance;
    public List<Competences> listeCompetences;

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
