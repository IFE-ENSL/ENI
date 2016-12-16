﻿using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Management
{
    public class SatisfactionGlobale : MonoBehaviour
    {
        public GameObject go_characterGrid;
        public Text textPourcentage;

        public float satisfactionGlobale { get; private set; }

        //Calcule la satisfaction globale en itérant dans tous les personnages présent dans une pièce
        public void UpdateGlobalSatisfaction()
        {
            int nbrPersonnage = 0;
            satisfactionGlobale = 0;
            foreach (SatisfactionPersonnage s in from Transform child in go_characterGrid.transform select child.GetComponent<SatisfactionPersonnage>())
            {
                if (s.Character.room)
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
