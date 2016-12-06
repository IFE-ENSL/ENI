using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Management
{

    //Cette classe liée à un personnage est utilisée pour mémoriser chaque critère de satisfaction
    public class SatisfactionPersonnage : MonoBehaviour
    {

        public float surface;
        public float luminosite;
        public float accesExterieur;
        public float distanceSallePause;
        public float distanceToilette;
        public float aCoteCopain;
        public float productiveLinkSatisfaction;

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

            #region Satsifaction Distance Salle Pause
            int breakRoomDistanceRank = GameManager.roomDistanceFromRestRoom.FindIndex(x => x == Personnage.piece.distanceSallePause) + 1;

            if (Personnage.distanceSallePause == 0)
                distanceSallePause = 1;
            else
            {
                float formula1 = (breakRoomDistanceRank - 1);
                float formula2 = (GameManager.roomDistanceFromRestRoom.Count - 1);
                float baseFormula = (float)(1 - formula1 / formula2);
                float floatFactor = (float)Personnage.distanceSallePause;

                if (Personnage.distanceSallePause == 1)
                    distanceSallePause = baseFormula;
                else
                    distanceSallePause = Mathf.Pow(baseFormula, floatFactor);
            }

            distanceSallePause *= 100;
            Debug.Log("Distance Salle Pause for " + Personnage.role + " (Salle Pause Indice Satis. = " + Personnage.distanceSallePause + ") is " + distanceSallePause + " in room " + Personnage.piece.id + ", proximity rank is " + breakRoomDistanceRank);
            #endregion

            #region Satisfaction Distance Toilette
            int bathRoomDistanceRank = GameManager.roomDistanceFromBathRoom.FindIndex(x => x == Personnage.piece.distanceToilette) + 1;

            if (Personnage.distanceToilette == 0)
                distanceToilette = 1;
            else
            {
                float formula1 = (bathRoomDistanceRank - 1);
                float formula2 = (GameManager.roomDistanceFromBathRoom.Count - 1);
                float baseFormula = (float)(1 - formula1 / formula2);
                float floatFactor = (float)Personnage.distanceToilette;

                if (Personnage.distanceToilette == 1)
                    distanceToilette = baseFormula;
                else
                    distanceToilette = Mathf.Pow(baseFormula, floatFactor);
            }

            distanceToilette *= 100;
            Debug.Log("Distance Toilettes for " + Personnage.role + " (Toilettes Indice Satis. = " + Personnage.distanceToilette+ ") is " + distanceToilette + " in room " + Personnage.piece.id + ", proximity rank is " + bathRoomDistanceRank);
            #endregion

            #region Satisfaction Copain
            //Calcule la satisfaction en fonction des personnages à coté de lui
            if (Personnage.copain != null && Personnage.copain.piece != null)
            {
                float rank = Personnage.piece.roomDistancesid.FindIndex(a => a == Personnage.copain.piece.id);
                aCoteCopain = (rank / 4f) * 100f;
                Personnage.copain.CalculSatisfaction();
            }
            #endregion

            #region Satisfaction Prod
            if (Personnage.myProductiveLink != null && Personnage.myProductiveLink.piece != null)
            {
                float rank = Personnage.piece.roomDistancesid.FindIndex(a => a == Personnage.myProductiveLink.piece.id);
                productiveLinkSatisfaction = (rank / 4f) * 100f;
                Personnage.myProductiveLink.CalculSatisfaction();
            }
            #endregion


            satisfactionTotale = surface + luminosite + distanceSallePause + distanceToilette;
            int nbrParam = 4;
            if (Personnage.accesExterieur)
            {
                satisfactionTotale += accesExterieur;
                nbrParam++;
            }
            if (Personnage.copain != null)
            {
                satisfactionTotale += aCoteCopain;
                nbrParam++;
            }
            if (Personnage.myProductiveLink != null)
            {
                satisfactionTotale += productiveLinkSatisfaction;
                nbrParam++;
            }

            satisfactionTotale = satisfactionTotale/nbrParam;
        }

        public void Reset()
        {
            this.surface = 0;
            this.luminosite = 0;
            this.satisfactionTotale = 0;
            this.productiveLinkSatisfaction = 0;
            this.aCoteCopain = 0;
            this.accesExterieur = 0;
            this.distanceSallePause = 0;
            this.distanceToilette = 0;
        }
    }
}
