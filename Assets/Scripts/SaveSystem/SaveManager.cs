using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Management;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.SaveSystem
{
    //Base du système de sauvegarde
    public class SaveManager : MonoBehaviour {
        private string myURL;
        public List<GameObject> gameobjects;
        public string PlayerSave { get; set; }

        public bool LoadData { get; set; }

        [HideInInspector]
        public bool gameLoaded;

        public bool saveExists = true;

        public bool isSaving = false;

        public SaveManager()
        {
            LoadData = false;
        }

        void Start()
        {
            PlayerSave = PlayerPrefs.GetString("username") + "save";
            gameLoaded = true;
            DontDestroyOnLoad(this.gameObject);
            //myURL = "http://vm-web7.ens-lyon.fr/eni/web/ES2.php?webfilename=" + PlayerSave;
            myURL = "http://localhost/eni/web/ES2.php?webfilename=" + PlayerSave;
        }
        //Permet de définir les objets à sauvegarder lors du changement d'un niveau
        void OnLevelWasLoaded(int level)
        {
            if (level == 1)
            {
                gameobjects = new List<GameObject> {GameObject.Find("PlayerData")};
                if (LoadData)
                {
                    StartCoroutine(this.Load());
                    LoadData = false;
                }
            }
			if (level == 5 || level == 6)
            {
				gameobjects = new List<GameObject> { GameObject.Find("GrillePersonnages"), GameObject.Find("PlayerData"), GameObject.Find("GameManager") };
                if (LoadData)
                {
                    //TODO : Get rid of this? GameObject.Find("GameManager").GetComponent<GameManager>().newGame = false;
                    StartCoroutine(this.Load());
                    LoadData = false;
                }
            }
        }
        //Sauvegarde tous les objets contenus dans la collection d'objets à sauvegarder
        public void Save()
        {
            isSaving = true;
            foreach (GameObject go in gameobjects)
            {
                go.SendMessage("Save", this.PlayerSave);
            }
            string actualScene = SceneManager.GetActiveScene().name;
            ES2.Save(actualScene, PlayerSave + "?tag=loadedScene");
            StartCoroutine(UploadSave());
        }
        //Charge tous les objets contenus dans la collection d'objets à sauvegarder
        public IEnumerator Load()
        {
            gameLoaded = false;
            yield return StartCoroutine(DownloadEntireFile(this.PlayerSave));
            if (saveExists)
            {
                foreach (GameObject go in gameobjects)
                {
                    go.SendMessage("Load", this.PlayerSave);
                }
                string saveData = ES2.Load<string>(PlayerSave + "?tag=questDialogueSave");
                Debug.Log("Load Game Data: " + saveData);
                gameLoaded = true;
            }

        }
        //Met la sauvegarde sur le serveur
        public IEnumerator UploadSave()
        {
            ES2Web web = new ES2Web(myURL);
            // Start uploading our data and wait for it to finish.
            yield return StartCoroutine(web.UploadFile(PlayerSave));
            if (web.isError)
            {
                // Enter your own code to handle errors here.
                // For a list of error codes, see www.moodkie.com/easysave/ES2Web.ErrorCodes.php
                Debug.LogError(web.errorCode + ":" + web.error);
            }
            isSaving = false;
        }
        //Télécharge la sauvegarde depuis le serveur
        public IEnumerator DownloadEntireFile(string fileName)
        {
            // As we don't specify a tag, it will download everything
            // within the file 'myFile.txt'.
            ES2Web web = new ES2Web(myURL);

            // Start downloading our data and wait for it to finish.
            yield return StartCoroutine(web.Download());

            if (web.isError)
            {

                if (web.errorCode == "05")
                {
                    saveExists = false;
                    Debug.LogWarning("Aucune sauvegarde n'existe pour cet utilisateur");
                }
                else
                    Debug.LogError(web.errorCode + ":" + web.error);
            }
            else
                web.SaveToFile(fileName);
        }
    }
}
