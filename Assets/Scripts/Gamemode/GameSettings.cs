using FistVR;
namespace CustomScripts
{
    public class GameSettings : MonoBehaviourSingleton<GameSettings>
    {
        public static bool MoreEnemies;
        public static bool FasterEnemies;
        public static bool WeakerEnemies;
        public static bool BackgroundMusic;

        public static bool UseZosigs;

        public static bool LimitedAmmo;

        public static bool ItemSpawnerSpawned;

        private void Start()
        {
            MoreEnemies = false;
            FasterEnemies = false;
            WeakerEnemies = false;
            LimitedAmmo = false;
            BackgroundMusic = false;
            UseZosigs = false;
            ItemSpawnerSpawned = false;
        }
        public static event Delegates.VoidDelegate OnSettingsChanged;

        public void ToggleMoreEnemies()
        {
            MoreEnemies = !MoreEnemies;
            OnSettingsChanged.Invoke();
        }

        public void ToggleFasterEnemies()
        {
            FasterEnemies = !FasterEnemies;
            OnSettingsChanged.Invoke();
        }

        public void ToggleWeakerEnemies()
        {
            WeakerEnemies = !WeakerEnemies;
            OnSettingsChanged.Invoke();
        }

        public void ToggleBackgroundMusic()
        {
            BackgroundMusic = !BackgroundMusic;
            OnSettingsChanged.Invoke();
        }

        public void ToggleUseZosigs()
        {
            UseZosigs = !UseZosigs;
            OnSettingsChanged.Invoke();
        }

        public void ToggleLimitedAmmo()
        {
            LimitedAmmo = !LimitedAmmo;
            GM.CurrentSceneSettings.IsSpawnLockingEnabled = !LimitedAmmo;
            OnSettingsChanged.Invoke();
        }

        public void SpawnSpawner()
        {
            ItemSpawnerSpawned = true;
            OnSettingsChanged.Invoke();
        }
    }
}