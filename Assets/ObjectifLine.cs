using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    public void SetObjectifLine (int idUserObjMission, int ObjMission, string libelleObjMission, string TB, string B, string M, string I, int point)
    {
        _idUserObjMission = idUserObjMission;
        _ObjMission = ObjMission;
        _libelleObjMission = libelleObjMission;
        _TB = TB;
        _B = B;
        _M = M;
        _I = I;
        _point = point;
    }

}
