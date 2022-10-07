#if H3VR_IMPORTED
using CODZombies.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts.Gamemode
{
    public class PointsView : MonoBehaviour
    {
        public Text PointsText;

        private void Awake()
        {
            GameManager.OnPointsChanged += OnPointsChanged;
        }

        private void OnDestroy()
        {
            GameManager.OnPointsChanged -= OnPointsChanged;
        }

        private void OnPointsChanged()
        {
            PointsText.text = "Points:\n" + GameManager.Instance.Points;
        }
    }
}
#endif