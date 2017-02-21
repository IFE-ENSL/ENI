using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Management
{
    //Cette classe définit un managementCharacter, et définit ses indices de satisfaction
    public class ManagementCharacter : MonoBehaviour
    {
        public GameManager gameManager;
        public CharacterGrid saveChildren;

        public int persoId;
        public int serviceId;
        public string role = "[Service Name]";
        public float surfaceSalarie = 5;
        public int luminosite = 3;
        public bool accesExterieur = false;
        public int distanceSallePause = 4;
        public float distanceToilette = 2f;

        public SpriteRenderer sr;
        public SatisfactionPersonnage Satisfaction { get; private set; }
        public ManagementCharacter friend;
        public ManagementCharacter likedBy;
        public ManagementCharacter myProductiveLink;
        public ManagementCharacter charIMakeProductive;
        public Room room;
        public SpriteRenderer satisfactionIcon;
        public Sprite[] satisfactionSprites;

        void Start()
        {
            if (!gameManager)
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

            sr = GetComponent<SpriteRenderer>();
            Satisfaction = GetComponent<SatisfactionPersonnage>();
            satisfactionIcon = transform.GetChild(1).GetComponent<SpriteRenderer>();
            satisfactionIcon.gameObject.SetActive(false);
        }

        //Permet de selectionner le managementCharacter
        void OnMouseDown()
        {
            gameManager.SelectedGameObject = this.gameObject;
        }

        //Sets a sprite as this character's avatar
        public void setAvatar(int persoId)
        {
            sr = gameObject.GetComponent<SpriteRenderer>();
            saveChildren = GameObject.Find("GrillePersonnages").GetComponent<CharacterGrid>();
            sr.sprite = saveChildren.avatarsSprites[persoId - 1];
        }

        void Update ()
        {
            //Let's display an icon to represent the satisfaction level of this character
            if (room != null)
            {
                if (!satisfactionIcon.gameObject.activeInHierarchy)
                    satisfactionIcon.gameObject.SetActive(true);

                if (Satisfaction.satisfactionTotale <= 35)
                {
                    satisfactionIcon.sprite = satisfactionSprites[0];
                }
                else if (Satisfaction.satisfactionTotale <= 45)
                {
                    satisfactionIcon.sprite = satisfactionSprites[1];
                }
                else if (Satisfaction.satisfactionTotale <= 75)
                {
                    satisfactionIcon.sprite = satisfactionSprites[2];
                }
                else if (Satisfaction.satisfactionTotale <= 100)
                {
                    satisfactionIcon.sprite = satisfactionSprites[3];
                }
            }
            else
            {
                if (satisfactionIcon.gameObject.activeInHierarchy)
                    satisfactionIcon.gameObject.SetActive(false);
            }
        }

        //Calcule la satisfaction du managementCharacter
        public void UpdateSatisfaction()
        {
            StartCoroutine(Satisfaction.UpdateSatisfaction());
        }

        //Réinitialise la satisfaction du managementCharacter
        public void ResetSatisfaction()
        {
            this.Satisfaction.Reset();
        }
    }
}
