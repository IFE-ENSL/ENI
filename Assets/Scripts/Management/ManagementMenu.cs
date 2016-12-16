using Assets.Scripts.SaveSystem;
using UnityEngine;

namespace Assets.Scripts.Management
{
    //Classe gérant le menu du management
    public class ManagementMenu : MonoBehaviour
    {
        public GameObject menu;

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                menu.SetActive(!menu.activeInHierarchy);
            }
        }
    }
}
