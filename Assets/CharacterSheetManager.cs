﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterSheetManager : MonoBehaviour, ISerializationCallbackReceiver {

    Camera SheetCamera;
    GameObject gameCanvas;
    Canvas sheetCanvas;

    public List<string> skillNames;
    public List<int> CompetenceAmount;

    void Start ()
    {
        SheetCamera = transform.Find("CharacterSheetCamera").GetComponent<Camera>();
        gameCanvas = GameObject.Find("GameUI");
        sheetCanvas = transform.Find("CharacterSheetCanvas").GetComponent<Canvas>();
    }

    int previousNamesLength = 0;
    int previousCompetenceLength = 0;

    public void OnBeforeSerialize()
    {
        if (skillNames.Count != CompetenceAmount.Count)
        {

            int diff;

            if (previousNamesLength != skillNames.Count)
            {
                //CompetenceAmount.Capacity = skillNames.Count;
                diff = CompetenceAmount.Count - skillNames.Count;
                diff = Mathf.Abs(diff);

                if (CompetenceAmount.Count > skillNames.Count)
                    CompetenceAmount.RemoveRange(CompetenceAmount.Count - diff, diff);
                else
                {
                    Debug.Log("Skill names greater than Competence Amount, resizing..." + diff);
                    List<int> rangeToAdd = new List<int>();

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
                //skillNames.Capacity = CompetenceAmount.Count;
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
        }

        previousNamesLength= skillNames.Count;
        previousCompetenceLength = CompetenceAmount.Count;
    }

    public void OnAfterDeserialize()
    {

    }

    public void ToggleDisplaySheet ()
    {
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

    void ToggleMainBoardConstraints()
    {
        if (BoardManager.preventPlayerControl == true)
            BoardManager.preventPlayerControl = false;
        else if (BoardManager.preventPlayerControl == false)
            BoardManager.preventPlayerControl = true;
    }

}
