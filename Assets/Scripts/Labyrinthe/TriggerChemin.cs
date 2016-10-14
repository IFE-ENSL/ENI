using System;
using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    //Permet de connaitre le chemin qui a été pris par le joueur (Chemin facile, chemin difficile)
    public class TriggerChemin : MonoBehaviour
    {

        public StressTimer stressTimer;
        private LabyPlayerData _labyPlayerData;
        private ConnexionController _connexion;

        void Start()
        {
            if (!stressTimer)
                stressTimer = FindObjectOfType<StressTimer>();
            _labyPlayerData = GameObject.Find("PlayerData").GetComponent<LabyPlayerData>();
            _connexion = FindObjectOfType<ConnexionController>();
        }
     
        void OnTriggerEnter2D()
        {
            if (stressTimer.StopTimer)
                stressTimer.StopTimer = false;

            if (_labyPlayerData.startMoving == false)
            {
                stressTimer.ResetDepartJoueurTimer();
            }
            switch (this.gameObject.name)
            {
                case "CheminFacile":
                    _labyPlayerData.cheminChoisi = 0;
                    break;
                case "CheminDifficile":
                    _labyPlayerData.cheminChoisi = 1;
                    break;
            }
            _labyPlayerData.startMoving = true;
            LogLabyrinthe logLaby = new LogLabyrinthe
            {
                chemin = Convert.ToString(_labyPlayerData.cheminChoisi),
                dureeDebutJeu = stressTimer.timerDebutJeu,
                numCheckpoint = "0"
            };
            StartCoroutine(_connexion.PostLog("Départ du joueur", "Labyrinthe", logLaby));
        }
    }
}
