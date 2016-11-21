using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Management
{

    //Cette classe liée à un personnage est utilisée pour mémoriser chaque critère de satisfaction
    public class SatisfactionPersonnage : MonoBehaviour
    {

        public float surface;
        public float luminosite;
        public float handicap;
        public float accesExterieur;
        public float distanceSallePause;
        public float distanceToilette;
        public float aCoteCopain;

        public float satisfactionTotale;

        public Personnage Personnage { get; private set; }


        void Start()
        {
            Personnage = GetComponent<Personnage>();
        }

        public IEnumerator CalculSatisfaction()
        {
            while (!Personnage.piece)
            {
                yield return new WaitForSeconds(0.1f);
            }


            #region Satisfaction Surface
            surface = Personnage.piece.surface > Personnage.surfaceSalarie ? 1 : (Personnage.piece.surface / Personnage.surfaceSalarie) * (Personnage.piece.surface / Personnage.surfaceSalarie);
            surface *= 100;
            Debug.Log("Surface for " + Personnage.role + " is " + surface + " in room " + Personnage.piece.id);
            #endregion

            #region Satisfaction Acces Exterieur
            if (Personnage.accesExterieur)
            {
                accesExterieur = Personnage.piece.accesExterieur ? 100 : 0;
            }
            Debug.Log("Accès exterieur for " + Personnage.role + " is " + accesExterieur + " in room " + Personnage.piece.id);
            #endregion

            #region Satisfaction Luminosite
            if (Personnage.luminosite == 0)
                luminosite = 1;
            else if (Personnage.luminosite == 1)
            {
                if (Personnage.piece.ouvertureExterieur <= 4)
                    luminosite = 1;
                else
                    luminosite = 0.5f;
            }
            else if (Personnage.piece.ouvertureExterieur <= 2)
            {
                luminosite = 1;
            }
            else if (Personnage.piece.ouvertureExterieur <= 4)
            {
                luminosite = 0.6f;
            }
            else
                luminosite = 0.2f;

            luminosite *= 100;

            Debug.Log("Luminosité for " + Personnage.role + " is " + luminosite + " in room " + Personnage.piece.id);
            #endregion

            if (Personnage.avatar.handicaped)
            {
                handicap = Personnage.piece.accesHandicape ? 100 : 0;
            }
            
            distanceSallePause = (Personnage.distanceSallePause / Personnage.piece.distanceSallePause) * 100;
            if (distanceSallePause > 100)
                distanceSallePause = 100;

            distanceToilette = (Personnage.distanceToilette / Personnage.piece.distanceToilette) * 100;
            if (distanceToilette > 100)
                distanceToilette = 100;

            //Calcule la satisfaction en fonction des personnages à coté de lui
            if (Personnage.copains.Count != 0)
            {
                foreach (Piece piece in Personnage.piece.nextTo)
                {
                    foreach (Personnage copain in Personnage.copains)
                    {
                        if (copain.piece && copain.piece.name == piece.name)
                        {
                            aCoteCopain = 100;
                            break;
                        }
                        else
                        {
                            aCoteCopain = 0;
                        }
                    }
                }
            }
            //Actualise la satisfaction des personnages qui peuvent être bien aimés par ce personnage
            foreach (Personnage personnage in Personnage.bienAimePar)
            {
                personnage.CalculSatisfaction();
            }
            satisfactionTotale = surface + luminosite + distanceSallePause + distanceToilette;
            int nbrParam = 4;
            if (Personnage.avatar.handicaped)
            {
                satisfactionTotale += handicap;
                nbrParam++;
            }
            if (Personnage.accesExterieur)
            {
                satisfactionTotale += accesExterieur;
                nbrParam++;
            }
            if (Personnage.copains.Count != 0)
            {
                satisfactionTotale += aCoteCopain;
                nbrParam++;
            }

            satisfactionTotale = satisfactionTotale/nbrParam;

        }

        public void Reset()
        {
            this.surface = 0;
            this.luminosite = 0;
            this.satisfactionTotale = 0;
            this.aCoteCopain = 0;
            this.accesExterieur = 0;
            this.distanceSallePause = 0;
            this.distanceToilette = 0;
            this.handicap = 0;
        }
    }
}
