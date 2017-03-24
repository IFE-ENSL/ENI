using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Assets.Scripts.SaveSystem;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

//Classe utilisée afin de communiquer avec le serveur
namespace Assets.Scripts.Connexion
{
    public class ConnexionController : MonoBehaviour
    {
        public bool dontDestroyOnLoad = false;
        public bool isLogged = false;
        public bool wait = false;
        public bool error = false;
        public bool appQuit = false;

        public string returnText;
        public string messages = null;

        public int step = 0;

        public ManagementConnexion mConnexion;
        public static ConnexionController ConnexionControllerInstance;

        bool enableCheckLog = false;

        void Awake()
        {
            if (dontDestroyOnLoad) //If we won't destroy this on load, let's make it a singleton object, to make sure there's no double of this object in any scene
            {
                if (ConnexionControllerInstance == null)
                {
                    DontDestroyOnLoad(gameObject);
                    ConnexionControllerInstance = this;
                }
                else if (ConnexionControllerInstance != this)
                {
                    Debug.Log("Connexion controller exists twice, destroying one right now...");
                    Destroy(gameObject);
                }
            }
            enableCheckLog = true;
        }

        void LateUpdate ()
        {
            if (enableCheckLog)
            {
                CheckLog();
                enableCheckLog = false;
            }
        }

        //Let's make sure that if we're in game, we're properly logged, else, let us take the player back to the login screen
        void CheckLog ()
        {
            if (!isLogged && SceneManager.GetActiveScene().name != "Login")
            {
                Debug.Break();
                Debug.LogWarning("Lost login connexionController during the game!");
                SceneManager.LoadScene("Login");
            }
        }

        //Méthode permettant d'envoyer un log
        public IEnumerator PostLog(string nomLog, string miniJeu, ILog donnees)
        {
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> {{"Cookie", sessionId}};
            WWWForm hs_post = new WWWForm();
            string donneesJSON = JsonUtility.ToJson(donnees);
            hs_post.AddField("nomLog", nomLog);
            hs_post.AddField("miniJeu", miniJeu);
            hs_post.AddField("donnees", donneesJSON);

            Debug.Log("Envoi d'un log au serveur");

            WWW hs_get = new WWW(SQLCommonVars.addLogURL, hs_post.data,headers);
            yield return hs_get;

            if (hs_get.error != null)
            {
                print("Erreur lors de l'envoie des logs au serveur : " + hs_get.error);
                print(hs_get.text);
                SceneManager.LoadScene(0);
            }
            else if(hs_get.text != "1")
            {
                print("Une erreur est survenue : " + hs_get.text);
                SceneManager.LoadScene(0);
            }
        }

        //Méthode permettant d'envoyer un log avec un retour d'information
        public IEnumerator PostLog(string nomLog, string miniJeu, ILog donnees, Waiter waiter)
        {
            waiter.waiting = true;
            yield return this.PostLog(nomLog, miniJeu, donnees);
            waiter.waiting = false;
        }

        //Méthode permettant de s'identifier avec le serveur
        public IEnumerator Login(string login, string password, Waiter waiter)
        {
            //Etape zero : Envoie de l'username et récupération du sallage de l'utilisateur
            step = 0;
            waiter.waiting = true;
            //wait = true;
            WWWForm hs_post = new WWWForm();
            hs_post.AddField("username", login);
            WWW hs_get = new WWW(SQLCommonVars.loginURL,hs_post);
            yield return hs_get;
            //Etape une : Génération du mot de passe en convertisant le mot de passe indiqué en SHA512 + combinaison avec le salage
            //Puis, vérification de la concordance identifiant / mot de passe, si oui on s'identifie avec symfony
            step = 1;
            if (hs_get.error != null)
            {
                Debug.Log("Erreur lors de l'authentification : " + hs_get.error);
                error = true;
            }
            string salt = hs_get.text;
            string passAndSalt = String.Format("{0}{{{1}}}", password, salt);
            String hashed = BitConverter.ToString(((SHA512)new SHA512Managed()).ComputeHash(Encoding.ASCII.GetBytes(passAndSalt))).Replace("-", "");
            hs_post = null;
            hs_get = null;
            hs_post = new WWWForm();
            hs_post.AddField("username", login);
            hs_post.AddField("password", hashed.ToLower());
            hs_get = new WWW(SQLCommonVars.checkLoginURL,hs_post);
            yield return hs_get;
            step = 2;
            if (hs_get.error != null)
            {
                Debug.Log("Erreur lors de l'authentification : " + hs_get.error);
                error = true;
            }
            string code = hs_get.text;
            if (code == "1")
            {
                //Récupération du cookie d'authentification
                string reqHeaders = hs_get.responseHeaders["SET-COOKIE"];
                string sessionId = reqHeaders.Split(';')[0];
                hs_post = null;
                hs_get = null;
                hs_post = new WWWForm();
                hs_post.AddField("sessionId", sessionId);
                //Renvoie du cookie d'authentification afin de maintenir la connexionController
                Dictionary<String, String> headers = new Dictionary<string, string> {{"Cookie", sessionId}};
                hs_get = new WWW(SQLCommonVars.addTokenURL, hs_post.data,headers);
                yield return hs_get;
                //Etape 3 : vérification du bon fonctionnement de l'authentification et insertion d'une nouvelle session de jeu dans la base de données
                step = 3;
                if (hs_get.error != null)
                {
                    Debug.Log("Erreur lors de l'authentification : " + hs_get.error);
                }
                if (hs_get.text == "1")
                {
                    PlayerPrefs.SetString("sessionId", sessionId);
                    PlayerPrefs.SetString("username", login);
                    isLogged = true;
                    Debug.Log(login + " connecté, id de session : " + sessionId);
                }
                else
                {
                    Debug.Log("Erreur lors de l'authentification : " + hs_get.text);
                    error = true;
                    returnText = code;
                }
            }
            else
            {
                Debug.Log("Erreur : " + code);
                returnText = code;
            }
            //wait = false;
            waiter.waiting = false;
        }

        //Méthode utilisée lors de la fermeture du jeu, afin de terminer proprement la session
        //TODO: This is not used anymore, but we should reimplement it.
        public IEnumerator EndSession()
        {
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = SQLCommonVars.endSessionURL;
            WWW hs_post = new WWW(post_url, null, headers);
            yield return hs_post; // Wait until the download is done
            if (hs_post.error != null)
            {
                print("Erreur lors de l'envoie d'un message au serveur : " + hs_post.error);
            }
            if (hs_post.text != "1")
            {
                print("Une erreur est survenue : " + hs_post.text);
            }
            this.appQuit = true;
            Application.Quit();
        }

    }
}