using UnityEngine;
using System.Collections;

public class Spider_Skill_Displayer : MonoBehaviour {

    public int BranchNumbers;
    Transform firstBranch;
    public GameObject CompetencePrefab;
    public GameObject SpiderWebWirePrefab;

	// Use this for initialization
	void Start ()
    {
        firstBranch = transform.FindChild("Competence");


	    if (BranchNumbers < 4)
        {
            BranchNumbers = 4;
        }

        float BranchAngle = 360 / BranchNumbers;
        float addAngle = BranchAngle;

        Vector3 previousLineTip = Vector3.zero;

        for (int i = 0; i < BranchNumbers; i++)
        {
            GameObject spawnedCompetence = GameObject.Instantiate(CompetencePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            spawnedCompetence.transform.SetParent(transform);
            spawnedCompetence.transform.name = "Competence" + i;

            Vector3 newRotatedVector = new Vector3(0, 20, 0);
            newRotatedVector = Quaternion.AngleAxis(-addAngle, Vector3.forward) * newRotatedVector;

            Vector3[] newPositions = new Vector3[2];
            newPositions[0] = Vector3.zero;
            newPositions[1] = newRotatedVector;

            LineRenderer CompetenceLine = spawnedCompetence.GetComponent<LineRenderer>();
            CompetenceLine.SetPositions(newPositions);

            addAngle -= BranchAngle;
            
            if (Mathf.IsPowerOfTwo (i+1))
            {
                Debug.Log("Yup, peer number spawned! Hooray, let's celebrate!");
                GameObject spawnedWebWire = GameObject.Instantiate(SpiderWebWirePrefab, Vector3.zero, Quaternion.identity) as GameObject;

                Vector3[] webWirePos = new Vector3[3];
                webWirePos[0] = previousLineTip;
                webWirePos[1] = Vector3.zero;
                webWirePos[2] = newPositions[1];

                spawnedWebWire.GetComponent<CustomizeLineRenderer>().linePositions = webWirePos;
                spawnedWebWire.GetComponent<CustomizeLineRenderer>().SmoothCurve();
                //spawnedWebWire.GetComponent<LineRenderer>().SetPositions(webWirePos);
            }

            previousLineTip = newPositions[1];
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
