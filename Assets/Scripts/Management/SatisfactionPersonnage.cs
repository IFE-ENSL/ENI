using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Management
{

    //Obviously this class is used to calculate the satisfaction of a character
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

        public ManagementCharacter Character { get; private set; }


        void Start()
        {
            Character = GetComponent<ManagementCharacter>();
        }

        public IEnumerator UpdateSatisfaction()
        {
            while (!Character.room)
            {
                yield return new WaitForSeconds(0.1f);
            }

            #region Satisfaction Surface
            surface = Character.room.surface > Character.surfaceSalarie ? 1 : (Character.room.surface / Character.surfaceSalarie) * (Character.room.surface / Character.surfaceSalarie);
            surface *= 100;
            #endregion

            #region Satisfaction Acces Exterieur
            if (Character.accesExterieur)
            {
                accesExterieur = Character.room.accesExterieur ? 100 : 0;
            }
            #endregion

            #region Satisfaction Luminosite
            if (Character.luminosite == 0)
                luminosite = 1;
            else if (Character.luminosite == 1)
            {
                if (Character.room.ouvertureExterieur <= 4)
                    luminosite = 1;
                else
                    luminosite = 0.5f;
            }
            else if (Character.room.ouvertureExterieur <= 2)
            {
                luminosite = 1;
            }
            else if (Character.room.ouvertureExterieur <= 4)
            {
                luminosite = 0.6f;
            }
            else
                luminosite = 0.2f;

            luminosite *= 100;
            #endregion

            #region Satsifaction Distance Salle Pause
            int breakRoomDistanceRank = GameManager.roomDistanceFromBreakRoom.FindIndex(x => x == Character.room.distanceSallePause) + 1;

            if (Character.distanceSallePause == 0)
                distanceSallePause = 1;
            else
            {
                float formula1 = (breakRoomDistanceRank - 1);
                float formula2 = (GameManager.roomDistanceFromBreakRoom.Count - 1);
                float baseFormula = (float)(1 - formula1 / formula2);
                float floatFactor = (float)Character.distanceSallePause;

                if (Character.distanceSallePause == 1)
                    distanceSallePause = baseFormula;
                else
                    distanceSallePause = Mathf.Pow(baseFormula, floatFactor);
            }

            distanceSallePause *= 100;
            #endregion

            #region Satisfaction Distance Toilette
            int bathRoomDistanceRank = GameManager.roomDistanceFromBathRoom.FindIndex(x => x == Character.room.distanceToilette) + 1;

            if (Character.distanceToilette == 0)
                distanceToilette = 1;
            else
            {
                float formula1 = (bathRoomDistanceRank - 1);
                float formula2 = (GameManager.roomDistanceFromBathRoom.Count - 1);
                float baseFormula = (float)(1 - formula1 / formula2);
                float floatFactor = (float)Character.distanceToilette;

                if (Character.distanceToilette == 1)
                    distanceToilette = baseFormula;
                else
                    distanceToilette = Mathf.Pow(baseFormula, floatFactor);
            }

            distanceToilette *= 100;
            #endregion

            satisfactionTotale = surface + luminosite + distanceSallePause + distanceToilette;
            int nbrParam = 4;
            if (Character.accesExterieur)
            {
                satisfactionTotale += accesExterieur;
                nbrParam++;
            }
            if (Character.friend != null)
            {
                satisfactionTotale += aCoteCopain;
                nbrParam++;
            }
            if (Character.myProductiveLink != null)
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
