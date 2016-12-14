

using System.Runtime.CompilerServices;
using UnityEngine;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Management
{
    //Pretty self explicit, this class manages all the image dragging in the management minigame
    //This also tracks any room change for the dragged character for the log trace
    public class DragImage : MonoBehaviour
    {
        #region Moves Var
        private float x;
        private float y;
        private bool isMoving = false;
        #endregion

        #region External Objects
        public GameManager gameManager;
        private Personnage thisManagementCharacter;
        #endregion

        #region Tracking room change variables
        private Vector3 startPoint;
        public GameObject Room;
        int CurrentRoomId = 0;
        int PreviousRoomId = 0;
        #endregion

        public SatisfactionGlobale globalSatisfaction; //TODO : What the hell is this variable doing here? Move it to a more convenient class.

        public bool IsDragging
        {
            get { return isMoving; }
            set
            {
                gameManager.draggingAnyCharacter = value;
                isMoving = value;
            }
        }

        void Start()
        {
            startPoint = this.transform.position;
            thisManagementCharacter = GetComponent<Personnage>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            globalSatisfaction = GameObject.Find("PourcentageSatisfaction").GetComponent<SatisfactionGlobale>();
        }

        void Update()
        {
            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
            draggingAnyCharacter();
        }

        //Unity API Method.
        //Called when the user clicked the GO and is still holding down the mouse
        void OnMouseDrag()
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10.0f));
            IsDragging = true;
        }

        //Main method, check if we're still dragging
        //If not, place the character in a room or send it back to its initial position, according to the position
        //of the cursor at the moment the player release the button
        void draggingAnyCharacter()
        {
            if (!IsDragging)
                return;
            else if (Input.GetMouseButtonUp(0))
                IsDragging = false;

            //Character is released from dragging and is touching a room
            if (!IsDragging && Room)
            {
                Room room = Room.GetComponent<Room>();

                //Check if room is empty
                if (!room.managementCharacter)
                {
                    //Place the character on the exact position of the room
                    this.transform.position = new Vector2(Room.transform.position.x,Room.transform.position.y);

                    //If the character was previously attached to a room, let's clear its link
                    if (thisManagementCharacter.room)
                        thisManagementCharacter.room.managementCharacter = null;
                    
                    //Now that we released the character from dragging,
                    //We attach the room to its script, and we attach the character to the room's script.
                    thisManagementCharacter.room = room;
                    room.managementCharacter = thisManagementCharacter;
                    IsDragging = false; //Of course, we're not dragging the picture anymore.

                    //For the db trace, we set the previous and now current room.
                    PreviousRoomId = CurrentRoomId;
                    CurrentRoomId = room.id;

                    //Then send these datas to te SQL DB.
                    LogManagement data = new LogManagement(thisManagementCharacter.persoId, PreviousRoomId, CurrentRoomId);
                    StartCoroutine(gameManager.connexionController.PostLog("Déplacement d'un thisManagementCharacter", "Management", data));

                    //Once this is done, we have to update the character's satisfaction.
                    //Let's update every satisfaction level
                    UpdateAllSatisfactionLevels();
                }
                //In case the room is not empty...
                else if (room.managementCharacter && room.managementCharacter.name != this.name)
                {
                    //Let's send back the character to their initial position
                    this.gameObject.transform.position = startPoint;

                    //If this character wasn't previously linked to a room, we're done.
                    if (!thisManagementCharacter.room) return;

                    //Else, let's make sure the character isn't attached to a room anymore.
                    //Basically, we reset everything about the character as they're
                    //returning to their starting state.
                    ResetCharacterStats();
                }
            }
            //Else if we dropped the character on anything but a room...
            else if (!isMoving && !Room)
            {
                //If it was different than its start position, let's send them back to it
                if(this.gameObject.transform.position != startPoint)
                    this.gameObject.transform.position = startPoint;

                //If there was no previous room attached to this character, we're done.
                if (!thisManagementCharacter.room) return;

                //Else, we reset the variables that linked this character to the room.
                ResetCharacterStats();
            }
        }

        void ResetCharacterStats ()
        {
            thisManagementCharacter.room.managementCharacter = null;
            thisManagementCharacter.room = null;
            thisManagementCharacter.ResetSatisfaction();

            UpdateAllSatisfactionLevels();
        }

        //Update the satisfaction of every character currently in a room, then the global satisfaction percentage
        void UpdateAllSatisfactionLevels ()
        {
            foreach (Personnage character in gameManager.managementCharacters)
            {
                if (character.room != null)
                    character.UpdateSatisfaction();
            }

            globalSatisfaction.UpdateGlobalSatisfaction();
        }
    }
}