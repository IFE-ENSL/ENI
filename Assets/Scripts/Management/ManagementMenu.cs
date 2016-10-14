using Assets.Scripts.SaveSystem;
using UnityEngine;

namespace Assets.Scripts.Management
{
    //Classe gérant le menu du management
    public class ManagementMenu : MonoBehaviour
    {

        private SaveManager saveManager;
        public GameObject menu;

        void Start()
        {
            saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        }

        public void Save()
        {
            saveManager.Save();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menu.SetActive(!menu.activeInHierarchy);
            }
        }
    }
}
