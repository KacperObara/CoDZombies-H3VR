#if H3VR_IMPORTED
using FistVR;

namespace CustomScripts
{
    public class GameSettings : MonoBehaviourSingleton<GameSettings>
    {
        public static bool MoreEnemies;
        public static bool FasterEnemies;
        public static bool WeakerEnemies;
        public static bool BackgroundMusic;

        public static bool UseCustomEnemies;

        public static bool LimitedAmmo;

        public static bool ItemSpawnerSpawned;

        private void Start()
        {
            MoreEnemies = false;
            FasterEnemies = false;
            WeakerEnemies = false;
            LimitedAmmo = false;
            BackgroundMusic = false;
            UseCustomEnemies = false;
            ItemSpawnerSpawned = false;
        }

        public static event Delegates.VoidDelegate OnSettingsChanged;

        public void ToggleMoreEnemies()
        {
            MoreEnemies = !MoreEnemies;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void ToggleFasterEnemies()
        {
            FasterEnemies = !FasterEnemies;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void ToggleWeakerEnemies()
        {
            WeakerEnemies = !WeakerEnemies;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void ToggleBackgroundMusic()
        {
            BackgroundMusic = !BackgroundMusic;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void ToggleUseZosigs()
        {
            UseCustomEnemies = !UseCustomEnemies;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void ToggleLimitedAmmo()
        {
            LimitedAmmo = !LimitedAmmo;
            GM.CurrentSceneSettings.IsSpawnLockingEnabled = !LimitedAmmo;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }

        public void SpawnSpawner()
        {
            ItemSpawnerSpawned = true;
            if (OnSettingsChanged != null)
                OnSettingsChanged.Invoke();
        }
    }
}
#endif