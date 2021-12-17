#if H3VR_IMPORTED
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts.Gamemode
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
            PointsText.text = "Points:\n" + GameManager.Instance.Points.ToString();
        }
    }
}
#endif