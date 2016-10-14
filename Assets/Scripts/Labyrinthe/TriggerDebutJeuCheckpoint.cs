using System;
using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    //Permet de savoir quand le joueur part pour la prémière fois de la zone de départ
    public class TriggerDebutJeuCheckpoint : MonoBehaviour
    {

        public int numCheckpoint;
        private ConnexionController _connexion;
        private LabyPlayerData _playerData;
        private StressTimer _timer;
        void Start()
        {
            _playerData = GameObject.Find("PlayerData").GetComponent<LabyPlayerData>();
            _connexion = FindObjectOfType<ConnexionController>();
            _timer = FindObjectOfType<StressTimer>();
        }


        void OnTriggerEnter2D(Collider2D coll)
        {
            if (_playerData.startMoving == false)
            {
                LogLabyrinthe logLaby = new LogLabyrinthe
                {
                    chemin = Convert.ToString(_playerData.cheminChoisi),
                    dureeDebutJeu = _timer.timerDebutJeu,
                    numCheckpoint = Convert.ToString(this.numCheckpoint)
                };
                _timer.ResetDepartJoueurTimer();
                StartCoroutine(_connexion.PostLog("Départ du joueur,", "Labyrinthe", logLaby));
                _playerData.startMoving = true;
            }
        }
    }
}
