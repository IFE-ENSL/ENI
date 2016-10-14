using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    //Cette classe est utilisée par le menu du labyrinthe
    public class LabyMenu : MonoBehaviour
    {
        public StressTimer stressTimer;
        private LabyPlayerData _playerData;
        private ConnexionController _connexion;
        public GameObject player;
        public GameObject menu;

        void Start()
        {
            _playerData = GameObject.Find("PlayerData").GetComponent<LabyPlayerData>();
            _connexion = FindObjectOfType<ConnexionController>();
        }

        //Réinitialise les différents paramètres du joueur
        public void ResetGame()
        {
            stressTimer.StopTimer = true;
            //stressTimer.ResetDepartJoueurTimer();
            _playerData.cheminChoisi = 0;
            _playerData.lastCheckPoint = 0;
            _playerData.nbrMortsAfterFirstCheckpoint = 0;
            _playerData.zoneActuelle = "B1";
            Destroy(GameObject.Find("Player"));
            Instantiate(player, GameObject.Find("Checkpoint0").transform.position, Quaternion.identity).name = "Player";
            LogLabyrinthe logLaby = new LogLabyrinthe();
            StartCoroutine(_connexion.PostLog("Reinitialisation", "Labyrinthe", logLaby));
            menu.SetActive(false);
            _playerData.startMoving = false;
            stressTimer.Reset();
            stressTimer.clockText.text = "00:00";
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                menu.SetActive(!menu.activeInHierarchy);
            }
        }
    }
}
