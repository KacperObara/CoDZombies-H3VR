#if H3VR_IMPORTED

using System;
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
        public Action OnPurchase;

        public List<Transform> UnlockableZombieSpawnPoints;
        public List<Transform> UnlockableSpecialZombieSpawnPoints;

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


            foreach (Transform spawnPoint in UnlockableZombieSpawnPoints)
            {
                if (!ZombieManager.Instance.CurrentLocation.ZombieSpawnPoints.Contains(spawnPoint))
                    ZombieManager.Instance.CurrentLocation.ZombieSpawnPoints.Add(spawnPoint);
            }

            foreach (Transform spawnPoint in UnlockableSpecialZombieSpawnPoints)
            {
                if (!ZombieManager.Instance.CurrentLocation.SpecialZombieSpawnPoints.Contains(spawnPoint))
                    ZombieManager.Instance.CurrentLocation.SpecialZombieSpawnPoints.Add(spawnPoint);
            }

            AudioManager.Instance.Play(AudioManager.Instance.BuySound, .5f);
            gameObject.SetActive(false);

            if (OnPurchase != null)
                OnPurchase.Invoke();
        }
    }
}
#endif