

using System.Runtime.CompilerServices;
using UnityEngine;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Management
{
    //Cette classe est utilisée afin de bouger une image sur la scène (Ici un personnage)
    //et de détécter dans quel endroit cette image est située afin de pouvoir intéragir en fonction avec le jeu
    public class DragImage : MonoBehaviour
    {
        private float x;
        private float y;
        private float lastX;
        private float lastY;
        private bool isMoving = false;

        public GameManager gameManager;
        public SatisfactionGlobale sGlobale;

        private Vector3 startPoint;
        private Personnage personnage;
        public GameObject Room;
        int CurrentRoomId = 0;
        int PreviousRoomId = 0;

        public bool IsMoving
        {
            get { return isMoving; }
            set
            {
                gameManager.isDragging = value;
                isMoving = value;
            }
        }


        void Start()
        {
            startPoint = this.transform.position;
            //InvokeRepeating("isDragging",0,0.08f);
            personnage = GetComponent<Personnage>();
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
            sGlobale = GameObject.Find("PourcentageSatisfaction").GetComponent<SatisfactionGlobale>();
        }
        void Update()
        {
            x = Input.mousePosition.x;
            y = Input.mousePosition.y;
            isDragging();
        }
        //Méthode principale du script : Permet de savoir si un personnage est en mouvement ou non en comparant sa position actuelle et sa dernière position
        //Une fois que le personnage n'est plus en mouvement le script vérifie si la souris est toujours en clic dessus. Si non, en fonction de l'endroit ou est situé l'avatar
        //le script va intéragir avec le jeu afin de l'assigner à une pièce ou de le renvoyer à la case départ
        void isDragging()
        {
            if (!IsMoving)
                return;
            if (lastX == x && lastY == y)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

                if (hit.collider != null)
                    if (hit.collider.gameObject.GetComponent<Personnage>() && Input.GetMouseButton(0))
                        return;
                if (IsMoving)
                    IsMoving = false;
            }
            else
            {
                lastX = x;
                lastY = y;
            }
            //Le personnage ne bouge plus et est dans une pièce
            if (!isMoving && Room)
            {
                Piece piece = Room.GetComponent<Piece>();
                //Si la est vide
                if (!piece.personnage)
                {
                    PreviousRoomId = CurrentRoomId;
                    this.transform.position = new Vector2(Room.transform.position.x,Room.transform.position.y);
                    if (personnage.piece)
                        personnage.piece.personnage = null;
                    personnage.piece = piece;
                    piece.personnage = personnage;
                    IsMoving = false;

                    CurrentRoomId = piece.id;

                    //TO DO : This is where to put the log of where the character has been moved (Replaced p by personnage, check if bugs)
                    LogManagement data = new LogManagement(personnage.id, PreviousRoomId, CurrentRoomId);
                    StartCoroutine(gameManager.connexion.PostLog("Déplacement d'un personnage", "Management", data));

                    
                    personnage.CalculSatisfaction();

                    if (personnage.bienAimePar != null)
                    {
                        personnage.bienAimePar.CalculSatisfaction();
                    }

                    if (personnage.charIMakeProductive != null)
                    {
                        personnage.charIMakeProductive.CalculSatisfaction();
                    }

                    sGlobale.CalculSatisfactionGlobale();
                }
                //Si la pièce n'est pas vide
                else if (piece.personnage && piece.personnage.name != this.name)
                {
                    this.gameObject.transform.position = startPoint;
                    if (!personnage.piece) return;
                    personnage.piece.personnage = null;
                    personnage.piece = null;
                    personnage.ResetSatisfaction();
                    sGlobale.CalculSatisfactionGlobale();
                }
            }
            //Le personnage ne bouge plus et n'est pas dans une pièce
            else if (!isMoving && !Room)
            {
                if(this.gameObject.transform.position != startPoint)
                    this.gameObject.transform.position = startPoint;
                if (!personnage.piece) return;
                personnage.piece.personnage = null;
                personnage.piece = null;
                personnage.ResetSatisfaction();
                sGlobale.CalculSatisfactionGlobale();
            }
        }

        void OnMouseDrag()
        {
            if (!personnage.avatar) return;
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(x, y, 10.0f));
            IsMoving = true;
        }
    }
}