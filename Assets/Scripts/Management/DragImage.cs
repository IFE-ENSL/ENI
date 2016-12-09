

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
        private Personnage managementCharacter;
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
            managementCharacter = GetComponent<Personnage>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            globalSatisfaction = GameObject.Find("PourcentageSatisfaction").GetComponent<SatisfactionGlobale>();
        }

        void Update()
        {
            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
            draggingAnyCharacter();
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

                    /*if (managementCharacter.room)
                        managementCharacter.room.managementCharacter = null;*/ //TODO : Useless or what?
                    
                    //Now that we released the character from dragging,
                    //We attach the room to its script, and we attach the character to the room's script.
                    managementCharacter.room = room;
                    room.managementCharacter = managementCharacter;
                    IsDragging = false; //Of course, we're not dragging the picture anymore.

                    //For the db trace, we set the previous and now current room.
                    PreviousRoomId = CurrentRoomId;
                    CurrentRoomId = room.id;
                    //Then send these datas to te SQL DB.
                    LogManagement data = new LogManagement(managementCharacter.persoId, PreviousRoomId, CurrentRoomId);
                    StartCoroutine(gameManager.connexion.PostLog("Déplacement d'un managementCharacter", "Management", data));
                    //Once this is done, we have to update the character's satisfaction.
                    managementCharacter.UpdateSatisfaction();

                    //And then the satisfaction for their relationships
                    if (managementCharacter.likedBy != null)
                    {
                        managementCharacter.likedBy.UpdateSatisfaction();
                    }

                    if (managementCharacter.charIMakeProductive != null)
                    {
                        managementCharacter.charIMakeProductive.UpdateSatisfaction();
                    }

                    //Once every concerned character has their satisfaction updated,
                    //we can safely update the global satsifaction percentage.
                    globalSatisfaction.UpdateGlobalSatisfaction();
                }
                //In case the room is not empty...
                else if (room.managementCharacter && room.managementCharacter.name != this.name)
                {
                    //Let's send back the character to their initial position
                    this.gameObject.transform.position = startPoint;

                    //If this character wasn't previously linked to a room, we're done.
                    if (!managementCharacter.room) return;

                    //Else, let's make sure the character isn't attached to a room anymore.
                    //Basically, we reset everything about the character as they're
                    //returning to their starting state.
                    managementCharacter.room.managementCharacter = null;
                    managementCharacter.room = null;
                    managementCharacter.ResetSatisfaction();
                    globalSatisfaction.UpdateGlobalSatisfaction();
                }
            }
            //Else if we dropped the character on anything but a room...
            else if (!isMoving && !Room)
            {
                //If it was different than its start position, let's send them back to it
                if(this.gameObject.transform.position != startPoint)
                    this.gameObject.transform.position = startPoint;

                //If there was no previous room attached to this character, we're done.
                if (!managementCharacter.room) return;

                //Else, we reset the variables that linked this character to the room.
                managementCharacter.room.managementCharacter = null;
                managementCharacter.room = null;
                managementCharacter.ResetSatisfaction();
                globalSatisfaction.UpdateGlobalSatisfaction();
            }
        }

        //Unity API Method.
        //Called when the user clicked the GO and is still holding down the mouse
        void OnMouseDrag()
        {
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10.0f));
            IsDragging = true;
        }
    }
}