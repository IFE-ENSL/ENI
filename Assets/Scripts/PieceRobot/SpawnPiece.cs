using UnityEngine;
using System.Collections;
using Assets.Scripts.SaveSystem;

//Classe utilisée afin de charger l'affichage des pièces de robot en fonction des pièces obtenues par le joueur
public class SpawnPiece : MonoBehaviour
{

    private PieceRobot _pieceRobot;
    public GameObject[] gosJambeRobot;
    public GameObject[] gosBrasRobot;
    void Start()
    {
        _pieceRobot = GameObject.Find("PlayerData").GetComponent<PieceRobot>();
        if(!_pieceRobot)
            Debug.LogError("Systeme de sauvegarde non chargé");
        if (_pieceRobot.Jambes != null) gosJambeRobot[(int)_pieceRobot.Jambes].SetActive(true);
        if (_pieceRobot.Bras != null) gosBrasRobot[(int)_pieceRobot.Bras].SetActive(true);
    }


}
