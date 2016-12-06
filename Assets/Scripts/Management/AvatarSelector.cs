using Assets.Scripts.Connexion;
using UnityEngine;

namespace Assets.Scripts.Management
{
    //Cette classe gère le panel de séléction d'avatars
    public class AvatarSelector : MonoBehaviour
    {
        public GameObject avatarPanel;
        private GameManager gameManager;
        private ConnexionController _connexion;


        void Start()
        {
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            _connexion = GameObject.Find("ConnexionController").GetComponent<ConnexionController>();
        }

        //Ouverture du panneau de séléction d'un avatar
        public void openPanel()
        {
            if(!gameManager.SelectedGameObject.GetComponent<Personnage>().avatar)
                avatarPanel.SetActive(!avatarPanel.activeInHierarchy);
        }

        //Définit un avatar pour un personnage séléctionné
        /*public void setAvatar(string avatarName)
        {
            Personnage personnage = gameManager.SelectedGameObject.GetComponent<Personnage>();
            personnage.setAvatar(avatarName);
            GameObject.Find(avatarName).SetActive(false);
            avatarPanel.SetActive(!avatarPanel.activeInHierarchy);
            StartCoroutine(_connexion.mConnexion.updateAvatar(personnage.persoId, personnage.avatar.persoId,
                PlayerPrefs.GetInt("sessionMiniJeuManagement")));
            gameManager.UpdateDescription();
        }*/
    }
}
