using Assets.Scripts.Connexion;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    //Classe utilisée lorsque un joueur sort de la zone de jeu, téléporte le joueur dans la zone de départ et envoie un log au serveur
    public class Teleporteur : MonoBehaviour
    {
        public GameObject destroyEffect;
        public GameObject player;
        private ConnexionController _connexion;

        void Start()
        {
            _connexion = FindObjectOfType<ConnexionController>();
        }

        void OnTriggerEnter2D(Collider2D coll)
        {
            Destroy(coll.gameObject);
            Instantiate(destroyEffect, coll.transform.position, coll.transform.rotation);
            Instantiate(player, GameObject.Find("Checkpoint0").transform.position, Quaternion.identity).name = "Player";
            StartCoroutine(_connexion.PostLog("Entree dans une zone interdite", "Labyrinthe", new LogLabyrinthe()));
            print("passe");

        }
    }
}
