using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts
{
    public class EndPanel : MonoBehaviour
    {
        public Text TotalPointsText;
        public Text BestPointsText;

        public void UpdatePanel()
        {
            TotalPointsText.text = "Total score: " + GameManager.Instance.TotalPoints;
            BestPointsText.text = "Best score: " + PlayerPrefs.GetInt("BestScore");
        }
    }
}