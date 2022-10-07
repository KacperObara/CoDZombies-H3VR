#if H3VR_IMPORTED
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts.Gamemode
{
    public class HighscoreShower : MonoBehaviour
    {
        public Text HighscoreText;

        private void Start()
        {
            HighscoreText.text = "Highscore:\n" + SaveSystem.Instance.GetHighscore();
        }
    }
}
#endif