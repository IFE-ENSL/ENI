using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class QualityList : PropertyAttribute
{
    public List<int> Qualities;
}
