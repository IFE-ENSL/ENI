using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Connexion;
using Assets.Scripts.Utility;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.Management
{
    //Cette classe permet de gérer les objets séléctionnées dans le mini jeu et les fonctions de base
    //La gamemanager va se charger au lancement du mini jeu de charger toute les pièces depuis la base de données et de charger aléatoirement un certain nombre de personnages
    //Il va aussi servir lors du clic sur un objet du jeu à gérer l'affichage du déscriptif de celui ci

    public class GameManager : MonoBehaviour
    {
        //Définit le numéro de la scène afin de charger les pièces correspondantes (Pieces de la scene 1, pieces de la scene 2...)
        public int sceneId;
        //Le gameobject selectionné
        private GameObject _selectedGameObject;
        //Le panneau d'avatars
        public GameObject avatarPanel;
        //La grille de personnages
        public GameObject grillePersonnages;

        //The array of distances for each room from the resting room, ordered by distance
        public static List<float> roomDistanceFromRestRoom = new List<float>();
        public static List<float> roomDistanceFromBathRoom = new List<float>();

        //Le panel de description
        public GameObject descriptionPanel;
        //Permet de savoir si un personnage est en train de bouger dans le jeu
        public bool isDragging;
        public GameObject SelectedGameObject
        {
            get { return _selectedGameObject; }
            set
            {
                //Effectue des controles lors du changement de l'objet selectionné et met à jour le panneau de description 
                //ainsi que les indications sur scene permettant de voir plus facilement l'objet selectionné
                if (_selectedGameObject && _selectedGameObject.name == value.name) return;
                if (avatarPanel.activeSelf) return;
                if (!descriptionPanel.activeInHierarchy)
                    descriptionPanel.SetActive(true);
                if(_selectedGameObject)
                    _selectedGameObject.transform.GetChild(0).gameObject.SetActive(false);
                _selectedGameObject = value;
                value.transform.GetChild(0).gameObject.SetActive(true);
                UpdateDescription();

            }
        }
        //Utilitaire de connexion à la base
        public ConnexionController connexion;
        //Tableau contenant la liste des champs utilisés lors de l'affichage du déscriptif d'un personnage
        public Text[] descriptionPersonnage;
        //Tableau contenant la liste des champs utilisés lors de l'affichage du déscriptif d'une pièce
        public Text[] descriptionPiece;
        public Image imageAvatarBtn;
        //Liste des pièces de la scene
        public Piece[] pieces;
        //Tableau de positions pour les zones de départ des personnages
        private readonly Vector3[] positionsPersonnage = new[] {new Vector3(0.03999f,2.51f), new Vector3(-2.91f,2.51f),
            new Vector3(2.96f,2.51f),new Vector3(-2.94f,0.09f),new Vector3(0.07f,0.09f),new Vector3(0.07f,-2.646f),
        new Vector3(-2.9f,-2.57f)};
        //Gameobject contenant les éléments permettant la déscription d'un personnage
        public GameObject dscPers;
        //Gameobject contenant les éléments permettant la déscription d'une pièce
        public GameObject dscPiece;

        public GameObject personnagePrefab;

        //Contient l'identifiant de la session de mini jeu actuelle obtenue a chaque nouvelle partie du mini jeu
        private int sessionMiniJeu;
        //Permet de savoir si il faut charger le mini jeu depuis une sauvegarde ou commencer une nouvelle partie
        public bool newGame = true;


        void Start()
        {
            connexion = GameObject.Find("ConnexionController").GetComponent<ConnexionController>();
            StartCoroutine(this.getPieces());
            if (newGame == false)
                print("Pas de chargement");
            else
                StartCoroutine(this.getPersonnages());
            StartCoroutine(connexion.PostLog("Début du jeu", "Management", new LogManagement()));
        }

        //Récupère la liste des pièces depuis la base de donnée
        private IEnumerator getPieces()
        {
            Waiter wait = new Waiter();
            StartCoroutine(connexion.mConnexion.getPieces(wait,this.sceneId));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(1);
            }
            Debug.Log("Récupération des pièces : " + wait.data);
            JSONNode piecesValues = null;
            piecesValues = JSON.Parse(wait.data);
            if (pieces == null) yield break;
            //Itère a travers les pièces récupérées, et les insère dans l'ordre dans le tableau de pièces présent sur la scène
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].accesExterieur = piecesValues[i]["accesExterieur"].Value == "1" ? true : false;
                pieces[i].accesHandicape = piecesValues[i]["accesHandicape"].Value == "1" ? true : false;

                pieces[i].distanceSallePause = piecesValues[i]["distanceSallePause"].AsFloat;
                roomDistanceFromRestRoom.Add(pieces[i].distanceSallePause);
                roomDistanceFromRestRoom.Sort();
                roomDistanceFromRestRoom.Reverse();

                pieces[i].distanceToilette = piecesValues[i]["distanceToillette"].AsFloat;
                roomDistanceFromBathRoom.Add(pieces[i].distanceToilette);
                roomDistanceFromBathRoom.Sort();
                roomDistanceFromBathRoom.Reverse();

                pieces[i].surface = piecesValues[i]["surface"].AsFloat;
                pieces[i].ouvertureExterieur = piecesValues[i]["ouvertureExterieur"].AsInt;
            }
        }

        //Récupère une liste aléatoire de personnages depuis la base de donnée
        private IEnumerator getPersonnages()
        {
            Waiter wait = new Waiter();
            //Crée une nouvelle session de mini jeu
            StartCoroutine(connexion.mConnexion.insertSessionMiniJeu(wait));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(0.5f);
            }
            try
            {
                sessionMiniJeu = Convert.ToInt32(wait.data);
                PlayerPrefs.SetInt("sessionMiniJeuManagement",sessionMiniJeu);
            }
            catch (Exception)
            {
                Debug.Log("Erreur lors de l'insertion d'une nouvelle session");
                print("Data : " + wait.data);
                SceneManager.LoadScene(0);
            }
            wait.Reset();
            StartCoroutine(connexion.mConnexion.getPersonnages(wait));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log("Récupération des personnages : " + wait.data);
            JSONNode personnageValues = null;
            personnageValues = JSON.Parse(wait.data);
            if (pieces == null) yield break;
            List<int> ids = new List<int>();
            int idPlacement = 0;
            for (int i = 0; i < personnageValues.Count; i++)
            {
                //Crée un personnage, et associe les valeurs récupérées dans la base à ce personnage pour chaque itération
                if (!ids.Contains(personnageValues[i]["id"].AsInt))
                {
                    //Création du personnage
                    GameObject personnage = Instantiate(personnagePrefab);
                    personnage.transform.parent = grillePersonnages.transform;
                    personnage.name = "Personnage" + personnageValues[i]["id"].Value;
                    personnage.transform.localPosition = positionsPersonnage[idPlacement];
                    idPlacement++;
                    //Association des valeurs récupérées
                    Personnage scriptP = personnage.GetComponent<Personnage>();
                    scriptP.role = personnageValues[i]["role"].Value;
                    scriptP.nbrSalaries = personnageValues[i]["nbrSalaries"].AsInt;
                    scriptP.surfaceSalarie = personnageValues[i]["surfaceSalarie"].AsInt;
                    scriptP.luminosite = personnageValues[i]["luminosite"].AsInt;
                    scriptP.accesExterieur = (personnageValues[i]["accesExterieur"].Value == "1") ? true : false;
                    scriptP.distanceSallePause = personnageValues[i]["distanceSallePause"].AsInt;
                    scriptP.distanceToilette = personnageValues[i]["distanceToilette"].AsFloat;
                    scriptP.id = personnageValues[i]["id"].AsInt;
                    //Envoie du log indiquant l'insertion du personnage
                    StartCoroutine(connexion.mConnexion.insertSessionPersonnage(personnageValues[i]["id"].Value,
                        sessionMiniJeu));
                    //Ajoute l'identifiant du personnage à une liste afin de ne pas en crééer deux ayant le même identifiant
                    //(Si la requête SQL retourne deux personnages avec le même identifiant c'est parceque ils ne sont pas associés au même copain)
                    ids.Add(scriptP.id);
                }
            }
            //Associe les copain d'un personnage ainsi que les personnage qui vont être bien aimées par ce personnage
            //TO DO - Hey dood, so it's here that starts your new journey =)
            for (int i = 0; i < personnageValues.Count; i++)
            {
                Debug.Log("User id is " + personnageValues[i]["id"]);

                for (int iterator = 0; iterator < personnageValues[i]["links"].Count; iterator++)
                {
                    if (personnageValues[i]["links"][iterator]["linktype"].Value == "Friend" && personnageValues[i]["links"][iterator]["userfrom_id"].Value != "")
                    {
                        Personnage thisPersonnage = GameObject.Find("Personnage" + personnageValues[i]["id"]).GetComponent<Personnage>();
                        GameObject copain = GameObject.Find("Personnage" + personnageValues[i]["links"][iterator]["userto_id"].Value);
                        Debug.Log("Trying to add a copain...");
                        if (copain)
                        {
                            Personnage friend = copain.GetComponent<Personnage>();
                            thisPersonnage.copain = friend;
                            friend.bienAimePar = thisPersonnage;
                            Debug.Log("Added a copain, trop meugnon lol");
                        }
                        else
                            Debug.Log("Sorry, your friend is imaginary, lol, you loser");
                    }
                    else if (personnageValues[i]["links"][iterator]["linktype"].Value == "Prod" && personnageValues[i]["links"][iterator]["userfrom_id"].Value != "")
                    {
                        Personnage thisPersonnage = GameObject.Find("Personnage" + personnageValues[i]["id"]).GetComponent<Personnage>();
                        GameObject ProductiveLink = GameObject.Find("Personnage" + personnageValues[i]["links"][iterator]["userto_id"].Value);
                        Debug.Log("Trying to add Professional Relationship");
                        if (ProductiveLink)
                        {
                            Personnage myProdLink = ProductiveLink.GetComponent<Personnage>();
                            thisPersonnage.myProductiveLink = myProdLink;
                            myProdLink.charIMakeProductive = thisPersonnage;
                            Debug.Log("Added productive Link");
                        }
                        else
                            Debug.Log("Productive link is not available in this current game");
                    }
                }
            }
        }

        //Permet de mettre à jour la fenêtre de description en fonction du type d'élément selectionné
        public void UpdateDescription()
        {
            if (_selectedGameObject.GetComponent<Personnage>())
            {
                descriptionPanel.GetComponent<Image>().color = new Color(0.3f, 0.4f, 0.6f, 0.9f);
                dscPiece.SetActive(false);
                dscPers.SetActive(true);
                Personnage p = _selectedGameObject.GetComponent<Personnage>();
                descriptionPersonnage[0].text = "{Personnage } : " + p.role;
                descriptionPersonnage[1].text = "Satisfaction : " + p.Satisfaction.satisfactionTotale;
                descriptionPersonnage[2].text = "Nombre de salariés : " + p.nbrSalaries;
                descriptionPersonnage[3].text = "Surface salarié : " + p.surfaceSalarie;
                descriptionPersonnage[4].text = "Luminosité : " + p.luminosite;
                descriptionPersonnage[5].text = "Accès Extérieur : " + p.accesExterieur;
                descriptionPersonnage[6].text = "Distance salle de pause : " + p.distanceSallePause;
                descriptionPersonnage[7].text = "Distance toilette : " + p.distanceToilette;
                if (p.avatar)
                {
                    descriptionPersonnage[8].text = "Sexe : " + p.avatar.sexe;
                    descriptionPersonnage[9].text = "Handicap : " + p.avatar.handicaped;
                }
                else
                {
                    descriptionPersonnage[8].text = "Sexe : N/A";
                    descriptionPersonnage[9].text = "Handicap : N/A";
                }
                if (p.copain != null)
                {
                    string description = "Copain : ";
                    description += " " + p.copain.role;
                    descriptionPersonnage[10].text = description;
                }
                else
                {
                    descriptionPersonnage[10].text = "Copain : N/A";
                }
                if(p.myProductiveLink != null)
                {
                    string description = "Relation Productive : ";
                    description += " " + p.myProductiveLink.role;
                    descriptionPersonnage[11].text = description;
                }
                else
                {
                    descriptionPersonnage[11].text = "Relation Productive : N/A";
                }
                imageAvatarBtn.sprite = _selectedGameObject.GetComponent<SpriteRenderer>().sprite;
            }
            else if (_selectedGameObject.GetComponent<Piece>())
            {
                descriptionPanel.GetComponent<Image>().color = new Color(0.8f, 0.4f, 0.4f, 0.9f);
                dscPiece.SetActive(true);
                dscPers.SetActive(false);
                Piece p = _selectedGameObject.GetComponent<Piece>();
                descriptionPiece[0].text = "{PIECE} - " + p.surface;
                descriptionPiece[2].text = "Ouverture extèrieure : " + p.ouvertureExterieur;
                descriptionPiece[3].text = "Accès Handicapé : " + p.accesHandicape;
                descriptionPiece[4].text = "Accès Extèrieur : " + p.accesExterieur;
                descriptionPiece[5].text = "Distance salle de pause : " + p.distanceSallePause;
                descriptionPiece[6].text = "Distance toilette : " + p.distanceToilette;
            }
        }
		public void Save(string fileName)
		{
			ES2.Save(SceneManager.GetActiveScene().name, fileName + "?tag=managementSceneId");
		}
		public void Load(string fileName)
		{
		}

    }
}