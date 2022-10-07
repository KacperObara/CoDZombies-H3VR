using CODZombies.Scripts.Common;
using UnityEngine.UI;

namespace CODZombies.Scripts
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