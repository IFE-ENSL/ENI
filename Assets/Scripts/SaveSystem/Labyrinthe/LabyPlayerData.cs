using UnityEngine;

namespace Assets.Scripts.SaveSystem.Labyrinthe
{
    //Cette classe sauvegarde les donées d'un joueur pour le mini jeu labyrinthe
    public class LabyPlayerData : MonoBehaviour
    {

        public int nbrCoins = 0;
        public int cheminChoisi = 0;
        public int lastCheckPoint = 0;
        public int nbrEntreeJeu = 0;
        public int nbrMortsAfterFirstCheckpoint = 0;
        public string zoneActuelle = "B1";
        public bool startMoving = false;

        [HideInInspector]
        public int[] meilleurTemps = new []{0,0}; // [0] = Minutes ; [1] = Seconds

        public void Save(string fileName)
        {
            ES2.Save(this.nbrCoins, fileName + "?tag=" + name + "nbrCoins");
            ES2.Save(this.meilleurTemps, fileName + "?tag=" + name + "meilleurTemps");
            ES2.Save(this.cheminChoisi, fileName + "?tag=" + name + "cheminChoisi");
            ES2.Save(this.cheminChoisi, fileName + "?tag=" + name + "nbrEntreeJeu");
        }

        public void Load(string fileName)
        {
            nbrCoins = ES2.Load<int>(fileName + "?tag=" + name + "nbrCoins");
            meilleurTemps = ES2.LoadArray<int>(fileName + "?tag=" + name + "meilleurTemps");
            cheminChoisi = ES2.Load<int>(fileName + "?tag=" + name + "cheminChoisi");
            nbrEntreeJeu = ES2.Load<int>(fileName + "?tag=" + name + "nbrEntreeJeu");
        }
    }
}
