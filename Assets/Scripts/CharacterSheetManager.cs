﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterSheetManager : MonoBehaviour, ISerializationCallbackReceiver {

    Camera SheetCamera;
    GameObject gameCanvas;
    Canvas sheetCanvas;

    public List<string> skillNames;
    public List<float> CompetenceAmount;
    public List<QualityList> masterQualityList;

    public int PallierQualité;


    void Update ()
    {
        PersistenFromSceneToScene.DataPersistenceInstance.masterQualityList = masterQualityList; //Saving for this playsession
    }

    void WeighUpSkillFactor()
    {
        int iterator = 0;
        foreach (int skillAmount in CompetenceAmount)
        {
            float qualityPercentValue = 100 / masterQualityList[iterator].Qualities.Count;

            foreach (int qualityPoints in masterQualityList[iterator].Qualities)
            {
                float qualityWeight = qualityPercentValue / PallierQualité;
                qualityWeight *= qualityPoints;
                CompetenceAmount[iterator] += qualityWeight;
            }
            iterator++;
        }
    }

    void Start ()
    {
        SheetCamera = transform.Find("CharacterSheetCamera").GetComponent<Camera>();
        gameCanvas = GameObject.Find("GameUI");
        sheetCanvas = transform.Find("CharacterSheetCanvas").GetComponent<Canvas>();

        if (PersistenFromSceneToScene.DataPersistenceInstance.masterQualityList.Count > 0) //Loading for this playsession
            masterQualityList = PersistenFromSceneToScene.DataPersistenceInstance.masterQualityList;
    }

    int previousNamesLength = 0;
    int previousCompetenceLength = 0;

    //Is called right before Unity Serializes anything
    //This is to make sure the skill names & points lists always have the same length when editing them in the inspector
    public void OnBeforeSerialize()
    {
        if (skillNames.Count != CompetenceAmount.Count)
        {

            int diff;

            if (previousNamesLength != skillNames.Count)
            {
                diff = CompetenceAmount.Count - skillNames.Count;
                diff = Mathf.Abs(diff);

                if (CompetenceAmount.Count > skillNames.Count)
                    CompetenceAmount.RemoveRange(CompetenceAmount.Count - diff, diff);
                else
                {
                    Debug.Log("Skill names greater than Competence Amount, resizing..." + diff);
                    List<float> rangeToAdd = new List<float>();

                    for (int i = 0; i < diff; i++)
                    {
                        rangeToAdd.Add(0);
                        Debug.Log("Added one");
                    }

                    CompetenceAmount.AddRange(rangeToAdd);
                }
            }

            else if (previousCompetenceLength != CompetenceAmount.Count)
            {
                diff = CompetenceAmount.Count - skillNames.Count;
                diff = Mathf.Abs(diff);

                if (skillNames.Count > CompetenceAmount.Count)
                    skillNames.RemoveRange(skillNames.Count - diff, diff);
                else
                {
                    List<string> rangeToAdd = new List<string>();

                    for (int i = 0; i < Mathf.Abs(diff); i++)
                    {
                        rangeToAdd.Add("");
                        Debug.Log("Added one");
                    }

                    skillNames.AddRange(rangeToAdd);
                }
            }

            diff = masterQualityList.Count - skillNames.Count;
            diff = Mathf.Abs(diff);

            #region Setting up the size of the master Quality List
            if (masterQualityList.Count < skillNames.Count)
            {
                List<QualityList> rangeToAdd = new List<QualityList>();

                for (int i = 0; i < Mathf.Abs(diff); i++)
                {
                    rangeToAdd.Add(new QualityList());
                }

                masterQualityList.AddRange(rangeToAdd);
            }
            else if (skillNames.Count < masterQualityList.Count)
            {
                skillNames.RemoveRange(masterQualityList.Count - diff, diff);
            }
            #endregion
        }

        int iterator = 0;

        //Set the name for each list of qualities
        foreach (QualityList qualityList in masterQualityList)
        {
            qualityList.Name = skillNames[iterator];
            iterator++;
        }

        previousNamesLength = skillNames.Count;
        previousCompetenceLength = CompetenceAmount.Count;
    }

    public void OnAfterDeserialize()
    {

    }

    public void AddQualityStep (string skillName, int qualityNumber, int stepIncrementation)
    {
        int index = skillNames.IndexOf(skillName);

        masterQualityList[index].Qualities[qualityNumber] += stepIncrementation;

        WeighUpSkillFactor();
    }

    public void ToggleDisplaySheet ()
    {
        //Basically, we just deactivate any Game UI to replace it with the character sheet.
        //As the spider skill is not rendered by the Unity UI System, it is displayed through
        //another Camera hidden in the game scene.
        if (gameCanvas != null)
        {
            if (gameCanvas.activeSelf)
                gameCanvas.SetActive(false);
            else
                gameCanvas.SetActive(true);
        }

        if (sheetCanvas.enabled)
            sheetCanvas.enabled = false;
        else
            sheetCanvas.enabled = true;

        if (SheetCamera.depth == -2)
        {
            SheetCamera.depth = 0;
        }
        else if (SheetCamera.depth == 0)
        {
            SheetCamera.depth = -2;
        }

        Debug.Log("Toggled Player Sheet");

        //Let's add some controls constraints according to the scene we're in
        Scene currentScene = SceneManager.GetActiveScene();

        if (currentScene.name == "MainBoard")
            ToggleMainBoardConstraints();
    }

    //This method will prevent the pawn from moving if we're clicking anywhere inside the character sheet
    void ToggleMainBoardConstraints()
    {
        if (BoardManager.preventPlayerControl == true)
            BoardManager.preventPlayerControl = false;
        else if (BoardManager.preventPlayerControl == false)
            BoardManager.preventPlayerControl = true;
    }

}