using UnityEngine;
using System.Collections;

static public class SQLCommonVars
{
    public const string baseURL = "http://vm-web7.ens-lyon.fr/eni"; //Prod
    //public const string baseURL = "http://127.0.0.1/eni"; //Local
    //public const string baseURL = "http://vm-web-qualif.pun.ens-lyon.fr/eni/"; //Preprod

    public const string getUserStats = baseURL + "/web/app.php/unity/management/initJeu";
    public const string addLogURL = baseURL + "/web/app.php/unity/addLog";
    public const string loginURL = baseURL + "/web/app.php/unity/loginOne";
    public const string checkLoginURL = baseURL + "/web/app.php/unity/loginCheck";
    public const string addTokenURL = baseURL + "/web/app.php/unity/loginToken";
    public const string insertMessageURL = baseURL + "/web/app.php/unity/insertMessage";
    public const string getMessagesURL = baseURL + "/web/app.php/unity/getMessages";
    public const string endSessionURL = baseURL + "/web/app.php/unity/endSession";

    public const string getPiecesURL = baseURL + "/web/app.php/unity/management/getPieces";
    public const string getPiecesDistancesURL = baseURL + "/web/app.php/unity/management/getPiecesDistances";
    public const string getPersonnagesURL = baseURL + "/web/app.php/unity/management/getPersonnages";
    public const string insertSessionURL = baseURL + "/web/app.php/unity/management/insertSessionPersonnage";
    public const string insertSessionMiniJeuURL = baseURL + "/web/app.php/unity/management/insertNewSessionMiniJeu";
    public const string updateAvatarURL = baseURL + "/web/app.php/unity/management/updateAvatar";

    public const string getMissionDatasURL = baseURL + "/web/app.php/unity/management/initMission";
}
