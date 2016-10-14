using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Chat
{
    //Cette classe permet d'envoyer un message de chat en appuyant sur la touche entrée
    public class ChatInputNavigator : MonoBehaviour
    {
        private EventSystem _system;
        void Start () {
            _system = EventSystem.current;
        }

        // Update is called once per frame
        void Update () {
            if (!_system.currentSelectedGameObject || !Input.GetKeyDown(KeyCode.Return) ||
                _system.currentSelectedGameObject.name != "Chat InputField") return;
            var pointer = new PointerEventData(_system);
            ExecuteEvents.Execute(GameObject.Find("Chat Send Button"), pointer, ExecuteEvents.pointerClickHandler);
        }
    }
}
