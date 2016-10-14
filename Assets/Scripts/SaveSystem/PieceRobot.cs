using System;
using System.Collections;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace Assets.Scripts.SaveSystem
{
    public enum TypePieceRobot
    {
        Bronze = 0,
        Argent = 1,
        Or = 2
    }

    //Cette classe permet de sauvegarder les pièces de robot obtenues par le joueur

    public class PieceRobot : MonoBehaviour
    {
        private int? _jambes;

        public int? Jambes
        {
            get { return _jambes; }
            set {
                _jambes = value > 3 ? 3 : value;
            }
        }

        public int? Bras { get; set; }

        public void Start()
        {
            DontDestroyOnLoad(this.gameObject);
            _jambes = null;
            Bras = null;
        }

        public void Save(string fileName)
        {
            if (_jambes != null)
                ES2.Save(this._jambes, fileName + "?tag=" + name + "PieceRobotJambe");
            if(Bras != null)
                ES2.Save(this.Bras, fileName + "?tag=" + name + "PieceRobotBras");
        }

        public void Load(string fileName)
        {
            if (ES2.Exists(fileName + "?tag=" + name + "PieceRobotJambe"))
                _jambes = ES2.Load<int>(fileName + "?tag=" + name + "PieceRobotJambe");
            if (ES2.Exists(fileName + "?tag=" + name + "PieceRobotBras"))
                Bras = ES2.Load<int>(fileName + "?tag=" + name + "PieceRobotBras");

        }




    }
}