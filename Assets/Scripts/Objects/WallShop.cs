using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Objects.Weapons;
using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts.Objects
{
    public class WallShop : MonoBehaviour
    {
        public bool ExistsInLimitedAmmo = true;
        public bool ExistsInSpawnlock = true;

        public string Name;
        public int Cost;

        public Text NameText;
        public Text CostText;

        public List<ItemSpawner> ItemSpawners;

        public WeaponData Weapon;

        private const int ammoSpawnerId = 1;

        private bool alreadyBoughtOnce;

        public bool SameRebuy = false;

        [HideInInspector] public bool IsFree;

        private void OnValidate()
        {
            NameText.text = Name;
            CostText.text = Cost.ToString();
        }

        private void Awake()
        {
            RoundManager.OnGameStarted -= OnRoundStarted;
            RoundManager.OnGameStarted += OnRoundStarted;
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
            if (GameManager.Instance.TryRemovePoints(Cost) || IsFree)
            {
                IsFree = false;
                if (GameSettings.LimitedAmmo && !SameRebuy)
                {
                    // Spawning weapons
                    if (!alreadyBoughtOnce)
                    {
                        for (int i = 0; i < Weapon.DefaultSpawners.Count; i++)
                        {
                            if (i == ammoSpawnerId)
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
                        ItemSpawners[ammoSpawnerId].ObjectId = Weapon.DefaultSpawners[ammoSpawnerId];
                        StartCoroutine(DelayedAmmoSpawn());
                        // for (int i = 0; i < Weapon.LimitedAmmoMagazineCount; i++)
                        // {
                        //     ItemSpawners[ammoSpawnerId].Spawn();
                        // }
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

                if (!alreadyBoughtOnce)
                    alreadyBoughtOnce = true;
                AudioManager.Instance.BuySound.Play();
            }
        }

        private IEnumerator DelayedAmmoSpawn()
        {
            for (int i = 0; i < Weapon.LimitedAmmoMagazineCount; i++)
            {
                ItemSpawners[ammoSpawnerId].Spawn();
                yield return new WaitForSeconds(0.05f);
            }
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= OnRoundStarted;
        }
    }
}