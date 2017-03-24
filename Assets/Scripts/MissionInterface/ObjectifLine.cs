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
    public bool choseOneToggle = false;
    public List<int> linkedCompENI = new List<int>();

    [SerializeField]
    List<Image> choices = new List<Image>();
    public static System.Random getRandom = new System.Random();
    List<Vector3> choicesStartPos = new List<Vector3>();

    //Randomizing each block's position inside a line
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
    }

    public void changedValue () //Called via Unity UI when the toggle value is changed for this line
    {
        foreach (Image choice in choices)
        {
            if (choice.GetComponent<Toggle>().isOn)
            {
                switch (choice.name)
                {
                    case "TBToggle":
                        _point = 3;
                        break;
                    case "BToggle":
                        _point = 2;
                        break;
                    case "MToggle":
                        _point = 1;
                        break;
                    case "IToggle":
                        _point = 0;
                        break;
                }
                break;
            }
        }

        choseOneToggle = true;
    }

    //Setting each new line with the datas retrieved by the server
    public void SetObjectifLine (int idUserObjMission, int ObjMission, string libelleObjMission, List<string> choiceList, int point, bool AlreadyValidatedOnce)
    {
        int iterator = 0;
        foreach (Image choice in choices)
        {
            choicesStartPos.Add(choice.rectTransform.position);
            choice.GetComponentInChildren<Text>().text = choiceList[iterator];

            if (AlreadyValidatedOnce)
            {
                if (choice.name == "IToggle" && point == 0)
                    choice.GetComponent<Toggle>().isOn = true;
                if (choice.name == "MToggle" && point == 1)
                    choice.GetComponent<Toggle>().isOn = true;
                if (choice.name == "BToggle" && point == 2)
                    choice.GetComponent<Toggle>().isOn = true;
                if (choice.name == "TBToggle" && point == 3)
                    choice.GetComponent<Toggle>().isOn = true;
            }

            iterator++;
        }
    
        _idUserObjMission = idUserObjMission;
        _ObjMission = ObjMission;
        transform.Find("LibellePic").GetComponentInChildren<Text>().text = libelleObjMission;
        _point = point;

        RandomizeTextPositions();

    }

}
