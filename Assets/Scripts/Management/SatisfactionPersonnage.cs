using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Management
{

    //Cette classe liée à un managementCharacter est utilisée pour mémoriser chaque critère de satisfaction
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

        public IEnumerator UpdateSatisfaction()
        {
            while (!Personnage.room)
            {
                yield return new WaitForSeconds(0.1f);
            }

            #region Satisfaction Surface
            surface = Personnage.room.surface > Personnage.surfaceSalarie ? 1 : (Personnage.room.surface / Personnage.surfaceSalarie) * (Personnage.room.surface / Personnage.surfaceSalarie);
            surface *= 100;
            #endregion

            #region Satisfaction Acces Exterieur
            if (Personnage.accesExterieur)
            {
                accesExterieur = Personnage.room.accesExterieur ? 100 : 0;
            }
            #endregion

            #region Satisfaction Luminosite
            if (Personnage.luminosite == 0)
                luminosite = 1;
            else if (Personnage.luminosite == 1)
            {
                if (Personnage.room.ouvertureExterieur <= 4)
                    luminosite = 1;
                else
                    luminosite = 0.5f;
            }
            else if (Personnage.room.ouvertureExterieur <= 2)
            {
                luminosite = 1;
            }
            else if (Personnage.room.ouvertureExterieur <= 4)
            {
                luminosite = 0.6f;
            }
            else
                luminosite = 0.2f;

            luminosite *= 100;
            #endregion

            #region Satsifaction Distance Salle Pause
            int breakRoomDistanceRank = GameManager.roomDistanceFromBreakRoom.FindIndex(x => x == Personnage.room.distanceSallePause) + 1;

            if (Personnage.distanceSallePause == 0)
                distanceSallePause = 1;
            else
            {
                float formula1 = (breakRoomDistanceRank - 1);
                float formula2 = (GameManager.roomDistanceFromBreakRoom.Count - 1);
                float baseFormula = (float)(1 - formula1 / formula2);
                float floatFactor = (float)Personnage.distanceSallePause;

                if (Personnage.distanceSallePause == 1)
                    distanceSallePause = baseFormula;
                else
                    distanceSallePause = Mathf.Pow(baseFormula, floatFactor);
            }

            distanceSallePause *= 100;
            #endregion

            #region Satisfaction Distance Toilette
            int bathRoomDistanceRank = GameManager.roomDistanceFromBathRoom.FindIndex(x => x == Personnage.room.distanceToilette) + 1;

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
