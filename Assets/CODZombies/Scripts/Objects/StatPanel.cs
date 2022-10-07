#if H3VR_IMPORTED
using CODZombies.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts.Objects
{
    public class StatPanel : MonoBehaviour
    {
        public Text RoundText;
        public Text PointsText;
        public Text LeftText;

        private void Start()
        {
            GameManager.OnPointsChanged -= UpdatePointsText;
            GameManager.OnPointsChanged += UpdatePointsText;

            RoundManager.OnRoundChanged -= UpdateRoundText;
            RoundManager.OnRoundChanged += UpdateRoundText;

            RoundManager.OnZombiesLeftChanged -= UpdateLeftText;
            RoundManager.OnZombiesLeftChanged += UpdateLeftText;

            UpdateRoundText();
            UpdatePointsText();
        }

        private void OnDestroy()
        {
            GameManager.OnPointsChanged -= UpdatePointsText;
            RoundManager.OnRoundChanged -= UpdateRoundText;
            RoundManager.OnZombiesLeftChanged -= UpdateLeftText;
        }

        private void UpdateRoundText()
        {
            RoundText.text = "Round:\n" + RoundManager.Instance.RoundNumber;
        }

        private void UpdatePointsText()
        {
            PointsText.text = "Points:\n" + GameManager.Instance.Points;
        }

        private void UpdateLeftText()
        {
            LeftText.text = "Left:\n" + ZombieManager.Instance.ZombiesRemaining;
        }
    }
}
#endif