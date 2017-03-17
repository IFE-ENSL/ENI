using UnityEngine;
using System.Collections;

public class SeanceNumberForButton : MonoBehaviour
{
    public int seanceNumber = 0;

    public void CallAutoEvalOnClick ()
    {
        GameObject.FindObjectOfType<MissionInterface>().GoToAutoEval(seanceNumber);
    }
}
