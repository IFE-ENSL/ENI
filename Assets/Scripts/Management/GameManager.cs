using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Connexion;
using Assets.Scripts.Utility;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

namespace Assets.Scripts.Management
{
    //Cette classe permet de gérer les objets séléctionnées dans le mini jeu et les fonctions de base
    //La gamemanager va se charger au lancement du mini jeu de charger toute les pièces depuis la base de données et de charger aléatoirement un certain nombre de personnages
    //Il va aussi servir lors du clic sur un objet du jeu à gérer l'affichage du déscriptif de celui ci

    public class GameManager : MonoBehaviour
    {
        #region General Vars
        //The management minigame id for this scene. Used to load the appropriate rooms stats.
        public int sceneId;
        public bool draggingAnyCharacter;
        #endregion

        #region External Objects, Components & misc. variables
        private GameObject _selectedGameObject;
        public GameObject go_characterGrid;
        public GameObject descriptionPanel;
        #endregion

        #region distance lists
        //The array of distances for each room from the break room and the bathroom, ordered by distance (Farthest first)
        public static List<float> roomDistanceFromBreakRoom = new List<float>();
        public static List<float> roomDistanceFromBathRoom = new List<float>();
        #endregion

        public GameObject SelectedGameObject
        {
            get { return _selectedGameObject; }
            set
            {
                //Effectue des controles lors du changement de l'objet selectionné et met à jour le panneau de description 
                //ainsi que les indications sur scene permettant de voir plus facilement l'objet selectionné
                if (_selectedGameObject && _selectedGameObject.name == value.name)
                    return;

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
        //Tableau contenant la liste des champs utilisés lors de l'affichage du déscriptif d'un managementCharacter
        public Text[] descriptionPersonnage;
        //Tableau contenant la liste des champs utilisés lors de l'affichage du déscriptif d'une pièce
        public Text[] descriptionPiece;
        public Image imageAvatarBtn;
        //Liste des pièces de la scene
        public Room[] pieces;
        //Tableau de positions pour les zones de départ des personnages
        private readonly Vector3[] positionsPersonnage = new[] {new Vector3(0.03999f,2.51f), new Vector3(-2.91f,2.51f),
            new Vector3(2.96f,2.51f),new Vector3(-2.94f,0.09f),new Vector3(0.07f,0.09f),new Vector3(0.07f,-2.646f),
        new Vector3(-2.9f,-2.57f)};
        //Gameobject contenant les éléments permettant la déscription d'un managementCharacter
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

            StartCoroutine(connexion.mConnexion.getPiecesDistance(wait, this.sceneId));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(1);
            }
            Debug.Log("Récupération des distances entre les pièces : " + wait.data);
            JSONNode piecesDistancesValues = null;
            piecesDistancesValues = JSON.Parse(wait.data);

            //Itère a travers les pièces récupérées, et les insère dans l'ordre dans le tableau de pièces présent sur la scène
            for (int i = 0; i < pieces.Length; i++)
            {
                pieces[i].id = piecesValues[i]["id"].AsInt;
                pieces[i].accesExterieur = piecesValues[i]["accesExterieur"].Value == "1" ? true : false;
                pieces[i].accesHandicape = piecesValues[i]["accesHandicape"].Value == "1" ? true : false;

                pieces[i].distanceSallePause = piecesValues[i]["distanceSallePause"].AsFloat;
                roomDistanceFromBreakRoom.Add(pieces[i].distanceSallePause);
                roomDistanceFromBreakRoom.Sort();
                roomDistanceFromBreakRoom.Reverse();

                pieces[i].distanceToilette = piecesValues[i]["distanceToillette"].AsFloat;
                roomDistanceFromBathRoom.Add(pieces[i].distanceToilette);
                roomDistanceFromBathRoom.Sort();
                roomDistanceFromBathRoom.Reverse();

                pieces[i].surface = piecesValues[i]["surface"].AsFloat;
                pieces[i].ouvertureExterieur = piecesValues[i]["ouvertureExterieur"].AsInt;


                //Adding the distances between this room and the others
                Dictionary<int, int> roomDistances = new Dictionary<int, int>();
                for (int iterator = 0; iterator < piecesDistancesValues.Count; iterator ++)
                {
                    if (piecesDistancesValues[iterator]["id_from"].AsInt == pieces[i].id)
                    {
                        roomDistances.Add(piecesDistancesValues[iterator]["id_to"].AsInt, piecesDistancesValues[iterator]["distance"].AsInt);
                    }
                    else if (piecesDistancesValues[iterator]["id_to"].AsInt == pieces[i].id)
                    {
                        roomDistances.Add(piecesDistancesValues[iterator]["id_from"].AsInt, piecesDistancesValues[iterator]["distance"].AsInt);
                    }

                }

                var items = from pair in roomDistances
                            orderby pair.Value descending
                            select pair.Key;

                pieces[i].roomDistancesid = items.ToList<int>();
                Debug.Log("Is Okay I think...");

            }
        }

        //Récupère une liste aléatoire de personnages depuis la base de donnée
        private IEnumerator getPersonnages()
        {
            #region Getting Character from SQL
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
            StartCoroutine(connexion.mConnexion.getPersonnages(wait, sessionMiniJeu));
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
            #endregion

            List<GameObject> allCharacters = new List<GameObject>();

            for (int i = 0; i < personnageValues.Count; i++)
            {
                //Crée un managementCharacter, et associe les valeurs récupérées dans la base à ce managementCharacter pour chaque itération
                if (!ids.Contains(personnageValues[i]["id"].AsInt))
                {
                    //Création du managementCharacter
                    GameObject managementCharacter = Instantiate(personnagePrefab);
                    allCharacters.Add(managementCharacter);
                    managementCharacter.transform.parent = go_characterGrid.transform;
                    managementCharacter.name = "Personnage" + personnageValues[i]["id"].Value;
                    managementCharacter.transform.localPosition = positionsPersonnage[idPlacement];
                    idPlacement++;

                    //Association des valeurs récupérées
                    Personnage scriptP = managementCharacter.GetComponent<Personnage>();
                    scriptP.role = personnageValues[i]["role"].Value;
                    scriptP.surfaceSalarie = personnageValues[i]["surfaceSalarie"].AsInt;
                    scriptP.luminosite = personnageValues[i]["luminosite"].AsInt;
                    scriptP.accesExterieur = (personnageValues[i]["accesExterieur"].Value == "1") ? true : false;
                    scriptP.distanceSallePause = personnageValues[i]["distanceSallePause"].AsInt;
                    scriptP.distanceToilette = personnageValues[i]["distanceToilette"].AsFloat;
                    scriptP.persoId = personnageValues[i]["id"].AsInt;
                    scriptP.serviceId = personnageValues[i]["service_id"].AsInt;

                    //Envoie du log indiquant l'insertion du managementCharacter
                    StartCoroutine(connexion.mConnexion.insertSessionPersonnage(personnageValues[i]["id"].Value,
                        sessionMiniJeu));

                    //Ajoute l'identifiant du managementCharacter à une liste afin de ne pas en crééer deux ayant le même identifiant
                    //(Si la requête SQL retourne deux personnages avec le même identifiant c'est parceque ils ne sont pas associés au même copain)
                    ids.Add(scriptP.persoId);


                    //Setting up the character's avatar picture based on its character ID.
                    scriptP.setAvatar(scriptP.persoId);

                }
            }

            //Now associating all the relationships of this character...
            //For each character in this game session...
            for (int characterIterator = 0; characterIterator < personnageValues.Count; characterIterator++)
            {
                int FriendIndexToUse = 0;
                int ServiceIndexToUse = 0;

                bool friendExist = false;
                bool serviceLinkExist = false;

                int lowestPlinksCoeff = 10;
                int lowestSlinksCoeff = 10;

                //For each character link for this character...
                for (int iterator = 0; iterator < personnageValues[characterIterator]["plinks"].Count; iterator++)
                {
                    //Finding which friend link to use (Lower coeff first)
                    if (personnageValues[characterIterator]["plinks"][iterator]["userfrom_id"].Value != "")
                    {
                        friendExist = true;

                        if (personnageValues[characterIterator]["plinks"][iterator]["coeff"].AsInt < lowestPlinksCoeff)
                        {
                            lowestPlinksCoeff = personnageValues[characterIterator]["plinks"][iterator]["coeff"].AsInt;
                            FriendIndexToUse = iterator;
                        }
                    }
                }
                
                //For each productive link for this character...
                for (int iterator = 0; iterator < personnageValues[characterIterator]["slinks"].Count; iterator ++)
                { 
                    //Finding which productive (Service Links) link to use (Lower coeff first)
                    if (personnageValues[characterIterator]["slinks"][iterator]["servicefrom_id"].Value != "")
                    {
                        serviceLinkExist = true;

                        if (personnageValues[characterIterator]["slinks"][iterator]["coeff"].AsInt < lowestSlinksCoeff)
                        {
                            lowestSlinksCoeff = personnageValues[characterIterator]["slinks"][iterator]["coeff"].AsInt;
                            ServiceIndexToUse = iterator;
                        }
                    }
                }

                //Associating friendly links if the character exists in the scene
                if (friendExist)
                {
                    Personnage thisPersonnage = GameObject.Find("Personnage" + personnageValues[characterIterator]["id"]).GetComponent<Personnage>();
                    GameObject copain = GameObject.Find("Personnage" + personnageValues[characterIterator]["plinks"][FriendIndexToUse]["userto_id"].Value);
                    if (copain != null)
                    {
                        Personnage friend = copain.GetComponent<Personnage>();
                        thisPersonnage.copain = friend;
                        friend.likedBy = thisPersonnage;
                    }
                }

                //Associating productive links (Service links) if the character exists in the scene
                if (serviceLinkExist)
                {
                    Personnage thisPersonnage = GameObject.Find("Personnage" + personnageValues[characterIterator]["id"]).GetComponent<Personnage>();
                    GameObject ProductiveLink = null;

                    foreach (GameObject character in allCharacters)
                    {
                        if (character.GetComponent<Personnage>().serviceId == personnageValues[characterIterator]["slinks"][ServiceIndexToUse]["serviceto_id"].AsInt)
                        {
                            ProductiveLink = character;
                            break;
                        }
                    }

                    if (ProductiveLink != null)
                    {
                        Personnage myProdLink = ProductiveLink.GetComponent<Personnage>();
                        thisPersonnage.myProductiveLink = myProdLink;
                        myProdLink.charIMakeProductive = thisPersonnage;
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
                descriptionPersonnage[2].text = "Surface salarié : " + p.surfaceSalarie;
                descriptionPersonnage[3].text = "Luminosité : " + p.luminosite;
                descriptionPersonnage[4].text = "Accès Extérieur : " + p.accesExterieur;
                descriptionPersonnage[5].text = "Distance salle de pause : " + p.distanceSallePause;
                descriptionPersonnage[6].text = "Distance toilette : " + p.distanceToilette;
                if (p.copain != null)
                {
                    string description = "Copain : ";
                    description += " " + p.copain.role;
                    descriptionPersonnage[7].text = description;
                }
                else
                {
                    descriptionPersonnage[7].text = "Copain : N/A";
                }
                if(p.myProductiveLink != null)
                {
                    string description = "Relation Productive : ";
                    description += " " + p.myProductiveLink.role;
                    descriptionPersonnage[8].text = description;
                }
                else
                {
                    descriptionPersonnage[8].text = "Relation Productive : N/A";
                }
                imageAvatarBtn.sprite = _selectedGameObject.GetComponent<SpriteRenderer>().sprite;
            }
            else if (_selectedGameObject.GetComponent<Room>())
            {
                descriptionPanel.GetComponent<Image>().color = new Color(0.8f, 0.4f, 0.4f, 0.9f);
                dscPiece.SetActive(true);
                dscPers.SetActive(false);
                Room p = _selectedGameObject.GetComponent<Room>();
                descriptionPiece[0].text = "{PIECE} - " + p.surface;
                descriptionPiece[2].text = "Ouverture extèrieure : " + p.ouvertureExterieur;
                descriptionPiece[3].text = "Accès Extèrieur : " + p.accesExterieur;
                descriptionPiece[4].text = "Distance salle de pause : " + p.distanceSallePause;
                descriptionPiece[5].text = "Distance toilette : " + p.distanceToilette;
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