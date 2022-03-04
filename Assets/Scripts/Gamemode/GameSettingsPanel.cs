#if H3VR_IMPORTED
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts
{
    public class GameSettingsPanel : MonoBehaviour
    {
        //TODO: Set the OnClick actions
        // public FVRPointableButton LimitedAmmo;
        // public FVRPointableButton MoreEnemies;
        // public FVRPointableButton FasterEnemies;
        // public FVRPointableButton WeakerEnemies;
        // public FVRPointableButton StartGame;

        public Text LimitedAmmo;
        public Text MoreEnemies;
        public Text FasterEnemies;
        public Text WeakerEnemies;
        public Text BackgroundMusic;
        public Text UseZosigs;
        public Text DisableSpecialRound;
        public Text ItemSpawnerStatus;

        private void Awake()
        {
            GameSettings.OnSettingsChanged += UpdateText;
        }

        private void OnDestroy()
        {
            GameSettings.OnSettingsChanged -= UpdateText;
        }

        private void UpdateText()
        {
            MoreEnemies.text = GameSettings.MoreEnemies ? "Enabled" : "Disabled";
            FasterEnemies.text = GameSettings.FasterEnemies ? "Enabled" : "Disabled";
            WeakerEnemies.text = GameSettings.WeakerEnemies ? "Enabled" : "Disabled";
            LimitedAmmo.text = GameSettings.LimitedAmmo ? "Enabled" : "Disabled";
            BackgroundMusic.text = GameSettings.BackgroundMusic ? "Enabled" : "Disabled";
            UseZosigs.text = GameSettings.UseCustomEnemies ? "Enabled" : "Disabled";
            ItemSpawnerStatus.text = GameSettings.ItemSpawnerSpawned ? "Spawned" : "Not spawned";
            DisableSpecialRound.text = GameSettings.SpecialRoundEnabled ? "Enabled" : "Disabled";
        }
    }
}
#endif