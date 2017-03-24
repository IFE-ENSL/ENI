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
        bool globalWait = false;
        public Camera mainCamera;
        public Camera LoadingCamera;
        public Canvas mainCanvas;
        public int sceneId; //The management minigame id for this scene. Used to load the appropriate rooms stats.
        private int miniGameSession;

        private readonly Vector3[] charactersGridPositions = new[] {new Vector3(0.03999f,1.25f), new Vector3(-2.91f,1.25f),
        new Vector3(2.96f,1.25f),new Vector3(-2.94f,-1.17f),new Vector3(0.07f,-1.17f),new Vector3(0.07f,-2.646f),
        new Vector3(-2.9f,-2.57f)};
        #endregion

        #region Game Status Vars
        public bool draggingAnyCharacter;
        #endregion

        #region External Objects, Components & misc. variables
        public GameObject personnagePrefab;
        private GameObject _selectedGameObject;
        public GameObject highlightedRoom;
        public GameObject go_characterGrid;
        public GameObject descriptionPanel;
        public GameObject roomDescriptionPanel;
        public ConnexionController connexionController;
        public GameObject go_characterDescription;
        public GameObject go_roomDescription;
        public Text[] textCharStats;
        public Text[] textRoomStats;
        public Image imageAvatarBtn;
        public Room[] rooms;

        [SerializeField]
        GameObject[] backgroundPrefabs;

        [HideInInspector]
        public List<ManagementCharacter> managementCharacters;
        #endregion

        #region distance lists
        //The array of distances for each room from the break room and the bathroom, ordered by distance (Farthest first)
        public static List<float> roomDistanceFromBreakRoom = new List<float>();
        public static List<float> roomDistanceFromBathRoom = new List<float>();
        #endregion

        //This variable is a bit special, when a gameObject gets selected, it will call some other methods, see below...
        public GameObject SelectedGameObject
        {
            get { return _selectedGameObject; }
            set
            {
                //If it's the same object than before, we're done...
                if (_selectedGameObject && _selectedGameObject.name == value.name)
                    return;

                //If we selected this object and the description panel is hidden, let's enable it.
                if (!descriptionPanel.activeInHierarchy)
                    descriptionPanel.SetActive(true);

                //If an object was previously activated, we now deactivate it
                if (_selectedGameObject)
                    _selectedGameObject.transform.GetChild(0).gameObject.SetActive(false);

                _selectedGameObject = value;
                value.transform.GetChild(0).gameObject.SetActive(true);
                UpdateDescription();
            }
        }

        void Start()
        {
            sceneId = GameObject.Find("PersistentSceneDatas").GetComponent<PersistentFromSceneToScene>().alternativeSceneId;
            SpawnBackgroundAndRooms();

            connexionController = GameObject.Find("ConnexionController").GetComponent<ConnexionController>();
            StartCoroutine(this.getRooms());
            StartCoroutine(this.getCharacters());
            StartCoroutine(connexionController.PostLog("Début du jeu", "Management", new LogManagement()));
            descriptionPanel.SetActive(false);
        }

        void Update ()
        {
            if (globalWait) //If we're waiting for datas to be retrieved from the server, display the loading screen
            {
                mainCamera.depth = -1;
                LoadingCamera.depth = 0;
                mainCanvas.enabled = false;
            }
            else
            {
                mainCamera.depth = 0;
                LoadingCamera.depth = -1;
                mainCanvas.enabled = true;
            }
        }

        void SpawnBackgroundAndRooms ()
        {
            GameObject instantiatedBackgroundAndRooms;
            instantiatedBackgroundAndRooms = GameObject.Instantiate(backgroundPrefabs[sceneId - 1], Vector3.zero, Quaternion.identity) as GameObject;
            rooms = instantiatedBackgroundAndRooms.GetComponentsInChildren<Room>().OrderBy( go => go.transform.name ).ToArray();
        }

        //Récupère la liste des pièces depuis la base de donnée
        private IEnumerator getRooms()
        {
            Waiter wait = new Waiter();

            StartCoroutine(connexionController.mConnexion.getRooms(wait,this.sceneId));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(1);
            }
            Debug.Log("Récupération des pièces : " + wait.data);
            JSONNode piecesValues = null;
            piecesValues = JSON.Parse(wait.data);
            if (rooms == null) yield break;

            StartCoroutine(connexionController.mConnexion.getPiecesDistance(wait, this.sceneId));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(1);
            }
            Debug.Log("Récupération des distances entre les pièces : " + wait.data);
            JSONNode piecesDistancesValues = null;
            piecesDistancesValues = JSON.Parse(wait.data);

            if (rooms.Length <= 0)
                Debug.LogError("No room into the Game Manager inspector! Are you sure you referenced them?");


            //For each room we got from the SQL db, we put the stats into each of their Go (Don't forget to put them in the inspector)
            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i].id = piecesValues[i]["id"].AsInt;
                rooms[i].accesExterieur = piecesValues[i]["accesExterieur"].Value == "1" ? true : false;
                rooms[i].accesHandicape = piecesValues[i]["accesHandicape"].Value == "1" ? true : false;

                rooms[i].distanceSallePause = piecesValues[i]["distanceSallePause"].AsFloat;
                roomDistanceFromBreakRoom.Add(rooms[i].distanceSallePause);
                roomDistanceFromBreakRoom.Sort();
                roomDistanceFromBreakRoom.Reverse();

                rooms[i].distanceToilette = piecesValues[i]["distanceToillette"].AsFloat;
                roomDistanceFromBathRoom.Add(rooms[i].distanceToilette);
                roomDistanceFromBathRoom.Sort();
                roomDistanceFromBathRoom.Reverse();

                rooms[i].surface = piecesValues[i]["surface"].AsFloat;
                rooms[i].ouvertureExterieur = piecesValues[i]["ouvertureExterieur"].AsInt;


                //Adding the distances between this room and the others
                Dictionary<int, int> roomDistances = new Dictionary<int, int>();
                for (int iterator = 0; iterator < piecesDistancesValues.Count; iterator ++)
                {
                    if (piecesDistancesValues[iterator]["id_from"].AsInt == rooms[i].id)
                    {
                        roomDistances.Add(piecesDistancesValues[iterator]["id_to"].AsInt, piecesDistancesValues[iterator]["distance"].AsInt);
                    }
                    else if (piecesDistancesValues[iterator]["id_to"].AsInt == rooms[i].id)
                    {
                        roomDistances.Add(piecesDistancesValues[iterator]["id_from"].AsInt, piecesDistancesValues[iterator]["distance"].AsInt);
                    }
                }

                //Ordering the roomDistances dictionary, the farthest first, and the nearest in last.
                var items = from pair in roomDistances
                            orderby pair.Value descending
                            select pair.Key;

                rooms[i].roomDistancesid = items.ToList<int>();
            }
        }

        //Récupère une liste aléatoire de personnages depuis la base de donnée
        private IEnumerator getCharacters()
        {
            #region Getting Character from SQL
            Waiter wait = new Waiter();
            globalWait = true;
            //Creating a new session for this minigame
            StartCoroutine(connexionController.mConnexion.insertSessionMiniJeu(wait));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(0.5f);
            }
            try
            {
                miniGameSession = Convert.ToInt32(wait.data);
                PlayerPrefs.SetInt("sessionMiniJeuManagement",miniGameSession);
            }
            catch (Exception)
            {
                Debug.Log("Erreur lors de l'insertion d'une nouvelle session");
                print("Data : " + wait.data);
                SceneManager.LoadScene(0);
            }
            wait.Reset();
            StartCoroutine(connexionController.mConnexion.getCharacters(wait, miniGameSession));
            while (wait.waiting)
            {
                yield return new WaitForSeconds(0.5f);
            }
            Debug.Log("Récupération des personnages : " + wait.data);
            JSONNode personnageValues = null;
            personnageValues = JSON.Parse(wait.data);
            if (rooms == null) yield break;
            int idPlacement = 0;
            #endregion

            List<GameObject> allCharacters = new List<GameObject>();

            //For each character we got from the SQL DB we instantiate a new character as a GO and sets all the attriute we got.
            for (int i = 0; i < personnageValues.Count; i++)
            {
                    //Instantiating character...
                    GameObject managementCharacter = Instantiate(personnagePrefab);
                    allCharacters.Add(managementCharacter);
                    managementCharacter.transform.parent = go_characterGrid.transform;
                    managementCharacter.name = "Personnage" + personnageValues[i]["id"].Value;
                    managementCharacter.transform.localPosition = charactersGridPositions[idPlacement];
                    idPlacement++;

                    //Association of values we got from SQL DB
                    ManagementCharacter scriptP = managementCharacter.GetComponent<ManagementCharacter>();

                    managementCharacters.Add(scriptP);

                    scriptP.role = personnageValues[i]["role"].Value;
                    scriptP.surfaceSalarie = personnageValues[i]["surfaceSalarie"].AsInt;
                    scriptP.luminosite = personnageValues[i]["luminosite"].AsInt;
                    scriptP.accesExterieur = (personnageValues[i]["accesExterieur"].Value == "1") ? true : false;
                    scriptP.distanceSallePause = personnageValues[i]["distanceSallePause"].AsInt;
                    scriptP.distanceToilette = personnageValues[i]["distanceToilette"].AsFloat;
                    scriptP.persoId = personnageValues[i]["id"].AsInt;
                    scriptP.serviceId = personnageValues[i]["service_id"].AsInt;

                    //Sending log of the character we just instantiated
                    StartCoroutine(connexionController.mConnexion.insertSessionPersonnage(personnageValues[i]["id"].Value,
                        miniGameSession));

                    //Setting up the character's avatar picture based on its character ID.
                    scriptP.setAvatar(scriptP.persoId);
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
                    ManagementCharacter thisCharacter = GameObject.Find("Personnage" + personnageValues[characterIterator]["id"]).GetComponent<ManagementCharacter>();
                    GameObject friend = GameObject.Find("Personnage" + personnageValues[characterIterator]["plinks"][FriendIndexToUse]["userto_id"].Value);
                    if (friend != null)
                    {
                        ManagementCharacter characterScript_Friend = friend.GetComponent<ManagementCharacter>();
                        thisCharacter.friend = characterScript_Friend;
                        characterScript_Friend.likedBy = thisCharacter;
                    }
                }

                //Associating productive links (Service links) if the character exists in the scene
                if (serviceLinkExist)
                {
                    ManagementCharacter thisPersonnage = GameObject.Find("Personnage" + personnageValues[characterIterator]["id"]).GetComponent<ManagementCharacter>();
                    GameObject ProductiveLink = null;

                    foreach (GameObject character in allCharacters)
                    {
                        if (character.GetComponent<ManagementCharacter>().serviceId == personnageValues[characterIterator]["slinks"][ServiceIndexToUse]["serviceto_id"].AsInt)
                        {
                            ProductiveLink = character;
                            break;
                        }
                    }

                    if (ProductiveLink != null)
                    {
                        ManagementCharacter myProdLink = ProductiveLink.GetComponent<ManagementCharacter>();
                        thisPersonnage.myProductiveLink = myProdLink;
                        myProdLink.charIMakeProductive = thisPersonnage;
                    }
                }

            }
            globalWait = false;
        }

        //Generating the text displayed for each character according to their needs
        void GenerateDialogueDescription (ManagementCharacter character, ref string dialogueDescription)
        {
            if (character.Satisfaction.surface < 80)
            {
                dialogueDescription = "Pour pouvoir bien travailler, j'ai besoin d'une surface de travail <b><i>aux alentours de " + character.surfaceSalarie;
                dialogueDescription += "m².</i></b> ";

                if (character.Satisfaction.luminosite <= 60f)
                {
                    if (character.luminosite == 1)
                        dialogueDescription += "J'ai également besoin d'une <b><i>pièce un minimum lumineuse.</i></b> ";
                    else if (character.luminosite == 2)
                        dialogueDescription += "J'ai également besoin d'une <b><i>pièce la plus lumineuse possible.</i></b> ";
                }
            }
            else
            {
                dialogueDescription = "Pour pouvoir bien travailler, j'ai besoin de plusieurs choses. ";


                if (character.Satisfaction.luminosite <= 60f)
                {
                    if (character.luminosite == 1)
                        dialogueDescription += "Nottament, j'ai besoin d'une <b><i>pièce pièce pas trop sombre.</i></b> ";
                    else if (character.luminosite == 2)
                        dialogueDescription += "Nottament, j'ai besoin d'une <b><i>pièce la plus lumineuse possible.</i></b> ";
                }
            }

            if (character.accesExterieur && character.Satisfaction.accesExterieur < 100)
                dialogueDescription += "Il est indispensable que j'aie <b><i>un accès direct à l'extérieur.</i></b> ";

            if (character.myProductiveLink )
                dialogueDescription += "J'ai besoin d'être <b><i>proche de mon collègue du service " + character.myProductiveLink.role.ToLower() + "</i></b> pour être plus efficace. ";

            bool displayBreakRoomNeed = false;
            bool displayBathroomNeed = false;

            if (character.distanceSallePause > 0 && character.distanceSallePause <= 15 && character.Satisfaction.distanceSallePause <= 70)
                displayBreakRoomNeed = true;
            if (character.distanceToilette > 0 && character.distanceToilette <= 15 && character.Satisfaction.distanceToilette <= 70)
                displayBathroomNeed = true;

            if (displayBathroomNeed || displayBreakRoomNeed)
            {
                dialogueDescription += "Ce serait appréciable si ";

                if (displayBreakRoomNeed)
                {
                    if (character.distanceSallePause <= 5)
                        dialogueDescription += "je pouvais être le <b><i>plus proche possible de la salle de pause</i></b>";
                    else if (character.distanceSallePause <= 10)
                        dialogueDescription += "je pouvais être <b><i>proche de la salle de pause</i></b>";
                    else if (character.distanceSallePause <= 15)
                        dialogueDescription += "je pouvais être <b><i>assez proche de la salle de pause</i></b>";
                }

                if (displayBathroomNeed && displayBreakRoomNeed)
                {
                    dialogueDescription += " et que ";
                }
                else if (displayBreakRoomNeed)
                {
                    dialogueDescription += ". ";
                }

                if (displayBathroomNeed)
                {
                    if (character.distanceToilette <= 5)
                        dialogueDescription += "je pouvais être le <b><i>plus proche possible des toilettes.</i></b> ";
                    else if (character.distanceToilette <= 10)
                        dialogueDescription += "je pouvais être <b><i>proche de la salle des toilettes.</i></b> ";
                    else if (character.distanceToilette <= 15)
                        dialogueDescription += "je pouvais être <b><i>assez proche des toilettes.</i></b> ";
                }
            }
            if (character.friend)
                dialogueDescription += "Pour finir, je m'entends bien avec mon <b><i>collègue du service " + character.friend.role.ToLower() + "</i></b>, pourrais-je être placé pas loin de son bureau ? Merci !";
            
        }

        public void UpdateRoomDescription()
        {
            //descriptionPanel.GetComponent<Image>().color = new Color(0.8f, 0.4f, 0.4f, 0.9f);
            go_roomDescription.SetActive(true);
            //go_characterDescription.SetActive(false);
            Room p = highlightedRoom.GetComponent<Room>();
            textRoomStats[0].text = "Taille de la pièce : " + p.surface + "m²";

            if (p.ouvertureExterieur < 2f)
                textRoomStats[2].text = "Très lumineuse. ";
            else if (p.ouvertureExterieur < 4f)
                textRoomStats[2].text = "Lumineuse. ";
            else if (p.ouvertureExterieur <= 5f)
                textRoomStats[2].text = "Sombre. ";

            if (p.accesExterieur)
                textRoomStats[3].text = "Accès Extèrieur.";
            else
                textRoomStats[3].text = "Pas d'Accès Extèrieur.";

            textRoomStats[4].text = "Distance salle de pause : " + p.distanceSallePause + "m";
                textRoomStats[5].text = "Distance toilette : " + p.distanceToilette + "m";
        }

        //Updating the description windows for the characters
        public void UpdateDescription()
        {
            if (_selectedGameObject.GetComponent<ManagementCharacter>())
            {
                descriptionPanel.SetActive(true);
                descriptionPanel.GetComponent<Image>().color = new Color32(120, 194, 249, 255);
                go_roomDescription.SetActive(false);
                go_characterDescription.SetActive(true);
                ManagementCharacter p = _selectedGameObject.GetComponent<ManagementCharacter>();

                string dialogueDescription = "";
                GenerateDialogueDescription(p, ref dialogueDescription);

                textCharStats[0].text = "Service : " + p.role;
                textCharStats[1].text = dialogueDescription; //Description part
                textCharStats[2].text = "Surface : " + p.surfaceSalarie + "m²";
                textCharStats[3].text = "Luminosité : " + p.luminosite;
                textCharStats[4].text = "Accès Extérieur : " + p.accesExterieur;
                textCharStats[5].text = "Distance salle de pause : " + p.distanceSallePause + "m";
                textCharStats[6].text = "Distance toilette : " + p.distanceToilette + "m";
                if (p.friend != null)
                {
                    string description = "Copain : ";
                    description += " " + p.friend.role;
                    textCharStats[7].text = description;
                }
                else
                {
                    textCharStats[7].text = "Copain : N/A";
                }
                if(p.myProductiveLink != null)
                {
                    string description = "Relation Productive : ";
                    description += " " + p.myProductiveLink.role;
                    textCharStats[8].text = description;
                }
                else
                {
                    textCharStats[8].text = "Relation Productive : N/A";
                }
                imageAvatarBtn.sprite = _selectedGameObject.GetComponent<SpriteRenderer>().sprite;
            }
           
        }
    }
}