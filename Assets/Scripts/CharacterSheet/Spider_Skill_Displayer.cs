using UnityEngine;
using System.Collections;
using System;

public class Spider_Skill_Displayer : MonoBehaviour {

    //TO DO: Factorize and clean this code before fixing the dynamic updating of the spider. It's a mess here, phew !

    public int[] CompetenceAmount;
    Transform firstBranch;
    public GameObject CompetencePrefab;
    public GameObject SpiderWebWirePrefab;
    public float branchesSize = 20;
    int greatestSkillValue = 0;
    public float spiderThickness = .1f;

    LineRenderer[] spawnedBranches;
    LineRenderer[] spawnedLines;

	// Use this for initialization
	void Start ()
    {
        spawnedLines = new LineRenderer[CompetenceAmount.Length];
        spawnedBranches = new LineRenderer[CompetenceAmount.Length];

        //Let's get the greatest skill value first
        foreach (int skillPoint in CompetenceAmount)
        {
            if (skillPoint > greatestSkillValue)
                greatestSkillValue = skillPoint;
        }

        //Force the Competence amount to be of at least 4 different skills.
	    if (CompetenceAmount.Length < 4)
        {
            Array.Resize(ref CompetenceAmount, 4);
        }

        InitializeSpider();
	}

    void InitializeSpider ()
    {
        float BranchAngle = 360 / CompetenceAmount.Length;
        float addAngle = BranchAngle;

        Vector3[] newPositions = new Vector3[2];

        Vector3 firstBranchPosition = Vector3.zero;
        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        for (int i = 0; i < CompetenceAmount.Length; i++)
        {
            //Spawn a branch, parent it to the spider object, then rename it for clarity's sake.
            GameObject spawnedCompetence = GameObject.Instantiate(CompetencePrefab, transform.position, Quaternion.identity) as GameObject;
            spawnedCompetence.transform.SetParent(transform);
            spawnedCompetence.transform.name = "Competence" + i;

            //Rotate the vector to make the branch face the good position, then store the position in the array for later use.
            Vector3 newRotatedVector = new Vector3(0, branchesSize, 0);
            newRotatedVector = Quaternion.AngleAxis(-addAngle, Vector3.forward) * newRotatedVector;
            newPositions[0] = transform.position;
            newPositions[1] = newRotatedVector + transform.position;

            //Then we apply the position to the branches' line renderers.
            LineRenderer CompetenceLine = spawnedCompetence.GetComponent<LineRenderer>();
            CompetenceLine.SetPositions(newPositions);
            CompetenceLine.SetWidth(spiderThickness, spiderThickness);

            // Debug.DrawLine(transform.position, transform.position + newRotatedVector, Color.red, Mathf.Infinity);

            //Set the position of the skill point, it should be on the associated branch
            float percentageValue = (float)CompetenceAmount[i] / (float)greatestSkillValue * branchesSize;
            currentSkillPosition = new Vector3(0, percentageValue, 0);
            currentSkillPosition = Quaternion.AngleAxis(-addAngle, Vector3.forward) * currentSkillPosition;

            // Debug.DrawLine(transform.position, transform.position + currentSkillPosition, Color.red, Mathf.Infinity);





            spawnedBranches[i] = CompetenceLine;

            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            addAngle -= BranchAngle;

            if (i != 0)
            {
                spawnedLines[i - 1] = SpawnWebWire(previousSkillPosition, currentSkillPosition, i - 1);
            }

            previousSkillPosition = currentSkillPosition;
        }

        spawnedLines[CompetenceAmount.Length - 1] = SpawnWebWire(currentSkillPosition, firstBranchPosition, CompetenceAmount.Length - 1);
    }

    LineRenderer SpawnWebWire (Vector3 previousLineTip, Vector3 newPositions, int spawnNumber)
    {
        GameObject spawnedWebWire = GameObject.Instantiate(SpiderWebWirePrefab, transform.position, Quaternion.identity) as GameObject;
        LineRenderer spawnedWebWireRenderer = spawnedWebWire.GetComponent<LineRenderer>();
        spawnedWebWire.name = "Spider_StatWire_" + spawnNumber;

        UpdateWebWirePositions(spawnedWebWireRenderer, previousLineTip, newPositions);

        return spawnedWebWire.GetComponent<LineRenderer>();
    }

    void UpdateWebWirePositions (LineRenderer line, Vector3 previousLineTip, Vector3 newPositions)
    {
        Vector3[] webWirePos = new Vector3[3];
        webWirePos[0] = previousLineTip + transform.position;
        webWirePos[1] = transform.position;
        webWirePos[2] = newPositions + transform.position;

        line.GetComponent<CustomizeLineRenderer>().linePositions = webWirePos;
        line.GetComponent<LineRenderer>().SetWidth(spiderThickness, spiderThickness);
        //spawnedWebWire.GetComponent<CustomizeLineRenderer>().RoughCurve(); //Use this instead of SmoothCurve() to get a clearer view of the stats shaping the web line, ONLY FOR DEBUG.
        line.GetComponent<CustomizeLineRenderer>().SmoothCurve();
    }

    void UpdateSpider ()
    {
        Vector3[] newPositions = new Vector3[2];

        float BranchAngle = 360 / CompetenceAmount.Length;
        float addAngle = BranchAngle;

        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        for (int i= 0; i < CompetenceAmount.Length; i++)
        {
            Vector3 newRotatedVector = new Vector3(0, branchesSize, 0);
            newRotatedVector = Quaternion.AngleAxis(-addAngle, Vector3.forward) * newRotatedVector;

            newPositions[0] = transform.position;
            newPositions[1] = newRotatedVector + transform.position;

            LineRenderer CompetenceLine = spawnedBranches[i].GetComponent<LineRenderer>();
            CompetenceLine.SetPositions(newPositions);
            CompetenceLine.SetWidth(spiderThickness, spiderThickness);

            //Set the position of the skill point, it should be on the associated branch
            float percentageValue = (float)CompetenceAmount[i] / (float)greatestSkillValue * branchesSize;
            currentSkillPosition = new Vector3(0, percentageValue, 0);
            currentSkillPosition = Quaternion.AngleAxis(-addAngle, Vector3.forward) * currentSkillPosition;

            UpdateWebWirePositions(spawnedLines[i], previousSkillPosition, currentSkillPosition);
            /*


            if (i != 0)
            {
                spawnedLines[i - 1].SetVertexCount(3);
                spawnedLines[i - 1].GetComponent<CustomizeLineRenderer>().linePositions = new Vector3[3];
            }*/

            addAngle -= BranchAngle;

            previousSkillPosition = currentSkillPosition;
        } //OK clean all of this up and don't forget to generate the last web line. Oh, and the first one is glitched out, take example in the start function to avoid this.

        /*foreach (LineRenderer line in spawnedLines)
        {
            line.SetVertexCount(3);
            //Debug.Log(line.transform.name + " should now only have 3 vertices");
            Vector3[] newPos = new Vector3[3];
            
            for (int i = 0; i < 3; i ++)
            {
                newPos[i] = Vector3.zero;
            }

            line.SetPositions(newPos);
            line.GetComponent<CustomizeLineRenderer>().linePositions = newPos;
            line.GetComponent<CustomizeLineRenderer>().SmoothCurve();
        }*/ //This was the first experimentation
    }

    void Update ()
    {
        //UpdateSpider();
    }
}
