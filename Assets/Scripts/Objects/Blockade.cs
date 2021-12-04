using System;
using System.Collections.Generic;
using System.Linq;
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

        private bool alreadyUsed = false;

        private void OnValidate()
        {
            foreach (Text text in CostTexts)
            {
                text.text = Cost.ToString();
            }
        }

        public void Buy()
        {
            if (alreadyUsed)
                return;

            if (!GameManager.Instance.TryRemovePoints(Cost))
                return;

            alreadyUsed = true;

            foreach (Transform zombieSP in UnlockableZombieSpawnPoints)
            {
                if (!ZombieManager.Instance.ZombieSpawnPoints.Contains(zombieSP))
                    ZombieManager.Instance.ZombieSpawnPoints.Add(zombieSP);
            }

            foreach (CustomSosigSpawner zosigSP in UnlockableZosigSpawnPoints)
            {
                if (!ZombieManager.Instance.ZosigsSpawnPoints.Contains(zosigSP))
                    ZombieManager.Instance.ZosigsSpawnPoints.Add(zosigSP);
            }

            AudioManager.Instance.BuySound.Play();
            gameObject.SetActive(false);
        }
    }
}