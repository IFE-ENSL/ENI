using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.Linq;

public class ObjectifLine : MonoBehaviour
{
    public int _idUserObjMission;
    public int _ObjMission;
    public string _libelleObjMission;
    public string _TB;
    public string _B;
    public string _M;
    public string _I;
    public int _point;

    [SerializeField]
    List<Image> choices = new List<Image>();
    public static System.Random getRandom = new System.Random();
    List<Vector3> choicesStartPos = new List<Vector3>();

    void RandomizeTextPositions ()
    {
        List<int> orders = new List<int>();
        for (int iterator = 0; iterator < 4; iterator++)
        {
            int random = getRandom.Next(4);
            while (orders.Contains(random))
            {
                random = getRandom.Next(4);
            }

            orders.Add(random);
        }

        int i = 0;
        foreach (int order in orders)
        {
            choices[i].rectTransform.SetSiblingIndex(order + 1);
            i++;
        }

        transform.GetChild(1).GetComponent<Toggle>().isOn = true;
        Debug.Log("Randomized choice order with this sequence = " + orders[0] + " + " + orders[1] + " + " + orders[2] + " + " + orders[3]);
    }

    public void SetObjectifLine (int idUserObjMission, int ObjMission, string libelleObjMission, List<string> choiceList, int point)
    {
        int iterator = 0;
        foreach (Image choice in choices)
        {
            choicesStartPos.Add(choice.rectTransform.position);
            choice.GetComponentInChildren<Text>().text = choiceList[iterator];
            iterator++;
        }
    
        _idUserObjMission = idUserObjMission;
        _ObjMission = ObjMission;
        transform.Find("LibellePic").GetComponentInChildren<Text>().text = libelleObjMission;
        _point = point;

        RandomizeTextPositions();

    }

}
