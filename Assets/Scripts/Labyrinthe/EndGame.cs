using System;
using System.Collections;
using Assets.Scripts.Connexion;
using Assets.Scripts.SaveSystem;
using Assets.Scripts.SaveSystem.Labyrinthe;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

//Script de fin du jeu, quand le joueur arrive sur la ligne d'arrivée
namespace Assets.Scripts.Labyrinthe
{
    public class EndGame : MonoBehaviour
    {
        public GameObject bravo1;
        public GameObject bravo2;

        private ConnexionController _connexion;
        private StressTimer _timer;
        private WinScript _winScript;
        private LabyPlayerData _playerData;
        private PieceRobot _pieceRobot;

        void Start()
        {
            _connexion = FindObjectOfType<ConnexionController>();
            _timer = FindObjectOfType<StressTimer>();
            _winScript = GetComponent<WinScript>();
            GameObject playerData = GameObject.Find("PlayerData");
            if (!playerData)
            {
                Debug.LogError("Système de sauvegarde non activé");
                //SceneManager.LoadScene(0);
            }
            else
            {
                _playerData = playerData.GetComponent<LabyPlayerData>();
                _pieceRobot = playerData.GetComponent<PieceRobot>();
            }

            //StartCoroutine(_winScript.Win(2));
            //txt_finDuJeu.SetActive(true);

        }

        public void win (Collider2D coll)
        {
            GameObject.Find("Menu").SetActive(false);
            this.SaveData();
            StartCoroutine(_winScript.Win(_playerData.nbrCoins));
            if (_playerData.nbrCoins == 3)
                bravo1.SetActive(true);
            else
                bravo2.SetActive(true);
            coll.gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            Waiter wait = new Waiter();
            _timer.StopTimer = true;
            LogLabyrinthe logLaby = new LogLabyrinthe
            {
                duree = _timer.timerDepartJoueur,
                dureeDebutJeu = _timer.timerDebutJeu,
                nbrEtoiles = Convert.ToString(_playerData.nbrCoins),
                chemin = Convert.ToString(_playerData.cheminChoisi),

            };

            StartCoroutine(_connexion.PostLog("Fin du jeu", "Labyrinthe", logLaby, wait));
            StartCoroutine(waitBeforeLeave(wait));
        }

        //Le joueur arrive sur la ligne d'arrivée, on stop les timers 
        void OnTriggerEnter2D(Collider2D coll)
        {
            win(coll);
        }
        //On compte le nombre d'étoiles qu'a obtenu le joueur, et on sauvegarde les données
        private void SaveData()
        {
            TimeSpan timeTimer = new TimeSpan(0,0,_timer.minutes,_timer.seconds);

            switch (_playerData.cheminChoisi)
            {
                case 0:
                    _playerData.nbrCoins = 1;
                    GameObject.FindGameObjectWithTag("CharacterSheetManager").GetComponent<CharacterSheetManager>().AddQualityStep(1, "mini-jeu 01");
                    break;
                case 1:
                    if (_playerData.nbrMortsAfterFirstCheckpoint <= 2)
                    {
                        GameObject.FindGameObjectWithTag("CharacterSheetManager").GetComponent<CharacterSheetManager>().AddQualityStep(3, "mini-jeu 01");
                        _playerData.nbrCoins = 3;
                    }
                    else
                    {
                        GameObject.FindGameObjectWithTag("CharacterSheetManager").GetComponent<CharacterSheetManager>().AddQualityStep(2, "mini-jeu 01");
                        _playerData.nbrCoins = 2;
                    }
                    break;
            }
            if(_playerData.meilleurTemps[0] == 0 && _playerData.meilleurTemps[1] == 0)
                _playerData.meilleurTemps = new[] { _timer.minutes, _timer.seconds };
            else if (TimeSpan.Compare(timeTimer, new TimeSpan(0,0,_playerData.meilleurTemps[0],_playerData.meilleurTemps[1])) != 1)
            {
                _playerData.meilleurTemps = new[] { _timer.minutes, _timer.seconds };
            }
            switch (_playerData.nbrCoins)
            {
                case 1:
                    _pieceRobot.Jambes = (int) TypePieceRobot.Bronze;
                    break;
                case 2:
                    _pieceRobot.Jambes = (int) TypePieceRobot.Argent;
                    break;
                case 3:
                    _pieceRobot.Jambes = (int) TypePieceRobot.Or;
                    break;
            }
        }
        //Permet d'attendre la fin des requêtes avant d'arriver sur la scene PieceRobot du jeu
        private IEnumerator waitBeforeLeave(Waiter wait)
        {
            yield return new WaitForSeconds(5f);
            while (wait.waiting != false)
            {
                print("Waiting");
                yield return new WaitForSeconds(0.1f);
            }
            SceneManager.LoadScene("PieceRobot");
        }
    }
}
