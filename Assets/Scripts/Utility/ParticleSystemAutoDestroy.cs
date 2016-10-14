using UnityEngine;

namespace Assets.Scripts.Utility
{
    //Cette classe permet de détruire un objet de particule une fois qu'il a été instancié
    public class ParticleSystemAutoDestroy : MonoBehaviour
    {
        private ParticleSystem ps;


        public void Start()
        {
            ps = GetComponent<ParticleSystem>();
        }

        public void Update()
        {
            if (ps)
            {
                if (!ps.IsAlive())
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}