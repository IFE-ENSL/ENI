using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Criteres : PropertyAttribute
{
    public string Name;
    public int criterePoints;
    public int criterePalliers = 3;
}
