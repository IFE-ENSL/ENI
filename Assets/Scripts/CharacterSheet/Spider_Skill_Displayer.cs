using UnityEngine;
using System.Collections;
using System;

public class Spider_Skill_Displayer : MonoBehaviour {

    public int[] CompetenceAmount;
    Transform firstBranch;
    public GameObject CompetencePrefab;
    public GameObject SpiderWebWirePrefab;
    public float branchesSize = 20;
    int greatestSkillValue = 0;

	// Use this for initialization
	void Start ()
    {
        //Let's get the greatest skill value first
        foreach (int skillPoint in CompetenceAmount)
        {
            if (skillPoint > greatestSkillValue)
                greatestSkillValue = skillPoint;
        }


	    if (CompetenceAmount.Length < 4)
        {
            Array.Resize(ref CompetenceAmount, 4);
        }

        Vector3 firstBranchPosition = Vector3.zero;
        float BranchAngle = 360 / CompetenceAmount.Length;
        float addAngle = BranchAngle;

        Vector3[] newPositions = new Vector3[2];

        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        for (int i = 0; i < CompetenceAmount.Length; i++)
        {
            GameObject spawnedCompetence = GameObject.Instantiate(CompetencePrefab, Vector3.zero, Quaternion.identity) as GameObject;
            spawnedCompetence.transform.SetParent(transform);
            spawnedCompetence.transform.name = "Competence" + i;

            Vector3 newRotatedVector = new Vector3(0, branchesSize, 0);
            newRotatedVector = Quaternion.AngleAxis(-addAngle, Vector3.forward) * newRotatedVector;

            //Set the position of the skill point, it should be on the associated branch
            float percentageValue =  (float)CompetenceAmount[i] / (float)greatestSkillValue * branchesSize;
            currentSkillPosition = new Vector3(0, percentageValue, 0);



            currentSkillPosition = Quaternion.AngleAxis(-addAngle, Vector3.forward) * currentSkillPosition;

            Debug.DrawLine(Vector3.zero, currentSkillPosition, Color.red, Mathf.Infinity);

            newPositions[0] = Vector3.zero;
            newPositions[1] = newRotatedVector;

            LineRenderer CompetenceLine = spawnedCompetence.GetComponent<LineRenderer>();
            CompetenceLine.SetPositions(newPositions);

            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            addAngle -= BranchAngle;

            SpawnWebWire(previousSkillPosition, currentSkillPosition);

            previousSkillPosition = currentSkillPosition;
        }

        SpawnWebWire(currentSkillPosition, firstBranchPosition);
	}

    void SpawnWebWire (Vector3 previousLineTip, Vector3 newPositions)
    {
        GameObject spawnedWebWire = GameObject.Instantiate(SpiderWebWirePrefab, Vector3.zero, Quaternion.identity) as GameObject;

        Vector3[] webWirePos = new Vector3[3];
        webWirePos[0] = previousLineTip;
        webWirePos[1] = Vector3.zero;

        webWirePos[2] = newPositions;

        spawnedWebWire.GetComponent<CustomizeLineRenderer>().linePositions = webWirePos;
        //spawnedWebWire.GetComponent<CustomizeLineRenderer>().RoughCurve();
        spawnedWebWire.GetComponent<CustomizeLineRenderer>().SmoothCurve();
    }
	
	// Update is called once per frame
	void Update ()
    {
	    
	}
}
