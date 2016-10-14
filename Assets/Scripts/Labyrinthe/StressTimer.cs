using Assets.Scripts.SaveSystem.Labyrinthe;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Labyrinthe
{
    //Cette classe gère le timer présent en haut à droite du mini jeu labyrinthe
    public class StressTimer : MonoBehaviour
    {
        //Timer affiché au joueur
        private float startTimer;
        public Text clockText;

        //Timer depuis le départ du joueur
        public string timerDepartJoueur;
        private float startTimerDepartJoueur;

        //Timer depuis le début du jeu
        public string timerDebutJeu;
        private float startTimerDebutJeu;

        private bool stopTimer = true;
        public int minutes { get; private set; }
        public int seconds { get; private set; }

        private LabyPlayerData _playerData;

        public bool StopTimer
        {
            get { return stopTimer; }
            set
            {
                if(value == true)
                    stopTimer = value;
                else
                {
                    startTimer = Time.time;
                    stopTimer = value;
                }
            }
        }

        void Start()
        {
            startTimer = Time.time;
            startTimerDepartJoueur = Time.time;
            startTimerDebutJeu = Time.time;
            timerDepartJoueur = "00:00:00";
            timerDebutJeu = "00:00:00";
            if(!clockText)
                clockText = GetComponent<Text>();
            _playerData = FindObjectOfType<LabyPlayerData>();
        }

        void Update()
        {
            if (clockText && !stopTimer)
            {
                var time = Time.time - startTimer;
                minutes = (int) time/60;
                seconds = (int) time%60;
                string stringTime = string.Format("{0:00}:{1:00}", minutes, seconds);
                clockText.text = stringTime;
            }

            if (_playerData.startMoving == true)
            {
                var timeHidden = Time.time - startTimerDepartJoueur;
                minutes = (int) timeHidden/60;
                seconds = (int) timeHidden%60;
                timerDepartJoueur = "00:" + string.Format("{0:00}:{1:00}", minutes, seconds);
            }


            var timeDebutJeu = Time.time - startTimerDebutJeu;
            minutes = (int)timeDebutJeu / 60;
            seconds = (int)timeDebutJeu % 60;
            timerDebutJeu = "00:" + string.Format("{0:00}:{1:00}", minutes, seconds);

        }

        public void Reset()
        {
            startTimer = Time.time;
        }

        public void ResetDepartJoueurTimer()
        {
            startTimerDepartJoueur = Time.time;
            timerDepartJoueur = "00:00:00";
        }

        public void ResetDebutJeuTimer()
        {
            startTimerDebutJeu = Time.time;
            timerDebutJeu = "00:00:00";
        }
    }
}
