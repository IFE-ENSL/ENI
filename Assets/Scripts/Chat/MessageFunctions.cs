using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Chat
{
    public class MessageFunctions : MonoBehaviour {
        [SerializeField]
        public Text message;

        public void ShowMessage(string message)
        {
            this.message.text = message;
        }
    }
}
