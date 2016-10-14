namespace Assets.Scripts.Utility
{
    //Cette classe définit les propriétés d'un log de type labyrinthe. Cette méthode permet de facilement convertir la classe en objet JSON, puis ensuite de l'envoyer au backoffice
    public class LogLabyrinthe : ILog
    {

        public string chemin = null;
        public string zone = null;
        public string duree = null;
        public string numCheckpoint = null;
        public string nbrEtoiles = null;
        public string dureeDebutJeu = null;

        public LogLabyrinthe(string chemin, string zone, string duree,string dureeDebutJeu, string numCheckpoint, string nbrEtoiles)
        {
            this.chemin = chemin;
            this.zone = zone;
            this.duree = duree;
            this.dureeDebutJeu = dureeDebutJeu;
            this.numCheckpoint = numCheckpoint;
            this.nbrEtoiles = nbrEtoiles;
        }

        public LogLabyrinthe()
        {
        
        }
    }
}
