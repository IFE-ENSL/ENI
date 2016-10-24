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
    public float spiderThickness = .1f;

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
            GameObject spawnedCompetence = GameObject.Instantiate(CompetencePrefab, transform.position, Quaternion.identity) as GameObject;
            spawnedCompetence.transform.SetParent(transform);
            spawnedCompetence.transform.name = "Competence" + i;

            Vector3 newRotatedVector = new Vector3(0, branchesSize, 0);
            newRotatedVector = Quaternion.AngleAxis(-addAngle, Vector3.forward) * newRotatedVector;

            Debug.DrawLine(transform.position, transform.position + newRotatedVector, Color.red, Mathf.Infinity);

            //Set the position of the skill point, it should be on the associated branch
            float percentageValue =  (float)CompetenceAmount[i] / (float)greatestSkillValue * branchesSize;
            currentSkillPosition = new Vector3(0, percentageValue, 0);



            currentSkillPosition = Quaternion.AngleAxis(-addAngle, Vector3.forward) * currentSkillPosition;

           // Debug.DrawLine(transform.position, transform.position + currentSkillPosition, Color.red, Mathf.Infinity);

            newPositions[0] = transform.position;
            newPositions[1] = newRotatedVector + transform.position;

            LineRenderer CompetenceLine = spawnedCompetence.GetComponent<LineRenderer>();
            CompetenceLine.SetPositions(newPositions);
            CompetenceLine.SetWidth(spiderThickness, spiderThickness);

            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            addAngle -= BranchAngle;

            SpawnWebWire(previousSkillPosition, currentSkillPosition); //Possibly because of previousSkillPosition, the first and last lines are fucked up. Fix it.

            previousSkillPosition = currentSkillPosition;
        }

        SpawnWebWire(currentSkillPosition, firstBranchPosition);
	}

    void SpawnWebWire (Vector3 previousLineTip, Vector3 newPositions)
    {
        GameObject spawnedWebWire = GameObject.Instantiate(SpiderWebWirePrefab, transform.position, Quaternion.identity) as GameObject;

        Vector3[] webWirePos = new Vector3[3];
        webWirePos[0] = previousLineTip + transform.position;
        webWirePos[1] = transform.position;

        webWirePos[2] = newPositions + transform.position;

        spawnedWebWire.GetComponent<CustomizeLineRenderer>().linePositions = webWirePos;
        spawnedWebWire.GetComponent<LineRenderer>().SetWidth(spiderThickness, spiderThickness);
        //spawnedWebWire.GetComponent<CustomizeLineRenderer>().RoughCurve();
        spawnedWebWire.GetComponent<CustomizeLineRenderer>().SmoothCurve();
    }
}
