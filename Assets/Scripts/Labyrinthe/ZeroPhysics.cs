using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    //Change la physique du jeu pour les mouvements du personnage, et initialise les propriétés du joueur
    public class ZeroPhysics : MonoBehaviour
    {
        private ConnexionController _connexion;
        private LabyPlayerData _playerData;
        void Start () {
            Physics2D.gravity = Vector2.zero;
            _connexion = FindObjectOfType<ConnexionController>();
            _playerData = GameObject.Find("PlayerData").GetComponent<LabyPlayerData>();
            _playerData.nbrEntreeJeu++;
            LogLabyrinthe logLaby = new LogLabyrinthe();
            StartCoroutine(_connexion.PostLog("Debut du jeu", "Labyrinthe",logLaby));
            _playerData.cheminChoisi = 0;
            _playerData.lastCheckPoint = 0;
            _playerData.nbrMortsAfterFirstCheckpoint = 0;
            _playerData.zoneActuelle = "B1";

        }
    }
}
