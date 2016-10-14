using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utility
{
    //Cette classe permet de simuler une écriture manuelle de texte
    public class TextWriter : MonoBehaviour
    {

        public float letterPause = 0.1f;

        string message;
        public Text textComp;

        void Start()
        {
            if(!textComp)
                textComp = GetComponent<Text>();
            message = textComp.text;
            textComp.text = "";
            if(message != "")
                StartCoroutine(TypeText());
        }

        public void setText(string msg)
        {
            textComp.text = "";
            message = msg;
            StartCoroutine(TypeText());
        }

        IEnumerator TypeText()
        {
            foreach (char letter in message)
            {
                textComp.text += letter;
                yield return new WaitForSeconds(letterPause);
            }
        }
    }
}