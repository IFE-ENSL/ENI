﻿using System;
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
        //public const string baseURL = "http://vm-web7.ens-lyon.fr/eni"; //Prod
        public const string baseURL = "http://127.0.0.1/eni"; //Local
        //private const string baseURL = "http://vm-web-qualif.pun.ens-lyon.fr/eni/"; //Preprod
        private const string addLogURL = baseURL + "/web/app_dev.php/unity/addLog";
        private const string loginURL = baseURL + "/web/app_dev.php/unity/loginOne";
        private const string checkLoginURL = baseURL + "/web/app_dev.php/unity/loginCheck";
        private const string addTokenURL = baseURL + "/web/app_dev.php/unity/loginToken";
        private const string insertMessageURL = baseURL + "/web/app_dev.php/unity/insertMessage";
        private const string getMessagesURL = baseURL + "/web/app_dev.php/unity/getMessages";
        private const string endSessionURL = baseURL + "/web/app_dev.php/unity/endSession";
        public bool dontDestroyOnLoad = false;
        public bool isLogged = false;
        public bool wait = false;
        public bool error = false;
        public bool appQuit = false;
        public int step = 0;
        public string returnText;
        public string messages = null;
        public bool justLoggedIn = false;

        bool enableCheckLog = false;

        private SaveManager sm;
        public ManagementConnexion mConnexion;

        public static ConnexionController ConnexionControllerInstance;

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

        void Start ()
        {
            sm = FindObjectOfType<SaveManager>();
        }

        void LateUpdate ()
        {
            if (enableCheckLog)
            {
                CheckLog();
                enableCheckLog = false;
            }

            Scene currentScene = SceneManager.GetActiveScene();

            Waiter _waiter = new Waiter();

            if (currentScene.name == "MainBoard" && currentScene.isLoaded && justLoggedIn)
            {
                //TODO : Move all those verifications in BoardManager maybe.
                justLoggedIn = false;
            }
        }



        //Let's make sure that if we're in game, we're properly logged, else, let us take the player back to the login screen
        void CheckLog () //TODO: The thing is, if this object exists before getting deleted by the singleton principle, this method will put the player back to the login screen.
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

            WWW hs_get = new WWW(addLogURL, hs_post.data,headers);
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

        //Sending the skill points to the SQL DB
        public IEnumerator PostPLayerStats(int idCompEni, int pointCompEni)
        {
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
            WWWForm hs_post = new WWWForm();
            hs_post.AddField("idCompEni", idCompEni);
            hs_post.AddField("point", pointCompEni);

            Debug.Log("Envoi d'un log au serveur");

            WWW hs_get = new WWW(baseURL + "/web/app_dev.php/unity/compEniPoint", hs_post.data, headers);
            yield return hs_get;

            if (hs_get.error != null)
            {
                print("Erreur lors de l'envoie des logs au serveur : " + hs_get.error);
                print(hs_get.text);
                SceneManager.LoadScene(0);
            }
            else if (hs_get.text != "1")
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
            WWW hs_get = new WWW(loginURL,hs_post);
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
            hs_get = new WWW(checkLoginURL,hs_post);
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
                hs_get = new WWW(addTokenURL, hs_post.data,headers);
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

        //Méthode utilisée afin de poster un message sur le chat
        public IEnumerator PostMessage(string message)
        {
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = insertMessageURL;
            WWWForm hs_form = new WWWForm();
            hs_form.AddField("message",message);
            WWW hs_post = new WWW(post_url,hs_form.data,headers);
            yield return hs_post; // Wait until the download is done
            if (hs_post.error != null)
            {
                print("Erreur lors de l'envoie d'un message au serveur : " + hs_post.error);
            }
            if (hs_post.text != "1")
            {
                print("Une erreur est survenue : " + hs_post.text);
            }
        }

        //Méthode utilisée afin de récupérer les messages du chat
        public IEnumerator getMessages()
        {
            Debug.Log("Get Messages Called...");
            wait = true;
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = getMessagesURL;
            WWW hs_get = new WWW(post_url,null,headers);
            yield return hs_get;
            if (hs_get.error != null)
            {
                Debug.Log("Erreur lors de la récupération des messages : " + hs_get.error);
                Debug.Log(hs_get.text);
                SceneManager.LoadScene(0);
            }
            //Debug.Log(messages);
            this.messages = hs_get.text;
            wait = false;
        }

        //Méthode utilisée lors de la fermeture du jeu, afin de terminer proprement la session
        public IEnumerator EndSession()
        {
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = endSessionURL;
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
            while (sm.isSaving == true)
                yield return new WaitForSeconds(0.1f);
            this.appQuit = true;
            Application.Quit();
        }

        //Méthode appellée lors de la fermeture du jeu, permet de sauvegarder le jeu ainsi que terminer la session proprement
        void OnApplicationQuit()
        {
            if (!isLogged) return;
            if(!this.appQuit)
                Application.CancelQuit();
            if (sm.isSaving) return;
            StartCoroutine(this.EndSession());
        }

    }
}