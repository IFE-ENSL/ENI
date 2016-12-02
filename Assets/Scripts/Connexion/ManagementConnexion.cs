using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Classe utilisée afin de gérer les connexions du mini jeu managament
namespace Assets.Scripts.Connexion
{
    public class ManagementConnexion : MonoBehaviour
    {


        private const string getPiecesURL = ConnexionController.baseURL + "/web/app_dev.php/unity/management/getPieces";
        private const string getPiecesDistancesURL = ConnexionController.baseURL + "/web/app_dev.php/unity/management/getPiecesDistances";
        private const string getPersonnagesURL = ConnexionController.baseURL + "/web/app_dev.php/unity/management/getPersonnages";
        private const string insertSessionURL = ConnexionController.baseURL + "/web/app_dev.php/unity/management/insertSessionPersonnage";
        private const string insertSessionMiniJeuURL = ConnexionController.baseURL + "/web/app_dev.php/unity/management/insertNewSessionMiniJeu";
        private const string updateAvatarURL = ConnexionController.baseURL + "/web/app_dev.php/unity/management/updateAvatar";

        void Start()
        {
        }


        void Update ()
        {
        }


        public IEnumerator getPiecesDistance(Waiter waiter, int scene)
        {
            //Testing out pieces distance
            waiter.waiting = true;
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
            string post_url = getPiecesDistancesURL + "/" + scene;
            print(post_url);
            WWW hs_get = new WWW(post_url, null, headers);
            yield return hs_get;
            if (hs_get.error != null)
            {
                Debug.Log("Erreur lors de la récupération des pieces : " + hs_get.error);
                Debug.Log(hs_get.text);
                SceneManager.LoadScene(0);
            }
            waiter.data = hs_get.text;
            waiter.waiting = false;
        }

        //Récupère la liste des pièces en json
        public IEnumerator getPieces(Waiter waiter, int scene)
        {
            waiter.waiting = true;
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };
            string post_url = getPiecesURL + "/" + scene;
            print(post_url);
            WWW hs_get = new WWW(post_url, null, headers);
            yield return hs_get;
            if (hs_get.error != null)
            {
                Debug.Log("Erreur lors de la récupération des pieces : " + hs_get.error);
                Debug.Log(hs_get.text);
                SceneManager.LoadScene(0);
            }
            waiter.data = hs_get.text;
            waiter.waiting = false;
        }

        //Récupère la liste des personnages en json
        public IEnumerator getPersonnages(Waiter waiter, int sessionMiniJeuId)
        {
            //TODO : Add MiniGame Session Id each time we catch the characters
            waiter.waiting = true;
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = getPersonnagesURL;
            WWWForm post_data = new WWWForm();
            post_data.AddField("sessionMiniJeuId", sessionMiniJeuId);
            WWW hs_get = new WWW(post_url, post_data.data, headers);
            yield return hs_get;
            if (hs_get.error != null)
            {
                Debug.Log("Erreur lors de la récupération des personnages : " + hs_get.error);
                Debug.Log(hs_get.text);
                SceneManager.LoadScene(0);
            }
            waiter.data = hs_get.text;
            waiter.waiting = false;
        }

        //Permet de lier dans la base de donnée un personnage séléctionné ainsi qu'une session de mini jeu
        public IEnumerator insertSessionPersonnage(string personnageId, int sessionMiniJeu)
        {
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = insertSessionURL;
            WWWForm post_data = new WWWForm();
            post_data.AddField("personnageId",personnageId);
            post_data.AddField("sessionMiniJeu", sessionMiniJeu);
            WWW hs_get = new WWW(post_url, post_data.data, headers);
            yield return hs_get;
            if (hs_get.error != null || hs_get.text != "1")
            {
                Debug.Log("Erreur lors de l'insertion de la session : " + hs_get.error);
                Debug.Log(hs_get.text);
                SceneManager.LoadScene(0);
            }
        }

        //Permet d'insérer une nouvelle session de mini jeu
        public IEnumerator insertSessionMiniJeu(Waiter wait)
        {
            wait.waiting = true;
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = insertSessionMiniJeuURL;
            WWW hs_get = new WWW(post_url, null, headers);
            yield return hs_get;
            if (hs_get.error != null)
            {
                Debug.Log("Erreur lors de la création de la session : " + hs_get.error);
                Debug.Log(hs_get.text);
                SceneManager.LoadScene(0);
            }
            wait.data = hs_get.text;
            wait.waiting = false;
        }

        //Permet de mettre a jour un lien personnage / sessionMiniJeu en ajoutant un id d'avatar
        public IEnumerator updateAvatar(int persId, int avatarId, int sessionMiniJeuId)
        {
            string sessionId = PlayerPrefs.GetString("sessionId");
            Dictionary<string, string> headers = new Dictionary<string, string> { { "Cookie", sessionId } };

            string post_url = updateAvatarURL;
            WWWForm post_data = new WWWForm();
            post_data.AddField("personnageId", persId);
            post_data.AddField("avatarId", avatarId);
            post_data.AddField("sessionMiniJeuId", sessionMiniJeuId);
            WWW hs_get = new WWW(post_url, post_data.data, headers);
            yield return hs_get;
            if (hs_get.error != null || hs_get.text != "1")
            {
                Debug.Log("Erreur lors de la mise à jour d'un avatar : " + hs_get.error);
                Debug.Log(hs_get.text);
                SceneManager.LoadScene(0);
            }
        }
    }
}
