using System;
using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    //Cette classe est utilisée afin de détecter les changements de zone
    public class LabyZoneChanger : MonoBehaviour
    {
        private LabyPlayerData _playerData;
        private ConnexionController _connexion;
        private StressTimer _timer;


        void Start()
        {
            _playerData = GameObject.Find("PlayerData").GetComponent<LabyPlayerData>();
            _connexion = FindObjectOfType<ConnexionController>();
            _timer = FindObjectOfType<StressTimer>();

        }

        //Lorsque l'on change de zone, on envoie un log à la base de donnée
        void OnTriggerEnter2D(Collider2D coll)
        {
            if (this.gameObject.name.Contains(":"))
            {
                _playerData.zoneActuelle = this.gameObject.name.Split(':')[_playerData.cheminChoisi];
            }
            else
            {
                _playerData.zoneActuelle = this.gameObject.name;
            }
            LogLabyrinthe logLaby = new LogLabyrinthe(Convert.ToString(_playerData.cheminChoisi), _playerData.zoneActuelle,
                _timer.timerDepartJoueur, _timer.timerDebutJeu, null, null);
            print(_timer.timerDepartJoueur);
            StartCoroutine(_connexion.PostLog("Changement de zone","Labyrinthe",logLaby));

        }
    }
}
