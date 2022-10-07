#if H3VR_IMPORTED
using System;
using System.Collections.Generic;
using System.Linq;
using CODZombies.Scripts.Common;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace CODZombies.Scripts.Gamemode
{
    public class GameSettings : MonoBehaviourSingleton<GameSettings>
    {
        public static Action OnSettingsChanged;
        public static Action OnMusicSettingChanged;

        public LootPool DefaultLootPool;
        [HideInInspector] public LootPool CurrentLootPool;
        [HideInInspector] public List<LootPoolChoice> LootPoolChoices;

        public static bool HardMode;
        public static bool UseCustomEnemies;
        public static bool LimitedAmmo;
        public static bool SpecialRoundDisabled;
        public static bool ItemSpawnerEnabled;
        public static bool WeakerEnemiesEnabled;

        public static bool BackgroundMusic;

        public Text OptionDescriptionText;

        private void Start()
        {
            LootPoolChoices = FindObjectsOfType<LootPoolChoice>().ToList();

            if (LootPoolChoices.Count == 0)
            {
                Debug.LogError("No LootPoolChoices in the scene, At least one component has to be present");
            }

            if (DefaultLootPool)
            {
                CurrentLootPool = DefaultLootPool;
            }
            else
            {
                CurrentLootPool = LootPoolChoices[0].LootPool;
            }

            if (CurrentLootPool) // Highlight default loot pool option
            {
                foreach (var choice in LootPoolChoices)
                {
                    if (choice.LootPool == CurrentLootPool)
                        choice.IsEnabled = true;
                }
            }

            OptionDescriptionText.text = "Call of Duty\nZOMBIES";
            if (Random.Range(0, 500) == 0)
            {
                int random = Random.Range(0, 7);
                switch (random)
                {
                    case 0: OptionDescriptionText.text = "You are my sunshine,\nMy only sunshine"; break;
                    case 1: OptionDescriptionText.text = "I missed you"; break;
                    case 2: OptionDescriptionText.text = "Play with me"; break;
                    case 3: OptionDescriptionText.text = "It's just a game, mostly"; break;
                    case 4: OptionDescriptionText.text = "I have granted kids to hell"; break;
                    case 5: OptionDescriptionText.text = "It's only partially your fault"; break;
                }
            }
        }

        public void RestoreDefaultSettings()
        {
            HardMode = false;
            LimitedAmmo = false;
            UseCustomEnemies = false;
            ItemSpawnerEnabled = false;
            SpecialRoundDisabled = false;
            WeakerEnemiesEnabled = false;

            OptionDescriptionText.text = "Default settings restored.";
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void DifficultyNormalClicked()
        {
            HardMode = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Normal mode: Normal Enemy speed, HP and numbers.";
        }

        public void DifficultyHardClicked()
        {
            HardMode = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Hard mode: Increased Enemy speed, HP and numbers.";
        }

        public void EnableCustomEnemiesClicked()
        {
            UseCustomEnemies = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Custom humanoid enemies\n(Instead of Sosigs).";
        }

        public void DisableCustomEnemiesClicked()
        {
            UseCustomEnemies = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Default Sosig zombies.";
        }

        public void EnableLimitedAmmoClicked()
        {
            LimitedAmmo = true;

            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Cannot use Spawnlock. Restore ammunition from Max Ammo power ups or buy it from a wall.";
        }

        public void DisableLimitedAmmoClicked()
        {
            LimitedAmmo = false;

            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Spawnlock enabled.";
        }

        public void EnableSpecialRoundClicked()
        {
            SpecialRoundDisabled = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Special round inspired by Hellhounds, occurs every 8 rounds.";
        }

        public void DisableSpecialRoundClicked()
        {
            SpecialRoundDisabled = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Special round disabled.";
        }

        public void EnableItemSpawnerClicked()
        {
            ItemSpawnerEnabled = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Item Spawner will appear at the starting location. Scoring will be disabled for that game.";
        }

        public void DisableItemSpawnerClicked()
        {
            ItemSpawnerEnabled = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "No Item Spawner. Scoring enabled";
        }

        public void EnableWeakerEnemiesClicked()
        {
            WeakerEnemiesEnabled = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Enemies have reduced HP";
        }

        public void DisableWeakerEnemiesClicked()
        {
            WeakerEnemiesEnabled = false;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();

            OptionDescriptionText.text = "Enemies have normal HP";
        }

        public void ToggleBackgroundMusic()
        {
            BackgroundMusic = !BackgroundMusic;

            if (OnMusicSettingChanged != null)
                OnMusicSettingChanged.Invoke();

            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void ChangeLootPool(LootPoolChoice newLootPool)
        {
            foreach (var lootPool in LootPoolChoices)
            {
                lootPool.IsEnabled = false;
            }

            newLootPool.IsEnabled = true;
            CurrentLootPool = newLootPool.LootPool;

            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }
    }
}
#endif