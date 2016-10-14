using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Labyrinthe
{
    //Cette classe est utilisée afin de gérer la scène d'introduction du mini jeu
    public class IntroManager : MonoBehaviour
    {

        public GameObject[] images;
        public GameObject loadingImage;
        public String[] textToWrite;
        public float[] letterPause;
        private Text text;
        private bool isClicked;
        private bool isChangeFinished = false;

        void Start()
        {
            StartCoroutine(changeImage());
            StartCoroutine(loadScene());
            text = GameObject.Find("IntroText").GetComponent<Text>();
            text.text = textToWrite[0];
        }

        //Méthode principale pour le changement d'image
        public IEnumerator changeImage()
        {
            for (int i = 1; i < images.Length; i++)
            {
                int iterator = 0;
                while (!isClicked && iterator != 60)
                {
                    yield return new WaitForSeconds(0.1f);
                    iterator++;
                }
                if (isClicked)
                    isClicked = false;
                images[i - 1].SetActive(false);
                images[i].SetActive(true);
                text.text = textToWrite[i];
            }
            int it = 0;
            while (!isClicked && it != 60)
            {
                yield return new WaitForSeconds(0.1f);
                it++;
            }
            images.Last().SetActive(false);
            loadingImage.SetActive(true);
            this.isChangeFinished = true;

            //SceneManager.LoadScene("Labyrinthe");

        }

        //Une fois que l'intro est terminée la scène principale du mini jeu est chargée
        public IEnumerator loadScene()
        {

            AsyncOperation asc = SceneManager.LoadSceneAsync("Labyrinthe");
            asc.allowSceneActivation = false;
            yield return asc.isDone;
            while (isChangeFinished == false)
            {
                yield return new WaitForSeconds(0.1f);
            }
            asc.allowSceneActivation = true;

        }

        //Permet de savoir si l'utilisateur a cliqué sur le bouton de la souris afin de changer d'image
        void Update()
        {
            if (Input.GetMouseButtonDown(0))
                isClicked = true;
        }
    }
}
