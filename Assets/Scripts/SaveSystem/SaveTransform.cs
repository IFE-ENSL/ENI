using UnityEngine;

namespace Assets.Scripts.SaveSystem
{
    //Permet de sauvegarder la position d'un objet dans le jeu
    public class SaveTransform : MonoBehaviour {

        public void Save(string fileName)
        {
            //StartCoroutine(UploadSave(this.gameObject.transform, this.gameObject.name + "Transform"));
            ES2.Save(this.gameObject.transform, fileName + "?tag=" + this.gameObject.name + "Transform");
        }

        public void Load(string fileName)
        {
            ES2.Load<Transform>(fileName + "?tag=" + this.gameObject.name + "Transform", this.gameObject.transform);
        }
    }
}
