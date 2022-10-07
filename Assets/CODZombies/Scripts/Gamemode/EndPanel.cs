#if H3VR_IMPORTED

using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers;
using UnityEngine.UI;

namespace CODZombies.Scripts.Gamemode
{
    public class EndPanel : MonoBehaviourSingleton<EndPanel>
    {
        public Text TotalPointsText;
        public Text BestPointsText;
        public Text KillsText;

        public Text DifficultyText;
        public Text EnemiesTypeText;
        public Text LimitedAmmoText;
        public Text SpecialRoundsText;
        public Text LessZombieHPText;


        public void UpdatePanel()
        {
            TotalPointsText.text = "Total Points:\n" + GameManager.Instance.TotalPoints;
            BestPointsText.text = "High Score:\n" + SaveSystem.Instance.GetHighscore();

            KillsText.text = "Kills:\n" + GameManager.Instance.Kills;

            DifficultyText.text = GameSettings.HardMode ? "Hard" : "Normal";
            EnemiesTypeText.text = GameSettings.UseCustomEnemies ? "Custom" : "Normal";
            LimitedAmmoText.text = GameSettings.LimitedAmmo ? "Yes" : "No";
            SpecialRoundsText.text = GameSettings.SpecialRoundDisabled ? "No" : "Yes";
            LessZombieHPText.text = GameSettings.WeakerEnemiesEnabled ? "Yes" : "No";
        }
    }
}

#endif