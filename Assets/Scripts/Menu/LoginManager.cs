using System.Collections;
using Assets.Scripts.Connexion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
	//Cette classe gère l'affiche du menu de login
    public class LoginManager : MonoBehaviour
    {

        public InputField login;
        public InputField password;
        public Text msgErreur;
        private ConnexionController connexionController;
        private Waiter _waiter = new Waiter();

        void Start()
        {
            connexionController = GameObject.Find("ConnexionController").GetComponent<ConnexionController>();
        }
		//Fonction appellée lors du clic sur le bouton login
        public void Login()
        {
            msgErreur.text = "";
            if (login.text != "" && password.text != "")
            {
                StartCoroutine(connexionController.Login(login.text, password.text, _waiter));
                msgErreur.color = Color.white;
                msgErreur.text = "Connexion en cours... (0/3)";
                StartCoroutine(this.Connect());
            }
            else
            {
                msgErreur.color = Color.red;
                msgErreur.text = "Veuillez remplir tous les champs";
            }
        }
		//Affiche le message de connexionController en cours en fonction de la situation
        public IEnumerator Connect()
        {
            while (_waiter.waiting)
            {
                yield return new WaitForSeconds(0.1f);
                msgErreur.text = "Connexion en cours... (" + connexionController.step + "/3)";
            }

            if (connexionController.isLogged)
            {
                PlayerPrefs.SetString("username", login.text);
                SceneManager.LoadScene("MainBoard");
            }
            else if (connexionController.error)
            {
                msgErreur.color = Color.red;
                msgErreur.text = "Erreur de connexionController";
            }
            else
            {
                msgErreur.color = Color.red;
                msgErreur.text = connexionController.returnText;
            }
        }
    }
}
