using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.Chat
{
    //Cette classe permet de bouger une fenêtre
    public class DragableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler {

        private Vector3 offset;

        public void OnBeginDrag(PointerEventData eventData)
        {
            offset = this.transform.position - (Vector3)eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            transform.position = (Vector3)eventData.position + offset;
        }
    }
}
