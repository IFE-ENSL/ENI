using System;
using Assets.Scripts.Connexion;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Chat
{
    //Cette classe permet de gérer un chat en utilisant une base de donnée
    public class ChatBoxFunctionsSQL : MonoBehaviour {

        [SerializeField]
        ContentSizeFitter contentSizeFitter;
        [SerializeField]
        Text showHideButtonText;
        public bool isChatShowing = false;
        public InputField inputField;
        public Text chatText;
        private ConnexionController connexionController;
        private int lastIdMessage;
        void Start()
        {
            ToggleChat();
            connexionController = FindObjectOfType<ConnexionController>();
            //InvokeRepeating("updateMessages", 0, 2.0f); //Check new messages every X seconds
            
        }

        void Update ()
        {
            
            updateMessages(); //Check new messages each rendered frame
        }

        //Va chercher dans la base de donnée si de nouveaux messages ont été postés
        void updateMessages()
        {
            if (connexionController.wait != false)
            {
                Debug.Log("Can't update messages now, have to wait...");
                return;
            }

            float timeToAnswer = 0f;
            StartCoroutine(connexionController.getMessages());
            timeToAnswer += Time.deltaTime;

            JSONNode messages = null;
            try
            {
                messages = JSON.Parse(connexionController.messages);
            }
            catch (Exception)
            {
                messages = null;
                Debug.LogError("Erreur lors de la récupération des messages");
            }
            if (messages == null) return;

            for (int i = 0; i < messages.Count; i++)
            {
                if (messages[i]["id"].AsInt <= lastIdMessage) continue;

                string message = "[ " + messages[i]["date"] + " ]" + "[ " + messages[i]["username"] + " ] " + messages[i]["message"];
                chatText.text += message + "\n";
                lastIdMessage = messages[i]["id"].AsInt;
            }
        }
        //Affiche / Cache le tchat
        public void ToggleChat()
        {
            isChatShowing = !isChatShowing;
            if (isChatShowing)
            {
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                showHideButtonText.text = "Cacher";
            }
            else
            {
                contentSizeFitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
                showHideButtonText.text = "Afficher";
            }
        }
        //Envoie un message au serveur
        public void SetMessage()
        {
            if (inputField.text == "") return;

            string text = inputField.text;
            inputField.text = "";
            //string message = "[" + PlayerPrefs.GetString("username") + "]" + " " + text;
            StartCoroutine(connexionController.PostMessage(text));
        }
    }
}
