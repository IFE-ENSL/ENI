using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Management
{
    public class SatisfactionGlobale : MonoBehaviour
    {
        public GameObject grillePersonnages;
        public Text textPourcentage;

        public float satisfactionGlobale { get; private set; }

        //Calcule la satisfaction globale en itérant dans tous les personnages présent dans une pièce
        public void CalculSatisfactionGlobale()
        {
            int nbrPersonnage = 0;
            satisfactionGlobale = 0;
            foreach (SatisfactionPersonnage s in from Transform child in grillePersonnages.transform select child.GetComponent<SatisfactionPersonnage>())
            {
                if (s.Personnage.piece)
                {
                    satisfactionGlobale += s.satisfactionTotale;
                    nbrPersonnage++;
                }
            }
            satisfactionGlobale = satisfactionGlobale/nbrPersonnage;
            textPourcentage.text = satisfactionGlobale + " %";
        }
    }
}
