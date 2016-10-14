namespace Assets.Scripts.Utility
{
    //Cette classe définit les propriétés d'un log de type management. Cette méthode permet de facilement convertir la classe en objet JSON, puis ensuite de l'envoyer au backoffice
    public class LogManagement : ILog
    {

        public int avatarId;
        public int personnageId;
        public int startRoom;
        public int endRoom;

        public LogManagement()
        {
        }

        public LogManagement(int avatarId, int personnageId)
        {
            this.avatarId = avatarId;
            this.personnageId = personnageId;
        }

        public LogManagement(int personnageId, int startRoom, int endRoom)
        {
            this.personnageId = personnageId;
            this.startRoom = startRoom;
            this.endRoom = endRoom;
        }
    }
}
