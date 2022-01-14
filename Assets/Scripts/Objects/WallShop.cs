#if H3VR_IMPORTED
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;
using UnityEngine.UI;


namespace CustomScripts.Objects
{
    public class WallShop : MonoBehaviour, IPurchasable
    {
        private const int AMMO_SPAWNER_ID = 1;
        public bool ExistsInLimitedAmmo = true;
        public bool ExistsInSpawnlock = true;

        public string Name;
        public int Cost;

        public int PurchaseCost { get { return Cost; } }

        public Text NameText;
        public Text CostText;

        public List<ObjectSpawnPoint> ItemSpawners;

        public WeaponData Weapon;

        public bool SameRebuy = false;

        [HideInInspector] public bool IsFree;

        private bool _alreadyBoughtOnce;

        private void Awake()
        {
            RoundManager.OnGameStarted -= OnRoundStarted;
            RoundManager.OnGameStarted += OnRoundStarted;
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= OnRoundStarted;
        }

        private void OnValidate()
        {
            NameText.text = Name;
            CostText.text = Cost.ToString();
        }

        private void OnRoundStarted()
        {
            if (GameSettings.LimitedAmmo && !ExistsInLimitedAmmo)
                gameObject.SetActive(false);

            if (!GameSettings.LimitedAmmo && !ExistsInSpawnlock)
                gameObject.SetActive(false);
        }

        public void TryBuying()
        {
            if (IsFree || GameManager.Instance.TryRemovePoints(Cost))
            {
                IsFree = false;
                if (GameSettings.LimitedAmmo && !SameRebuy)
                {
                    // Spawning weapons
                    if (!_alreadyBoughtOnce)
                    {
                        for (int i = 0; i < Weapon.DefaultSpawners.Count; i++)
                        {
                            if (i == AMMO_SPAWNER_ID)
                                continue;

                            ItemSpawners[i].ObjectId = Weapon.DefaultSpawners[i];
                            ItemSpawners[i].Spawn();
                        }
                    }

                    if (SameRebuy)
                    {
                        // spawning weapons in unlimited
                        for (int i = 0; i < Weapon.DefaultSpawners.Count; i++)
                        {
                            ItemSpawners[i].ObjectId = Weapon.DefaultSpawners[i];
                            ItemSpawners[i].Spawn();
                        }
                    }
                    else
                    {
                        // Spawning ammo
                        ItemSpawners[AMMO_SPAWNER_ID].ObjectId = Weapon.DefaultSpawners[AMMO_SPAWNER_ID];
                        StartCoroutine(DelayedAmmoSpawn());
                        for (int i = 0; i < Weapon.LimitedAmmoMagazineCount; i++)
                        {
                            ItemSpawners[AMMO_SPAWNER_ID].Spawn();
                        }
                    }

                    // Updating text
                    Cost = Weapon.AmmoBuyCost;
                    CostText.text = Weapon.AmmoBuyCost.ToString();
                }
                else
                {
                    // spawning weapons in unlimited
                    for (int i = 0; i < Weapon.DefaultSpawners.Count; i++)
                    {
                        ItemSpawners[i].ObjectId = Weapon.DefaultSpawners[i];
                        ItemSpawners[i].Spawn();
                    }
                }

                if (!_alreadyBoughtOnce)
                    _alreadyBoughtOnce = true;
                AudioManager.Instance.BuySound.Play();
            }
        }

        private IEnumerator DelayedAmmoSpawn()
        {
            for (int i = 0; i < Weapon.LimitedAmmoMagazineCount; i++)
            {
                ItemSpawners[AMMO_SPAWNER_ID].Spawn();
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}
#endif