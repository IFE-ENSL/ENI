using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class Spider_Skill_Displayer : MonoBehaviour {

    #region External Objects
    public GameObject tagPrefab;
    Image backgroundWindow;
    CharacterSheetManager characterSheet;
    #endregion

    #region Render
    Dictionary<int, LineRenderer> spawnedBranches = new Dictionary<int, LineRenderer>();
    Dictionary<int, LineRenderer> spawnedLines = new Dictionary<int, LineRenderer>();
    Dictionary<int, Vector3> RegisteredBranchTopPositions = new Dictionary<int, Vector3>();
    Dictionary<int, GameObject> spawnedTags = new Dictionary<int, GameObject>();
    Transform firstBranch;

    public float spiderThickness = .1f;
    public int tagsFontSize = 3;

    public GameObject CompetencePrefab;
    public GameObject SpiderWebWirePrefab;
    public Vector3 minimizePosLargeScreen;
    public Vector3 minimizePosSmallScreen;
    public float branchesSize = 20;
    #endregion

    #region Stats & state
    Dictionary<int, int> GeneralSkillPoints = new Dictionary<int, int>();
    float greatestSkillValue = 0;
    int firstBranchSkillNumber;
    int lastBranchSkillNumber;

    bool MouseOverThis = false;
    bool fullScreen = false;
    bool initComplete = false;
    public static float[] staticSkillAmount;
    #endregion

    void Start ()
    {
        GameObject backgroundObject = GameObject.Find("FullScreenSpiderBack");

        if (backgroundObject != null)
        {
            backgroundWindow = backgroundObject.GetComponent<Image>();
            backgroundWindow.enabled = false;
        }

        minimize();
    }

    // Use this for initialization
    public void StartSpider()
    {
        characterSheet = transform.parent.GetComponent<CharacterSheetManager>();

        UpdateGeneralSkillPoints();

        LoadPlayerStats();

        //Let's get the greatest skill value first && associate each branch & line with its skill number
        foreach (KeyValuePair<int, int> valuePair in GeneralSkillPoints)
        {
            if (valuePair.Value > greatestSkillValue)
                greatestSkillValue = valuePair.Value;

            spawnedLines.Add(valuePair.Key, new LineRenderer());
            spawnedBranches.Add(valuePair.Key, new LineRenderer());

            //Initializing the dictionaries based on the number of skills contained in the character sheet class
            RegisteredBranchTopPositions.Add(valuePair.Key, Vector3.zero);
            spawnedTags.Add(valuePair.Key, null);
        }

        InitializeSpiderDisplayComponents();
        SpawnTags();

        initComplete = true;
    }

    //Setup and call every method needed to initialize all the components that are parts of the spider's display
    public void InitializeSpiderDisplayComponents()
    {
        float BranchAngle = 360 / GeneralSkillPoints.Count;
        float addAngle = BranchAngle;

        Vector3[] newPositions = new Vector3[2];

        Vector3 firstBranchPosition = Vector3.zero;
        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;

        int iterator = 0;
        KeyValuePair<int, int> previousKeyValue = new KeyValuePair<int, int>(0, 0);
        foreach (KeyValuePair<int, int> keyValue in GeneralSkillPoints)
        {
            //Spawn a branch, parent it to the spider object, then rename it for better clarity.
            GameObject spawnedCompetence = GameObject.Instantiate(CompetencePrefab, transform.position, Quaternion.identity) as GameObject;
            spawnedCompetence.transform.SetParent(transform);
            spawnedCompetence.transform.name = "Competence" + keyValue.Key;

            // Debug.DrawLine(transform.position, transform.position + currentSkillPosition, Color.red, Mathf.Infinity);

            spawnedBranches[keyValue.Key] = UpdateBranchPosAndSkillValues(addAngle, newPositions, keyValue.Key, ref currentSkillPosition, spawnedCompetence);

            //If we didn't change the first branch's position, then it must be the one we're looking at right now
            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            //Incrementing the angle for the next branch, in order to rotate it properly
            addAngle -= BranchAngle;

            if (keyValue.Key != firstBranchSkillNumber)
            {
                //As long as we're not looking to the first branch, we can spawn one of the web's wire. 
                spawnedLines[previousKeyValue.Key] = SpawnWebWire(previousSkillPosition, currentSkillPosition, previousKeyValue.Key);
            }

            previousSkillPosition = currentSkillPosition;
            previousKeyValue = keyValue;

            if (iterator >= spawnedLines.Count - 1)
            {
                lastBranchSkillNumber = keyValue.Key;
            }

            iterator++;
        }

        //For the very last web wire, we spawn it using the first branch position
        spawnedLines[lastBranchSkillNumber] = SpawnWebWire(currentSkillPosition, firstBranchPosition, lastBranchSkillNumber);
    }

    //Initialize the tag at the spider's display start
    void SpawnTags()
    {
        foreach (KeyValuePair<int, Vector3> topPosition in RegisteredBranchTopPositions)
        {
            spawnedTags[topPosition.Key] = Instantiate(tagPrefab, topPosition.Value, Quaternion.identity) as GameObject;
        }
    }

    //Called the first time, to initialize the wire render at the start of the spider
    LineRenderer SpawnWebWire(Vector3 previousLineTip, Vector3 newPositions, int spawnNumber)
    {
        GameObject spawnedWebWire = GameObject.Instantiate(SpiderWebWirePrefab, transform.position, Quaternion.identity) as GameObject;
        LineRenderer spawnedWebWireRenderer = spawnedWebWire.GetComponent<LineRenderer>();
        spawnedWebWire.name = "Spider_StatWire_" + spawnNumber;
        spawnedWebWire.transform.SetParent(transform);

        UpdateWebWirePositions(spawnedWebWireRenderer, previousLineTip, newPositions);

        return spawnedWebWire.GetComponent<LineRenderer>();
    }

    public void SavePlayerStats()
    {
        PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences = CharacterSheetManager.competencesList;
    }

    public void LoadPlayerStats ()
    {
        if (PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences.Count > 0)
            CharacterSheetManager.competencesList = PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences;
    }

    void Update()
    {
        if (initComplete)
        {
            UpdateSpider();
            SavePlayerStats();
            UpdateTags();
        }

        float animSpeed = .5f;

        //Adapting the spider's size.
        if (MouseOverThis)
        {
            Debug.Log("Mouse over spider skill");

            if (!fullScreen)
                branchesSize = Mathf.MoveTowards(branchesSize, .90f, animSpeed * Time.deltaTime); //Small animation for the spider when the mouse is getting over it.

            if (Input.GetMouseButtonDown(0)) //If we click it...
            {
                if (fullScreen)
                    minimize();
                else
                    GoFullScreen();
            }
        }
        else if (!fullScreen)
        {
            branchesSize = Mathf.MoveTowards(branchesSize, .85f, animSpeed * Time.deltaTime);
        }
    }

    public void UpdateGeneralSkillPoints ()
    {
        bool first = true;

        GeneralSkillPoints.Clear();

        foreach (KeyValuePair<int, CompetenceENI> competenceENI in CharacterSheetManager.competencesList)
        {
            if (!GeneralSkillPoints.ContainsKey(competenceENI.Value._MainSkillNumber)) //If the dictionary already contains the General Skill we're looking at, let's skip it and just add the points;
            {
                GeneralSkillPoints.Add(competenceENI.Value._MainSkillNumber, competenceENI.Value._nbPointsCompetence);
            }
            else
                GeneralSkillPoints[competenceENI.Value._MainSkillNumber] += (int)competenceENI.Value._nbPointsCompetence;

            if (first)
            {
                firstBranchSkillNumber = competenceENI.Value._MainSkillNumber;
                first = false;
            }
        }
    }

    //Update the name of each skill displayed on the spider
    void UpdateTags()
    {
        foreach (KeyValuePair<int, Vector3> topPosition in RegisteredBranchTopPositions)
        {
            spawnedTags[topPosition.Key].transform.position = topPosition.Value;
            spawnedTags[topPosition.Key].GetComponent<TextMesh>().fontSize = tagsFontSize;

            foreach (KeyValuePair<int, CompetenceENI> competence in CharacterSheetManager.competencesList)
            {
                if (competence.Value._MainSkillNumber == topPosition.Key)
                {
                    spawnedTags[topPosition.Key].GetComponent<TextMesh>().text = competence.Value._Name;
                    break;
                }
            }

            spawnedTags[topPosition.Key].transform.SetParent(transform);
        }
    }

    //Update every wire position, so that each time a skill value has change, it will be reflected immediately on the spider.
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
                percentageValue = GeneralSkillPoints[i] * branchesSize + branchesSize * .1f;
            else
                percentageValue = GeneralSkillPoints[i] / greatestSkillValue * (branchesSize - branchesSize * .1f) + branchesSize * .1f;
        }
        else
        {
            percentageValue = branchesSize * .1f;
        }

        currentSkillPosition = new Vector3(0, percentageValue, 0);
        currentSkillPosition = Quaternion.AngleAxis(-addAngle, Vector3.forward) * currentSkillPosition;

        return CompetenceLine;
    }

    //This method makes sure every aspect of the spider will be updated in real time, so we can move the spider, change its size, etc... In real time.
    void UpdateSpider ()
    {
        //Reset greatestSkillValue for update
        greatestSkillValue = 0;

        //Let's get the greatest skill value first
        foreach (KeyValuePair<int,int> keyValuePair in GeneralSkillPoints)
        {
            if (keyValuePair.Value > greatestSkillValue)
                greatestSkillValue = keyValuePair.Value;
        }

        Vector3[] newPositions = new Vector3[2];

        float BranchAngle = 360 / GeneralSkillPoints.Count;
        float addAngle = BranchAngle;

        Vector3 firstBranchPosition = Vector3.zero;
        Vector3 previousSkillPosition = Vector3.zero;
        Vector3 currentSkillPosition = Vector3.zero;


        KeyValuePair<int, int> previousKeyValue = new KeyValuePair<int, int>(0, 0);
        int iterator = 0;
        foreach (KeyValuePair<int, int> keyValue in GeneralSkillPoints) //For each general skill in the database...
        {
            UpdateBranchPosAndSkillValues(addAngle, newPositions, keyValue.Key, ref currentSkillPosition, spawnedBranches[keyValue.Key].gameObject);

            if (firstBranchPosition == Vector3.zero)
                firstBranchPosition = currentSkillPosition;

            if ( previousKeyValue.Key != 0)
                UpdateWebWirePositions(spawnedLines[previousKeyValue.Key], previousSkillPosition, currentSkillPosition);

            addAngle -= BranchAngle;

            previousSkillPosition = currentSkillPosition;
            previousKeyValue = keyValue;

            if (iterator >= spawnedLines.Count - 1)
            {
                lastBranchSkillNumber = keyValue.Key;
            }

            iterator++;
        }
         UpdateWebWirePositions(spawnedLines[lastBranchSkillNumber], currentSkillPosition, firstBranchPosition);

        
    }

    void OnMouseOver()
    {
        if(characterSheet.ClickableSpider)
            MouseOverThis = true;
    }

    void OnMouseExit()
    {
        MouseOverThis = false;
    }

    //Reset the spider position to its original size, in case it was previously displayed on full screen
    void minimize()
    {
        branchesSize = .85f;
        spiderThickness = .01f;
        tagsFontSize = 10;
        gameObject.GetComponent<CircleCollider2D>().radius = branchesSize;
        fullScreen = false;

        if (backgroundWindow != null)
            backgroundWindow.enabled = false;

        if (Camera.main.aspect > 1.4f && Camera.main.aspect < 1.8f) // 16/9
            transform.position = minimizePosLargeScreen;
        else if (Camera.main.aspect > 1f && Camera.main.aspect < 1.4f) // 4/3
            transform.position = minimizePosSmallScreen;

    }

    //Display the spider on full screen
    void GoFullScreen()
    {
        branchesSize = 3.7f;
        spiderThickness = .1f;
        tagsFontSize = 20;
        gameObject.GetComponent<CircleCollider2D>().radius = branchesSize;
        fullScreen = true;
        backgroundWindow.enabled = true;
        transform.position = new Vector3(0, 0, -1f);
    }

}
