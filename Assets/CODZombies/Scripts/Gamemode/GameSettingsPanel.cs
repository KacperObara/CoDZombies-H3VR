#if H3VR_IMPORTED
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts.Gamemode
{
    public class GameSettingsPanel : MonoBehaviour
    {
        public Text BackgroundMusic;

        public Text DifficultyNormalText;
        public Text DifficultyHardText;
        public Text CustomEnemiesEnabledText;
        public Text CustomEnemiesDisabledText;
        public Text LimitedAmmoEnabledText;
        public Text LimitedAmmoDisabledText;
        public Text SpecialRoundEnabledText;
        public Text SpecialRoundDisabledText;
        public Text ItemSpawnerEnabledText;
        public Text ItemSpawnerDisabledText;
        public Text WeakerZombiesEnabledText;
        public Text WeakerZombiesDisabledText;

        public Color EnabledColor;
        public Color DisabledColor;

        private void Awake()
        {
            GameSettings.OnSettingsChanged += UpdateText;
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnDestroy()
        {
            GameSettings.OnSettingsChanged -= UpdateText;
        }

        private void UpdateText()
        {
            BackgroundMusic.text = GameSettings.BackgroundMusic ? "Enabled" : "Disabled";

            DifficultyNormalText.color = GameSettings.HardMode ? DisabledColor : EnabledColor;
            DifficultyHardText.color = GameSettings.HardMode ? EnabledColor : DisabledColor;

            CustomEnemiesEnabledText.color = GameSettings.UseCustomEnemies ? EnabledColor : DisabledColor;
            CustomEnemiesDisabledText.color = GameSettings.UseCustomEnemies ? DisabledColor : EnabledColor;

            LimitedAmmoEnabledText.color = GameSettings.LimitedAmmo ? EnabledColor : DisabledColor;
            LimitedAmmoDisabledText.color = GameSettings.LimitedAmmo ? DisabledColor : EnabledColor;

            SpecialRoundEnabledText.color = GameSettings.SpecialRoundDisabled ? DisabledColor : EnabledColor;
            SpecialRoundDisabledText.color = GameSettings.SpecialRoundDisabled ? EnabledColor : DisabledColor;

            ItemSpawnerEnabledText.color = GameSettings.ItemSpawnerEnabled ? EnabledColor : DisabledColor;
            ItemSpawnerDisabledText.color = GameSettings.ItemSpawnerEnabled ? DisabledColor : EnabledColor;

            WeakerZombiesEnabledText.color = GameSettings.WeakerEnemiesEnabled ? EnabledColor : DisabledColor;
            WeakerZombiesDisabledText.color = GameSettings.WeakerEnemiesEnabled ? DisabledColor : EnabledColor;

            foreach (var lootPool in GameSettings.Instance.LootPoolChoices)
            {
                lootPool.LootChoiceText.color = lootPool.IsEnabled ? EnabledColor : DisabledColor;
            }
        }
    }
}
#endif