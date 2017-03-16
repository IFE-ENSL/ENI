using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentFromSceneToScene : MonoBehaviour
{

    public static PersistentFromSceneToScene DataPersistenceInstance;
    public Dictionary<int, CompetenceENI> listeCompetences = new Dictionary<int, CompetenceENI>();

    public int alternativeSceneId = 0; //Checked by some scenes to load a different layout when called (E.G. The Management MiniGame's scene)
    public int missionId = 0; //Checked by the mission scene in order to know what mission to load

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
