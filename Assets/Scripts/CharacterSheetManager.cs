using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class CharacterSheetManager : MonoBehaviour {

    Camera SheetCamera;
    GameObject gameCanvas;
    Canvas sheetCanvas;

    public List<CompetenceENI> competencesList;

    void Update ()
    {
        PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences = competencesList; //Saving for this playsession
    }

    void WeighUpSkillFactor() //Updates the total amount of points of each skill based on their associated criterias
    {
        foreach (CompetenceENI competence in competencesList)
        {
            if (competence._listeCriteres.Count != 0)
            {
                float qualityPercentValue = 100 / competence._listeCriteres.Count;

                foreach (Criteres critere in competence._listeCriteres)
                {
                    float qualityWeight = qualityPercentValue / critere.criterePalliers;
                    qualityWeight *= critere.criterePoints;
                    competence._nbPointsCompetence += (int)qualityWeight;
                }
            }
            else
                Debug.LogWarning("Updated total amount of skill " + competence._Name + ", but it does not contain any criterias");
        }
    }

    void Start ()
    {
        SheetCamera = transform.Find("CharacterSheetCamera").GetComponent<Camera>();
        gameCanvas = GameObject.Find("GameUI");
        sheetCanvas = transform.Find("CharacterSheetCanvas").GetComponent<Canvas>();

        if (PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences.Count > 0) //Loading for this playsession
            competencesList = PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences;
    }

    public void AddQualityStep (string skillName, int qualityNumber, int stepIncrementation)
    {
        for (int iterator = 0; iterator < competencesList.Count; iterator++)
        {
            if (competencesList[iterator]._Name == skillName)
            {
                //Let's check if we improved our score, if not, we do not update the new points
                if (competencesList[iterator]._listeCriteres[qualityNumber].criterePoints < stepIncrementation)
                {
                    Debug.Log("Improved score in criteria ''" + competencesList[iterator]._listeCriteres[qualityNumber].Name + "'', linked to skill ''" + competencesList[iterator]._Name + "''.");

                    //Then we make sure the new criteria does not exceed the max step
                    if (stepIncrementation > competencesList[iterator]._listeCriteres[qualityNumber].criterePalliers)
                        competencesList[iterator]._listeCriteres[qualityNumber].criterePoints = competencesList[iterator]._listeCriteres[qualityNumber].criterePalliers;
                    else
                        competencesList[iterator]._listeCriteres[qualityNumber].criterePoints = stepIncrementation;

                    WeighUpSkillFactor();
                }
                break;
            }
        }
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
