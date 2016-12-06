using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Management
{
    //Cette classe définit un personnage, et définit ses indices de satisfaction
    public class Personnage : MonoBehaviour
    {
        public GameManager gameManager;
        public SaveChildrens saveChildren;

        public int persoId;
        public int serviceId;
        public string role = "Machin";
        public int nbrSalaries = 4;
        public float surfaceSalarie = 5;
        public int luminosite = 3;
        public bool accesExterieur = false;
        public int distanceSallePause = 4;
        public float distanceToilette = 2f;

        public SpriteRenderer sr;
        public SatisfactionPersonnage Satisfaction { get; private set; }
        public Avatar avatar;
        public Personnage copain;
        public Personnage bienAimePar;
        public Personnage myProductiveLink;
        public Personnage charIMakeProductive;
        public Piece piece;

        void Start()
        {
            if (!gameManager)
                gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            sr = GetComponent<SpriteRenderer>();
            Satisfaction = GetComponent<SatisfactionPersonnage>();
        }

        //Permet de selectionner le personnage
        void OnMouseDown()
        {
            gameManager.SelectedGameObject = this.gameObject;
        }

        //Methode à appeller afin de définir un avatar
        public void setAvatar(int persoId)
        {
            /*string spriteName = "Management/Avatars/avatar-" + persoId;
            sr.sprite = Resources.Load<Sprite>(spriteName);
            avatar = GameObject.Find("avatar-" + persoId).GetComponent<Avatar>();
            this.transform.GetChild(0).transform.localScale = new Vector2(0.7f,1.6f);
            LogManagement data = new LogManagement(avatar.persoId,this.persoId);
            StartCoroutine(gameManager.connexion.PostLog("Mise à jour d'un avatar", "Management", data));*/
            sr = gameObject.GetComponent<SpriteRenderer>();
            saveChildren = GameObject.Find("GrillePersonnages").GetComponent<SaveChildrens>();
            sr.sprite = saveChildren.avatars[persoId];
        }

        //Calcule la satisfaction du personnage
        public void CalculSatisfaction()
        {
            StartCoroutine(Satisfaction.CalculSatisfaction());
        }

        //Réinitialise la satisfaction du personnage
        public void ResetSatisfaction()
        {
            this.Satisfaction.Reset();
        }

        //Sauvegarde les spécificitées du personnage
        /*public void Save(string fileName)
        {
            ES2.Save(this.persoId, fileName + "?tag=" + name + "id");
            ES2.Save(this.role, fileName + "?tag=" + name + "role");
            ES2.Save(this.nbrSalaries, fileName + "?tag=" + name + "nbrSalaries");
            ES2.Save(this.surfaceSalarie, fileName + "?tag=" + name + "surfaceSalarie");
            ES2.Save(this.luminosite, fileName + "?tag=" + name + "luminosite");
            ES2.Save(this.accesExterieur, fileName + "?tag=" + name + "accesExterieur");
            ES2.Save(this.distanceSallePause, fileName + "?tag=" + name + "distanceSallePause");
            ES2.Save(this.distanceToilette, fileName + "?tag=" + name + "distanceToilette");
            ES2.Save(this.avatar ? this.avatar.gameObject.name : "", fileName + "?tag=" + name + "avatar");
            ES2.Save(this.transform.position, fileName + "?tag=" + name + "position");
            Debug.Log("Sauvegarde du personnage" + persoId);
        }

        //Charge le personnage
        public void Load(string fileName)
        {
            persoId = ES2.Load<int>(fileName + "?tag=" + name + "id");
            role = ES2.Load<string>(fileName + "?tag=" + name + "role");
            nbrSalaries = ES2.Load<int>(fileName + "?tag=" + name + "nbrSalaries");
            surfaceSalarie = ES2.Load<float>(fileName + "?tag=" + name + "surfaceSalarie");
            luminosite = ES2.Load<int>(fileName + "?tag=" + name + "luminosite");
            accesExterieur = ES2.Load<bool>(fileName + "?tag=" + name + "accesExterieur");
            distanceSallePause = ES2.Load<int>(fileName + "?tag=" + name + "distanceSallePause");
            distanceToilette = ES2.Load<float>(fileName + "?tag=" + name + "distanceToilette");
            string avatarName = ES2.Load<string>(fileName + "?tag=" + name + "avatar");
            if (avatarName != "")
            {
                SaveChildrens script = GetComponentInParent<SaveChildrens>();
                foreach (GameObject go in script.avatars.Where(go => go.name == avatarName))
                {
                    this.avatar = go.GetComponent<Avatar>();
                    GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Management/Avatars/" + avatarName);
                    Destroy(go);
                }
            }
            this.transform.position = ES2.Load<Vector3>(fileName + "?tag=" + name + "position");
            Debug.Log("Chargement des données personnage" + persoId);
        }*/
    }
}
