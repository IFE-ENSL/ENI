using System;
using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;

//Règles de collision pour les bords du chemin du labyrinthe
namespace Assets.Scripts.Labyrinthe
{
    public class CollidersRules : MonoBehaviour
    {

        public GameObject player;
        public GameObject destroyEffect;
        private ConnexionController _connexion;
        private StressTimer _timer;
        private LabyPlayerData _playerData;
        void Start()
        {
            _connexion = FindObjectOfType<ConnexionController>();
            _timer = FindObjectOfType<StressTimer>();
            _playerData = GameObject.Find("PlayerData").GetComponent<LabyPlayerData>();
        }
        //Envoie d'un log, mort du joueur,destruction, reset de certaines variables, instantiation d'un nouveau joueur, reset du timer de départ joueur
        void OnCollisionEnter2D(Collision2D coll)
        {
            LogLabyrinthe logLaby = new LogLabyrinthe(Convert.ToString(_playerData.cheminChoisi),_playerData.zoneActuelle, _timer.timerDepartJoueur,_timer.timerDebutJeu,null,null);
            StartCoroutine(_connexion.PostLog("Mort du joueur", "Labyrinthe", logLaby));
            Destroy(coll.gameObject);
            if (_playerData.lastCheckPoint != 0)
                _playerData.nbrMortsAfterFirstCheckpoint++;
            _playerData.startMoving = false;
            Instantiate(destroyEffect, coll.transform.position, coll.transform.rotation);
            Instantiate(player, GameObject.Find("Checkpoint" + _playerData.lastCheckPoint).transform.position,Quaternion.identity).name = "Player";
            _timer.ResetDepartJoueurTimer();
        }
    }
}
