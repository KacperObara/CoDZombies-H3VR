#if H3VR_IMPORTED
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts
{
    public class HighscoreShower : MonoBehaviour
    {
        public Text HighscoreText;

        private void Start()
        {
            HighscoreText.text = "Highscore: " + PlayerPrefs.GetInt("BestScore").ToString();
        }
    }
}
#endif