using CODZombies.Scripts.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts.Player
{
    public class Pause : MonoBehaviour
    {
        [SerializeField] private GameObject _pauseWrapper;
        [SerializeField] private GameObject _playerBlocker;

        [SerializeField] private Text _isPausedText;

        private bool _isPaused;

        private void Awake()
        {
            if (_playerBlocker)
                _playerBlocker.SetActive(false);

            RoundManager.RoundEnded += Show;
            RoundManager.RoundStarted += Hide;

            _isPausedText.text = "Disabled";

            Hide();
        }

        private void Show()
        {
            _pauseWrapper.SetActive(true);
        }

        private void Hide()
        {
            _pauseWrapper.SetActive(false);
        }

        public void OnLeverPull()
        {
            if (_isPaused)
            {
                RoundManager.Instance.ResumeGame();
                _isPaused = false;

                if (_playerBlocker)
                    _playerBlocker.SetActive(false);

                _isPausedText.text = "Disabled";
            }
            else
            {
                RoundManager.Instance.PauseGame();
                _isPaused = true;

                if (_playerBlocker)
                    _playerBlocker.SetActive(true);

                _isPausedText.text = "Enabled";
            }
        }

        private void OnDestroy()
        {
            RoundManager.RoundEnded -= Show;
            RoundManager.RoundStarted -= Hide;
        }
    }
}
