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
        [Header("LootPool asset defines which weapon can be purchased")]

        public int ID;

        private const int AMMO_SPAWNER_ID = 1;
        public bool ExistsInLimitedAmmo = true;
        public bool ExistsInSpawnlock = true;

        [HideInInspector] public int Cost;

        public int PurchaseCost { get { return Cost; } }
        [SerializeField] private bool _isOneTimeOnly;
        public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

        private bool _alreadyBought;
        public bool AlreadyBought { get { return _alreadyBought; } }

        public Text NameText;
        public Text CostText;

        public List<ObjectSpawnPoint> ItemSpawners;

        [HideInInspector] public WeaponData Weapon;

        public bool SameRebuy = false;

        [HideInInspector] public bool IsFree;

        private void Awake()
        {
            RoundManager.OnGameStarted += OnRoundStarted;
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= OnRoundStarted;
        }

        private void LoadWeapon()
        {
            if (ID >= GameSettings.Instance.CurrentLootPool.WallShopsPool.Count)
            {
                gameObject.SetActive(false);
                return;
            }

            Weapon = GameSettings.Instance.CurrentLootPool.WallShopsPool[ID];
            Cost = Weapon.Price;

            NameText.text = Weapon.DisplayName;
            CostText.text = Cost.ToString();
        }

        private void OnRoundStarted()
        {
            if (GameSettings.LimitedAmmo && !ExistsInLimitedAmmo)
                gameObject.SetActive(false);

            if (!GameSettings.LimitedAmmo && !ExistsInSpawnlock)
                gameObject.SetActive(false);

            LoadWeapon();
        }

        public void TryBuying()
        {
            if (IsFree || GameManager.Instance.TryRemovePoints(Cost))
            {
                IsFree = false;
                if (GameSettings.LimitedAmmo && !SameRebuy)
                {
                    // Spawning weapons
                    if (!_alreadyBought)
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

                if (!_alreadyBought)
                    _alreadyBought = true;
                AudioManager.Instance.Play(AudioManager.Instance.BuySound, .5f);
            }
        }

        private IEnumerator DelayedAmmoSpawn()
        {
            for (int i = 0; i < Weapon.LimitedAmmoMagazineCount; i++)
            {
                ItemSpawners[AMMO_SPAWNER_ID].Spawn();
                yield return new WaitForSeconds(0.15f);
            }
        }
    }
}
#endif