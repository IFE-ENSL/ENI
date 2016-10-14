using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.Menu
{
	//Cette classe permet de naviguer dans le menu a l'aide du clavier
    public class InputNavigator : MonoBehaviour
    {
        private EventSystem system;
        public GameObject firstInput;

        void Start()
        {
            system = EventSystem.current; // EventSystemManager.currentSystem;
            system.SetSelectedGameObject(firstInput);

        }

        // Update is called once per frame
        void Update()
        {
            tabLines();
            enterButton();
        }

        void tabLines()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                //Debug.Log(system.currentSelectedGameObject.name);
                Selectable nextDown = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

                if (nextDown != null && nextDown.gameObject.name != "Login")
                {

                    InputField inputfield = nextDown.GetComponent<InputField>();
                    if (inputfield != null)
                        inputfield.OnPointerClick(new PointerEventData(system));

                    system.SetSelectedGameObject(nextDown.gameObject, new BaseEventData(system));
                }
                else
                {
                    Selectable nextUp = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();
                    if (nextUp != null)
                    {

                        InputField inputfield = nextUp.GetComponent<InputField>();
                        if (inputfield != null)
                            inputfield.OnPointerClick(new PointerEventData(system));

                        system.SetSelectedGameObject(nextUp.gameObject, new BaseEventData(system));
                    }
                }

            }
        }

        void enterButton()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                Selectable nextDown = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();
                if (nextDown != null && nextDown.gameObject.name == "Login")
                {
                    Button btn = nextDown.GetComponent<Button>();
                    if (btn != null)
                    {
                        btn.OnPointerClick(new PointerEventData(system));

                        system.SetSelectedGameObject(nextDown.gameObject, new BaseEventData(system));
                    }
                }
            }
        }
    }
}