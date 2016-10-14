using UnityEngine;

namespace Assets.Scripts.Management
{
    //Cette classe permet de changer les couleurs des pièces en fonction de leur état (occupé, libre..)
    public class TriggerPiece : MonoBehaviour
    {


        private SpriteRenderer imagePiece;
        private Piece piece;

        void Start()
        {
            imagePiece = GetComponentInChildren<SpriteRenderer>();
            piece = GetComponent<Piece>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if(piece.personnage)
                imagePiece.color = new Color(1f, 0.5f, 0.5f, 0.2f);
            else
                imagePiece.color = new Color(0.5f,1f,0.5f,0.2f);
            other.GetComponent<DragImage>().Room = this.gameObject;
        }

        void OnTriggerExit2D(Collider2D other)
        {
            
            imagePiece.color = new Color(58, 52, 34, 0);
            if(other.GetComponent<DragImage>().Room == this.gameObject)
                other.GetComponent<DragImage>().Room = null;
        }
    }
}
