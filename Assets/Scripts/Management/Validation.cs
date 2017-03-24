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
        public GameObject warningWindow;
        public SatisfactionGlobale globalSatisfaction;
        public GameObject[] imagesDirecteur;
        private PieceRobot _pieceRobot;
        private WinScript _winScript;
        public GameManager gameManager;

        [SerializeField]
        int[] compGenToAddPoint;
        [SerializeField]
        int qualityNumberToAddPoints;

        [SerializeField]
        int GeneralSkill;
        [SerializeField]
        int CriteriaNumber;

        void Start()
        {
            _pieceRobot = GameObject.Find("PlayerData").GetComponent<PieceRobot>();
            _winScript = GetComponent<WinScript>();
            warningWindow.SetActive(false);
        }
        public void Validate()
        {


            int nbrPers = gameManager.rooms.Count(room => room.managementCharacter);
            if (nbrPers == 5)
            {
                if (globalSatisfaction.satisfactionGlobale <= 73)
                {
                    _pieceRobot.Bras = (int)TypePieceRobot.Bronze;
                    GameObject.FindGameObjectWithTag("CharacterSheetManager").GetComponent<CharacterSheetManager>().AddQualityStep(1, "mini-jeu 02");
                }
                else if (globalSatisfaction.satisfactionGlobale <= 75f)
                {
                    _pieceRobot.Bras = (int)TypePieceRobot.Argent;
                    GameObject.FindGameObjectWithTag("CharacterSheetManager").GetComponent<CharacterSheetManager>().AddQualityStep(2, "mini-jeu 02");
                }
                else
                {
                    _pieceRobot.Bras = (int)TypePieceRobot.Or;
                    GameObject.FindGameObjectWithTag("CharacterSheetManager").GetComponent<CharacterSheetManager>().AddQualityStep(3, "mini-jeu 02");
                }
                StartCoroutine(_winScript.Win((int) _pieceRobot.Bras + 1));
                imagesDirecteur[(int) _pieceRobot.Bras].SetActive(true);
                StartCoroutine(LoadNextLevel());
            }
            else
            {
                print("Il n'y a pas 5 personnes");
                warningWindow.SetActive(true);
            }
        }

        public IEnumerator LoadNextLevel()
        {
            yield return new WaitForSeconds(5f);
            SceneManager.LoadScene("PieceRobot");
        }
    }
}
