using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utility;

public class CharacterSheetManager : MonoBehaviour {

    #region Optional Features
    public bool GenerateCharacterSheetDisplay = true;
    public bool ClickableSpider = true;
    #endregion

    #region Retrieved datas from SQL
    static public JSONNode userStats;
    static public Dictionary<int, CompetenceENI> competencesList = new Dictionary<int, CompetenceENI>();
    public Dictionary<int, int> correspondenceUserCompENIAndMiniGame = new Dictionary<int, int>();
    #endregion

    #region Mandatory Parameters
    static public int game1ID;
    static public int game2ID;
    #endregion

    static public bool sendingDatas = false; //Used if we have to check if the datas are being sent. For example, during a loading screen...

    void Update ()
    {
        PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences = competencesList; //Saving for this playsession
    }

    void Start ()
    {
        if (PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences.Count > 0) //Loading for this playsession
            competencesList = PersistentFromSceneToScene.DataPersistenceInstance.listeCompetences;
    }

    //Use the datas retrieved on the scene start to populate the skill spider
    //This also calculates the spider general skill points based on the points in the CompENI
    static public void PopulateCharacterSkills()
    {
        competencesList.Clear();

        Dictionary<int, string> SkillTags = new Dictionary<int, string>();

        foreach (JSONNode value in userStats["listeCompetences"].Children)
        {
            if (!SkillTags.ContainsKey(value["idCompGen"].AsInt))
                SkillTags.Add(value["idCompGen"].AsInt, value["LibCompGen"].Value);
        }

        foreach (JSONNode value in userStats["listeCriteres"].Children)
        {
            competencesList.Add(value["idCompEni"].AsInt, 
                new CompetenceENI(SkillTags[value["idCompGen"].AsInt], value["idCompGen"].AsInt, value["point"].AsInt, value["idCritere"].AsInt, value["idJM"].AsInt));
        }

        foreach (JSONNode value in userStats["listeJeux"].Children)
        {
            if (value["jeuNom"].Value == "mini-jeu 01")
                CharacterSheetManager.game1ID = value["idJeu"].AsInt;
            else if (value["jeuNom"].Value == "mini-jeu 02")
                CharacterSheetManager.game2ID = value["idJeu"].AsInt;
        }

        GameObject.Find("SkillSpider").GetComponent<Spider_Skill_Displayer>().UpdateGeneralSkillPoints();
    }

    //Add points to the player's qualities in the corresponding CompENI before send it to the server
    public void AddQualityStep (int stepIncrementation, string gameLabel)
    {
        int gameID = 0;

        if (gameLabel == "mini-jeu 01")
                gameID = game1ID;

        if (gameLabel == "mini-jeu 02")
            gameID = game2ID;

        foreach (KeyValuePair<int,CompetenceENI> skill in competencesList)
        {
                if (skill.Value._idJM == gameID)
                {
                    Debug.Log("Comp Gen idJM Number is okay...");
                        competencesList[skill.Key]._nbPointsCompetence = stepIncrementation;
                        Debug.Log("Calling coroutine for ending player stats...");
                        StartCoroutine(PostPLayerStats(skill.Key, competencesList[skill.Key]._nbPointsCompetence));
                }
        }
    }

    //Sending the skill points to the SQL DB
    public IEnumerator PostPLayerStats(int idCompEni, int pointCompEni)
    {
        sendingDatas = true;
        string sessionId = PlayerPrefs.GetString("sessionId");
        Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
        WWWForm hs_post = new WWWForm();
        hs_post.AddField("idCompEni", idCompEni);
        hs_post.AddField("point", pointCompEni);

        Debug.Log("Envoi d'un log au serveur");

        WWW hs_get = new WWW(SQLCommonVars.baseURL + "/web/app.php/unity/compEniPoint", hs_post.data, headers);
        yield return hs_get;
        Debug.Log("Sent new player skill datas");
        sendingDatas = false;

        if (hs_get.error != null)
        {
            print("Erreur lors de l'envoi des points de skills : " + hs_get.error);
            print(hs_get.text);
            SceneManager.LoadScene(0);
        }
        else if (hs_get.text != "1")
        {
            print("Une erreur est survenue : " + hs_get.text);
            SceneManager.LoadScene(0);
        }
    }

}
