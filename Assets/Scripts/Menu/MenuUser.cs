using Assets.Scripts.SaveSystem;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
	//Cette classe est utilisée par le menu général du jeu
    public class MenuUser : MonoBehaviour {

        public Text userText;
		public SaveManager sm;

        public void Start()
        {
			sm = GameObject.Find ("SaveManager").GetComponent<SaveManager>();
            if (userText != null)
                userText.text = "Bienvenue, " + PlayerPrefs.GetString("username");
            else
                Debug.Log("Le texte d'affichage de l'username du menu n'est pas défini");
        }
		//Fonction appellée lors du clic sur un bouton permettant de lancer une scene du jeu
        public void openScene(string scene)
        {
            if (scene == "Management")
            {
                System.Random r = new System.Random();
                scene = "Management" + r.Next(1, 3);
            }
            SceneManager.LoadScene(scene);
        }
		//Fonction appellée lors du lancement d'une scène sauvegardée
        public void openSceneAndLoad()
        {
			if(ES2.Exists(sm.PlayerSave + "?tag=managementSceneId"))
			{
				string scene = ES2.Load<string> (sm.PlayerSave + "?tag=managementSceneId");
				GameObject.Find ("SaveManager").GetComponent<SaveManager> ().LoadData = true;
				SceneManager.LoadScene(scene);
			}
			else
				Debug.Log("Aucun sauvegarde disponible");
        }
    }
}
