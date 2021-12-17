using System.Collections;
using System.Collections.Generic;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;
using UnityEngine.UI;
namespace CustomScripts.Objects
{
    public class WallShop : MonoBehaviour
    {
        private const int AMMO_SPAWNER_ID = 1;
        public bool ExistsInLimitedAmmo = true;
        public bool ExistsInSpawnlock = true;

        public string Name;
        public int Cost;

        public Text NameText;
        public Text CostText;

        public List<ItemSpawner> ItemSpawners;

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
            if (GameManager.Instance.TryRemovePoints(Cost) || IsFree)
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
                            //TODO: New itemspawner doesnt have ObjectId field
                            // ItemSpawners[i].ObjectId = Weapon.DefaultSpawners[i];
                            ItemSpawners[i].SpawnItem();
                        }
                    }

                    if (SameRebuy)
                    {
                        // spawning weapons in unlimited
                        for (int i = 0; i < Weapon.DefaultSpawners.Count; i++)
                        {
                            //TODO: See above
                            // ItemSpawners[i].ObjectId = Weapon.DefaultSpawners[i];
                            ItemSpawners[i].SpawnItem();
                        }
                    }
                    else
                    {
                        //TODO: See above
                        // Spawning ammo
                        // ItemSpawners[ammoSpawnerId].ObjectId = Weapon.DefaultSpawners[ammoSpawnerId];
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
                        //TODO: See above
                        // ItemSpawners[i].ObjectId = Weapon.DefaultSpawners[i];
                        ItemSpawners[i].SpawnItem();
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
                ItemSpawners[AMMO_SPAWNER_ID].SpawnItem();
                yield return new WaitForSeconds(0.05f);
            }
        }
    }
}