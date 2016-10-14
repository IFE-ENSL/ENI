using System;
using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    //Permet de gérer les checkpoint du labyrinthe
    public class LabyCheckpoint : MonoBehaviour
    {

        public int numCheckpoint;
        private LabyPlayerData playerData;
        private ConnexionController _connexion;
        private StressTimer _stressTimer;

        void Start()
        {
            playerData = GameObject.Find("PlayerData").GetComponent<LabyPlayerData>();
            _connexion = FindObjectOfType<ConnexionController>();
            _stressTimer = FindObjectOfType<StressTimer>();
        }
        //Lorsque le joueur rentre dans un checkpoint, on le mémorise
        void OnTriggerEnter2D(Collider2D coll)
        {
            if (playerData.lastCheckPoint != numCheckpoint)
            {
                LogLabyrinthe logLaby = new LogLabyrinthe
                {
                    duree = _stressTimer.timerDepartJoueur,
                    dureeDebutJeu = _stressTimer.timerDebutJeu,
                    numCheckpoint = Convert.ToString(numCheckpoint)
                };
                StartCoroutine(_connexion.PostLog("Passage checkpoint", "Labyrinthe", logLaby));
            }
            playerData.lastCheckPoint = numCheckpoint;
        }
    }
}
