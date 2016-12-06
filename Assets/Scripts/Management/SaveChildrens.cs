using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Management
{
    //Cette classe permet de sauvegarder / charger la liste des personnages
    public class SaveChildrens : MonoBehaviour
    {

        public GameObject prefabPersonnage;
        public Sprite[] avatars;

        /*void Save(string fileName)
        {
            List<int> idPersonnages = new List<int>();
            foreach (Personnage p in from Transform child in transform select child.GetComponent<Personnage>())
            {
                p.Save(fileName);
                idPersonnages.Add(p.persoId);
            }
            ES2.Save(idPersonnages, fileName + "?tag=" + name + "idPersonnages");
        }

        void Load(string fileName)
        {
            List<int> idPersonnages = ES2.LoadList<int>(fileName + "?tag=" + name + "idPersonnages");
            foreach (int id in idPersonnages)
            {
                print("Chargement du personnage" + id);
                GameObject personnage = Instantiate(prefabPersonnage);
                personnage.name = "Personnage" + id;
                personnage.transform.parent = transform;
                personnage.GetComponent<Personnage>().Load(fileName);
            }
        }*/
    }
}
