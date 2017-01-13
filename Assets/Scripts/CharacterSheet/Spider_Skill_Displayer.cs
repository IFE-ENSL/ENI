using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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

    Dictionary<int, int> GeneralSkillPoints = new Dictionary<int, int>();

    bool initComplete = false;

    public void SavePlayerStats()
    {
        PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences = characterSheet.competencesList;
    }

    public void LoadPlayerStats ()
    {
        if (PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences.Count > 0)
            characterSheet.competencesList = PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences;
    }

    // Use this for initialization
    public void InitSpider ()
    {
        characterSheet = transform.parent.GetComponent<CharacterSheetManager>();

        foreach (CompetenceENI competenceENI in characterSheet.competencesList)
        {
            if (!GeneralSkillPoints.ContainsKey(competenceENI._MainSkillNumber)) //If the dictionary already contains the General Skill we're looking at, let's skip it and just add the points if any.
                GeneralSkillPoints.Add(competenceENI._MainSkillNumber, competenceENI._nbPointsCompetence); //TODO : This works, now you have to rewrite the generation of the spider based on this dictionary.
            else
                GeneralSkillPoints[competenceENI._MainSkillNumber] += (int)competenceENI._nbPointsCompetence;
        }

        //Initializing the lists based on the number of skills contained in the character sheet class
        RegisteredBranchTopPositions = new Vector3[GeneralSkillPoints.Count];
        spawnedTags = new GameObject[GeneralSkillPoints.Count];

        LoadPlayerStats(); //TODO: I was here ok

        //Let's get the greatest skill value first
        foreach (KeyValuePair<int, int> valuePair in GeneralSkillPoints)
        {
            if (valuePair.Value > greatestSkillValue)
                greatestSkillValue = valuePair.Value;
        }

        spawnedLines = new LineRenderer[GeneralSkillPoints.Count];
        spawnedBranches = new LineRenderer[GeneralSkillPoints.Count];

        InitializeSpider();
        SpawnTags();

        initComplete = true;
	}

    void InitializeSpider ()
    {
        float BranchAngle = 360 / GeneralSkillPoints.Count;
        float addAngle = BranchAngle;

        Vector3[] newPositions = new Vector3[2];

        Vector3 firstBranchPosition = Vector3.zero;
        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        for (int i = 0; i < GeneralSkillPoints.Count; i++)
        {
            //Spawn a branch, parent it to the spider object, then rename it for better clarity.
            GameObject spawnedCompetence = GameObject.Instantiate(CompetencePrefab, transform.position, Quaternion.identity) as GameObject;
            spawnedCompetence.transform.SetParent(transform);
            spawnedCompetence.transform.name = "Competence" + i;

            // Debug.DrawLine(transform.position, transform.position + currentSkillPosition, Color.red, Mathf.Infinity);

            spawnedBranches[i] = UpdateBranchPosAndSkillValues(addAngle, newPositions, i, ref currentSkillPosition, spawnedCompetence); ;

            //If we didn't change the first branch's position, then it must be the one we're looking at right now
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
        spawnedLines[GeneralSkillPoints.Count - 1] = SpawnWebWire(currentSkillPosition, firstBranchPosition, GeneralSkillPoints.Count - 1);
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

            spawnedTags[iterator].GetComponent<TextMesh>().text = characterSheet.competencesList[iterator]._Name;
            spawnedTags[iterator].transform.name = "Tag_" + characterSheet.competencesList[iterator];
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
        webWirePos[0] = previousLineTip + transform.position;
        webWirePos[1] = newPositions + transform.position ;

        line.GetComponent<CustomizeLineRenderer>().linePositions = webWirePos;
        line.GetComponent<LineRenderer>().SetWidth(spiderThickness, spiderThickness);

        //Use this instead of SmoothCurve() to get a clearer view of the stats shaping the web line
        line.GetComponent<CustomizeLineRenderer>().RoughCurve();

        //line.GetComponent<CustomizeLineRenderer>().SmoothCurve();
    }

    LineRenderer UpdateBranchPosAndSkillValues (float addAngle, Vector3[] newPositions, int i, ref Vector3 currentSkillPosition, GameObject spawnedCompetence)
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
        {
            if (greatestSkillValue < 1)
                percentageValue = characterSheet.competencesList[i]._nbPointsCompetence * branchesSize + branchesSize * .1f;
            else
                percentageValue = characterSheet.competencesList[i]._nbPointsCompetence / greatestSkillValue * (branchesSize - branchesSize * .1f) + branchesSize * .1f;
        }
        else
        {
            percentageValue = branchesSize * .1f;
        }

        currentSkillPosition = new Vector3(0, percentageValue, 0);
        currentSkillPosition = Quaternion.AngleAxis(-addAngle, Vector3.forward) * currentSkillPosition;

        return CompetenceLine;
    }

    void UpdateSpider ()
    {
        //Reset greatestSkillValue for update
        greatestSkillValue = 0;

        //Let's get the greatest skill value first
        foreach (CompetenceENI competence in characterSheet.competencesList)
        {
            if (competence._nbPointsCompetence > greatestSkillValue)
                greatestSkillValue = competence._nbPointsCompetence;
        }

        Vector3[] newPositions = new Vector3[2];

        float BranchAngle = 360 / GeneralSkillPoints.Count;
        float addAngle = BranchAngle;

        Vector3 firstBranchPosition = Vector3.zero;
        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        for (int i= 0; i < GeneralSkillPoints.Count; i++)
        {
            UpdateBranchPosAndSkillValues(addAngle, newPositions, i, ref currentSkillPosition, spawnedBranches[i].gameObject);

            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            if ( i != 0 )
                UpdateWebWirePositions(spawnedLines[i - 1], previousSkillPosition, currentSkillPosition);

            addAngle -= BranchAngle;

            previousSkillPosition = currentSkillPosition;
        }

         UpdateWebWirePositions(spawnedLines[GeneralSkillPoints.Count - 1], currentSkillPosition, firstBranchPosition);
    }

    void Update ()
    {
        if (initComplete)
        {
            foreach (KeyValuePair<int, int> keyValuePair in GeneralSkillPoints)
            {
                Debug.Log("SkillNumber = " + keyValuePair.Key + " & points for this skill = " + keyValuePair.Value);
            }

            UpdateSpider();
            SavePlayerStats();
            UpdateTags();
        }
    }
}
