using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Cette classe permet d'afficher une liste de pièces au milieu de l'écran ( jusqu'à 3 pièces) et de choisir le type de pièce (argent ou or)
namespace Assets.Scripts.Utility
{
    public class WinScript : MonoBehaviour
    {

        private readonly Vector3[] _startPositions = new[] { new Vector3(-20.62f,-7.74f), new Vector3(-13.91f,-7.74f), new Vector3(-7.14f,-7.74f) };
        public GameObject goldCoin;
        public GameObject ironCoin;
        public GameObject[] UItoDeactivate;
        

        void Start()
        {
            //StartCoroutine(Win(3));

        }
        //Fait apparaitre les pièces, le nombre passé en paramètre définit le nombre de pièces en or
        public IEnumerator Win(int nbrCoinGold)
        {
            foreach (GameObject GO in UItoDeactivate)
            {
                GO.SetActive(false);
            }

            if (SceneManager.GetActiveScene().name == "Management")
            {
                this.GetComponent<Image>().enabled = false;
                GameObject.Find("GrillePersonnages").GetComponent<SpriteRenderer>().enabled = false;
            }

            int cptBoucle = 0;
            for (int i = 0; i < nbrCoinGold ; i++)
            {
                Instantiate(goldCoin, _startPositions[i], Quaternion.identity);
                yield return new WaitForSeconds(1f);
                cptBoucle++;
            }
            for (int i = cptBoucle; i < 3 ; i++)
            {
                Instantiate(ironCoin, _startPositions[i], Quaternion.identity);
                yield return new WaitForSeconds(1f);
            }


        }
    }
}
