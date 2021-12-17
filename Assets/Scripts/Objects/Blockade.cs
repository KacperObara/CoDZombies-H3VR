using System.Collections.Generic;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Managers;
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts
{
    /// <summary>
    /// Blocks both player and zombies, when it's removed, it checks and enables unlockable spawn points for zombies.
    /// Because there is no reason to spawn zombies on the blocked side of the map
    /// </summary>
    public class Blockade : MonoBehaviour
    {
        public List<Transform> UnlockableZombieSpawnPoints;
        public List<CustomSosigSpawner> UnlockableZosigSpawnPoints;

        public List<Text> CostTexts;

        public int Cost;

        private bool _alreadyUsed;

        private void OnValidate()
        {
            foreach (Text text in CostTexts)
            {
                text.text = Cost.ToString();
            }
        }

        public void Buy()
        {
            if (_alreadyUsed)
                return;

            if (!GameManager.Instance.TryRemovePoints(Cost))
                return;

            _alreadyUsed = true;

            foreach (Transform zombieSp in UnlockableZombieSpawnPoints)
            {
                if (!ZombieManager.Instance.ZombieSpawnPoints.Contains(zombieSp))
                    ZombieManager.Instance.ZombieSpawnPoints.Add(zombieSp);
            }

            foreach (CustomSosigSpawner zosigSp in UnlockableZosigSpawnPoints)
            {
                if (!ZombieManager.Instance.ZosigsSpawnPoints.Contains(zosigSp))
                    ZombieManager.Instance.ZosigsSpawnPoints.Add(zosigSp);
            }

            AudioManager.Instance.BuySound.Play();
            gameObject.SetActive(false);
        }
    }
}