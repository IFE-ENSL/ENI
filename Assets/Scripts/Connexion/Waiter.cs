//Classe permettant de récupérer des informations lors de l'appel des méthodes de connexion
//Exemple : Savoir si l'envoie du log est terminé + récupérer des données de retour afin de savoir si le tout s'est bien passé
namespace Assets.Scripts.Connexion
{
    public class Waiter
    {
        public bool waiting;
        public string data;

        public void Reset()
        {
            waiting = false;
            data = "";
        }
    }
}
