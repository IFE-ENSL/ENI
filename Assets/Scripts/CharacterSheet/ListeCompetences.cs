using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CompetenceENI : PropertyAttribute //This class is used to easily store the CompetenceENIs in convenient Lists
{
    public string _Name;
    public int _MainSkillNumber;
    public int _nbPointsCompetence;
    public List<Criteres> _listeCriteres;
    public int _CriteriaNumber;
    public int _idJM;

    public CompetenceENI(string Name, int MainSkillNumber, int nbPointsCompetence, int CriteriaNumber, int idJM)
    {
        _Name = Name;
        _MainSkillNumber = MainSkillNumber;
        _nbPointsCompetence = nbPointsCompetence;
        _CriteriaNumber = CriteriaNumber;
        _idJM = idJM;
    }

    public CompetenceENI (string Name, int MainSkillNumber, int nbPointsCompetence, List<Criteres> listeCriteres)
    {
        _Name = Name;
        _MainSkillNumber = MainSkillNumber;
        _nbPointsCompetence = nbPointsCompetence;
    }

    public CompetenceENI(string Name, int MainSkillNumber, int nbPointsCompetence)
    {
        _Name = Name;
        _MainSkillNumber = MainSkillNumber;
        _nbPointsCompetence = nbPointsCompetence;
    }
}
