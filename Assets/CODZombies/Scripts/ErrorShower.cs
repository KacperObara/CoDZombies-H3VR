using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts.Gamemode.GMDebug
{
    public class ErrorShower : MonoBehaviourSingleton<ErrorShower>
    {
        public Text ErrorText;

        public void Show(string message)
        {
            ErrorText.text = message;
            ErrorText.gameObject.SetActive(true);
        }
    }
}