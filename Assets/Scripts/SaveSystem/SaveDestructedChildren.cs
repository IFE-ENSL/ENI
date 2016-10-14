using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.SaveSystem
{
    //Sauvegarde les objets détruits des enfants d'un gameobject, afin de les redétruire lors du chargement d'une sauvegarde
    public class SaveDestructedChildren : MonoBehaviour {

        [HideInInspector]
        public List<string> childrens;
        public string saveTag = "";

        void Start()
        {
            childrens = new List<string>();
        }
        public void Save(string fileName)
        {
            //StartCoroutine(UploadSave(this.childrens, saveTag));
            ES2.Save(this.childrens, fileName + "?tag=" + saveTag);
        }

        public void Load(string fileName)
        {
            this.childrens = ES2.LoadList<string>(fileName + "?tag=" + saveTag);
            foreach (string go in childrens)
            {
                Destroy(GameObject.Find(go));
            }
        }
    }
}
