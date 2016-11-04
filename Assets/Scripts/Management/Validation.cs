using System.Collections;
using System.Linq;
using Assets.Scripts.SaveSystem;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Management
{
    //Cette classe sert à valider la grille de jeu lorsque l'élève a terminé le mini jeu
    public class Validation : MonoBehaviour
    {

        public SatisfactionGlobale sGlobale;
        public GameObject[] imagesDirecteur;
        private PieceRobot _pieceRobot;
        private WinScript _winScript;
        public GameManager gm;

        void Start()
        {
            _pieceRobot = GameObject.Find("PlayerData").GetComponent<PieceRobot>();
            _winScript = GetComponent<WinScript>();
        }
        public void Validate()
        {
            int nbrPers = gm.pieces.Count(piece => piece.personnage);
            if (nbrPers == 5)
            {
                if (sGlobale.satisfactionGlobale < 60)
                {
                    _pieceRobot.Bras = (int)TypePieceRobot.Bronze;

                }
                else if (sGlobale.satisfactionGlobale < 80)
                {
                    _pieceRobot.Bras = (int)TypePieceRobot.Argent;
                }
                else if (sGlobale.satisfactionGlobale > 80)
                {
                    _pieceRobot.Bras = (int)TypePieceRobot.Or;
                }
                StartCoroutine(_winScript.Win((int) _pieceRobot.Bras + 1));
                imagesDirecteur[(int) _pieceRobot.Bras].SetActive(true);
                StartCoroutine(LoadNextLevel());
            }
            else
            {
                print("Il n'y a pas 5 personnes");
            }
        }

        public IEnumerator LoadNextLevel()
        {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("PieceRobot");
        }
    }
}
