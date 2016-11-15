using UnityEngine;
using System.Collections;
using System;

public class Spider_Skill_Displayer : MonoBehaviour {

    CharacterSheetManager characterSheet;
    public static float[] staticSkillAmount;
    Transform firstBranch;
    public GameObject CompetencePrefab;
    public GameObject SpiderWebWirePrefab;
    public float branchesSize = 20;
    float greatestSkillValue = 0;
    public float spiderThickness = .1f;

    public GameObject tagPrefab;

    LineRenderer[] spawnedBranches;
    LineRenderer[] spawnedLines;

    Vector3[] RegisteredBranchTopPositions;
    GameObject[] spawnedTags;

    public void SavePlayerStats()
    {
        PersistenFromSceneToScene.DataPersistenceInstance.competenceAmount = characterSheet.CompetenceAmount;
    }

    public void LoadPlayerStats ()
    {
        if (PersistenFromSceneToScene.DataPersistenceInstance.competenceAmount.Count > 0)
            characterSheet.CompetenceAmount = PersistenFromSceneToScene.DataPersistenceInstance.competenceAmount;
    }

    // Use this for initialization
    void Start ()
    {
        characterSheet = transform.parent.GetComponent<CharacterSheetManager>();

        //Initializing the lists based on the number of skills contained in the character sheet class
        RegisteredBranchTopPositions = new Vector3[characterSheet.skillNames.Count];
        spawnedTags = new GameObject[characterSheet.skillNames.Count];

        LoadPlayerStats();


        //Force the Competence amount to be of at least 4 different skills.
	    if (characterSheet.CompetenceAmount.Count < 4)
        {
            //Array.Resize(ref characterSheet.CompetenceAmount, 4);
        }

        //Let's make sure the competence amount won't be spawned with a value inferior to 1.
        // TO DO : Make sure this is actually a good idea, but if we spawn at 0 right now, it mess up with the curves display and generates error and perfs issues.
       /* for (int i = 0; i < characterSheet.CompetenceAmount.Count; i++)
        {
            if (characterSheet.CompetenceAmount[i] <= 0)
                characterSheet.CompetenceAmount[i] = 1;
        }*/

        //Let's get the greatest skill value first
        foreach (float skillPoint in characterSheet.CompetenceAmount)
        {
            if (skillPoint > greatestSkillValue)
                greatestSkillValue = skillPoint;
        }

        spawnedLines = new LineRenderer[characterSheet.CompetenceAmount.Count];
        spawnedBranches = new LineRenderer[characterSheet.CompetenceAmount.Count];

        InitializeSpider();
        SpawnTags();
	}

    void InitializeSpider ()
    {
        float BranchAngle = 360 / characterSheet.CompetenceAmount.Count;
        float addAngle = BranchAngle;

        Vector3[] newPositions = new Vector3[2];

        Vector3 firstBranchPosition = Vector3.zero;
        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        for (int i = 0; i < characterSheet.CompetenceAmount.Count; i++)
        {
            //Spawn a branch, parent it to the spider object, then rename it for clarity's sake.
            GameObject spawnedCompetence = GameObject.Instantiate(CompetencePrefab, transform.position, Quaternion.identity) as GameObject;
            spawnedCompetence.transform.SetParent(transform);
            spawnedCompetence.transform.name = "Competence" + i;

           
            // Debug.DrawLine(transform.position, transform.position + currentSkillPosition, Color.red, Mathf.Infinity);

            spawnedBranches[i] = UpdateBranchPositions(addAngle, newPositions, i, ref currentSkillPosition, spawnedCompetence); ;

            //If we did'nt change the first branch's position, then it must be the one we're looking at right now
            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            //Incrementing the angle for the next branch, in order to rotate it properly
            addAngle -= BranchAngle;

            if (i != 0)
            {
                //As long as we're not looking to the first branch, we can spawn one of the web's wire. 
                spawnedLines[i - 1] = SpawnWebWire(previousSkillPosition, currentSkillPosition, i - 1);
            }

            previousSkillPosition = currentSkillPosition;
        }

        //For the very last web wire, we spawn it using the first branch position
        spawnedLines[characterSheet.CompetenceAmount.Count - 1] = SpawnWebWire(currentSkillPosition, firstBranchPosition, characterSheet.CompetenceAmount.Count - 1);
    }

    void SpawnTags ()
    {
        int iterator = 0;
        foreach (Vector3 topPosition in RegisteredBranchTopPositions)
        {
            spawnedTags[iterator] = Instantiate(tagPrefab, topPosition, Quaternion.identity) as GameObject;
                iterator++;
        }
    }

    void UpdateTags()
    {
        int iterator = 0;
        foreach (Vector3 topPosition in RegisteredBranchTopPositions)
        {
            spawnedTags[iterator].transform.position = topPosition;

            spawnedTags[iterator].GetComponent<TextMesh>().text = characterSheet.skillNames[iterator];
            spawnedTags[iterator].transform.name = "Tag_" + characterSheet.skillNames[iterator];
            spawnedTags[iterator].transform.SetParent(transform);

            iterator++;
        }
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
        Vector3[] webWirePos = new Vector3[2];
        webWirePos[0] = previousLineTip + transform.position /*+ previousLineTip * .1f*/;
        webWirePos[1] = newPositions + transform.position /*+ newPositions * .1f*/;

        line.GetComponent<CustomizeLineRenderer>().linePositions = webWirePos;
        line.GetComponent<LineRenderer>().SetWidth(spiderThickness, spiderThickness);

        //Use this instead of SmoothCurve() to get a clearer view of the stats shaping the web line
        line.GetComponent<CustomizeLineRenderer>().RoughCurve();

        //line.GetComponent<CustomizeLineRenderer>().SmoothCurve();
    }

    LineRenderer UpdateBranchPositions (float addAngle, Vector3[] newPositions, int i, ref Vector3 currentSkillPosition, GameObject spawnedCompetence)
    {
        Vector3 newRotatedVector = new Vector3(0, branchesSize, 0);
        newRotatedVector = Quaternion.AngleAxis(-addAngle, Vector3.forward) * newRotatedVector;
        newPositions[0] = transform.position;
        newPositions[1] = newRotatedVector + transform.position;

        Vector3 branchDirection = newPositions[1] - newPositions[0];
        RegisteredBranchTopPositions[i] = newPositions[1] + branchDirection * .2f;

        //Then we apply the position to the branches' line renderers.
        LineRenderer CompetenceLine = spawnedCompetence.GetComponent<LineRenderer>();
        CompetenceLine.SetPositions(newPositions);
        CompetenceLine.SetWidth(spiderThickness, spiderThickness);

         Debug.DrawLine(transform.position, transform.position + newRotatedVector, Color.red, Time.deltaTime);
        //Only a small part of the vector
         Debug.DrawLine(transform.position, transform.position + newRotatedVector * .1f, Color.blue, Time.deltaTime);

        //Set the position of the skill point, it should be on the associated branch
        float percentageValue;

        if (greatestSkillValue > 0)
            percentageValue = characterSheet.CompetenceAmount[i] / greatestSkillValue * branchesSize + branchesSize * .1f;
        else
            percentageValue = 0;

        currentSkillPosition = new Vector3(0, percentageValue, 0);
        currentSkillPosition = Quaternion.AngleAxis(-addAngle, Vector3.forward) * currentSkillPosition;

        return CompetenceLine;
    }

    void UpdateSpider ()
    {
        //Reset greatestSkillValue for update
        greatestSkillValue = 0;

        //Let's get the greatest skill value first
        foreach (float skillPoint in characterSheet.CompetenceAmount)
        {
            if (skillPoint > greatestSkillValue)
                greatestSkillValue = skillPoint;
        }

        Vector3[] newPositions = new Vector3[2];

        float BranchAngle = 360 / characterSheet.CompetenceAmount.Count;
        float addAngle = BranchAngle;

        Vector3 firstBranchPosition = Vector3.zero;
        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        for (int i= 0; i < characterSheet.CompetenceAmount.Count; i++)
        {
            UpdateBranchPositions(addAngle, newPositions, i, ref currentSkillPosition, spawnedBranches[i].gameObject);

            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            if ( i != 0 )
                UpdateWebWirePositions(spawnedLines[i - 1], previousSkillPosition, currentSkillPosition); //TO DO, EXPERIMENTING THIS

            addAngle -= BranchAngle;

            previousSkillPosition = currentSkillPosition;
        }

         UpdateWebWirePositions(spawnedLines[characterSheet.CompetenceAmount.Count - 1], currentSkillPosition, firstBranchPosition);
    }

    void Update ()
    {
        UpdateSpider();
        SavePlayerStats();
        UpdateTags();
    }
}
