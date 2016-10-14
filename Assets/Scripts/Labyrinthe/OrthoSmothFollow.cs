using UnityEngine;

namespace Assets.Scripts.Labyrinthe
{
    // Classe non utilisée : Permet de faire en sorte que la caméra suive le joueur
    public class OrthoSmothFollow : MonoBehaviour
    {

        public Transform target;
        public float smoothTime = 0.3f;

        private Vector2 velocity = Vector2.zero;

        void Update()
        {
            Vector2 goalPos = target.position;
            //goalPos.y = transform.position.y;
            transform.position = Vector2.SmoothDamp(transform.position, goalPos, ref velocity, smoothTime);
        }
    }
}