#if H3VR_IMPORTED

using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
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
    public class Blockade : MonoBehaviour, IPurchasable
    {
        public List<Transform> UnlockableZombieSpawnPoints;
        public List<CustomSosigSpawnPoint> UnlockableZosigSpawnPoints;

        public List<Text> CostTexts;

        public int Cost;
        public int PurchaseCost { get { return Cost; } }

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
                if (!ZombieManager.Instance.CustomZombieSpawnPoints.Contains(zombieSp))
                    ZombieManager.Instance.CustomZombieSpawnPoints.Add(zombieSp);
            }

            foreach (CustomSosigSpawnPoint zosigSp in UnlockableZosigSpawnPoints)
            {
                if (!ZombieManager.Instance.ZosigSpawnPoints.Contains(zosigSp))
                    ZombieManager.Instance.ZosigSpawnPoints.Add(zosigSp);
            }

            AudioManager.Instance.BuySound.Play();
            gameObject.SetActive(false);
        }
    }
}
#endif