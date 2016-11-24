using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Competences : PropertyAttribute
{
    public string Name;
    public float nbPointsCompetence;
    public List<Criteres> listeCriteres;
}
