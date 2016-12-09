using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Management
{
    //Tutoriel au lancement du mini jeu
    public class Introduction : MonoBehaviour
    {

        public Text text;
        public GameObject[] gameObjects;

        void Start()
        {
            StartCoroutine(StartIntroduction());
        }

        public IEnumerator StartIntroduction()
        {
            text.text = "Bienvenue dans le mini jeu {NOM}";
            yield return new WaitForSeconds(2f);
            text.text = "Ce tutoriel va vous montrer les bases du mini jeu";
            yield return new WaitForSeconds(2f);
            gameObjects[0].SetActive(true);
            text.text = "Voici ci-dessus le terrain du mini jeu, le but du mini jeu va être de placer des personnages dans des pièces.";
            yield return new WaitForSeconds(4f);
            gameObjects[0].SetActive(false);
            text.text = "Voici la grille de selection de managementCharacter, il suffira de cliquer sur un managementCharacter pour le séléctionner et voir ses caractéristiques";
            gameObjects[1].GetComponent<SpriteRenderer>().enabled = true;
            yield return new WaitForSeconds(4f);
            text.text = "Pour l'instant, chaque managementCharacter va être représenté par un '?'";
            foreach (Transform child in gameObjects[1].transform)
            {
                child.GetComponent<SpriteRenderer>().enabled = true;
            }
            yield return new WaitForSeconds(2f);
            text.text = "Il faudra remplacer ce '?' par un avatar, en cliquant sur l'image managementCharacter du menu de droite";
            gameObjects[2].SetActive(true);
            gameObjects[3].SetActive(true);
            yield return new WaitForSeconds(4f);
            text.text = "Une fois l'avatar séléctionné, il suffira de déplacer le managementCharacter dans une des pièce";
            gameObjects[1].SetActive(false);
            gameObjects[2].SetActive(false);
            gameObjects[3].SetActive(false);
            gameObjects[0].SetActive(true);
            foreach (Transform child in gameObjects[4].transform)
            {
                child.GetChild(1).GetComponent<SpriteRenderer>().color = new Color(1f, 0.3f, 0.8f, 0.2f);
                yield return new WaitForSeconds(0.5f);
            }
            text.text = "Attention : Chaque managementCharacter devra être placé à un endroit stratégique afin que chaque managementCharacter soit le plus satisfait possible";
            yield return new WaitForSeconds(2f);
            text.text =
                "L'écran en haut à gauche affichera le pourcentage total de satisfaction de tout les personnages placés sur la carte";
            gameObjects[5].SetActive(true);
            yield return new WaitForSeconds(2f);
            text.text =
                "Et le bouton situé en bas à gauche permettra de valider le placement des personnages une fois le mini jeu terminé";
            gameObjects[6].SetActive(true);
            yield return new WaitForSeconds(2f);
            text.text = "Essayez de faire le meilleur score, bonne chance ! ";
            foreach (GameObject go in gameObjects)
            {
                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if (sr)
                    sr.enabled = true;
                go.SetActive(true);
            }
            yield return new WaitForSeconds(2f);
            text.gameObject.SetActive(false);





        }
    }
}
