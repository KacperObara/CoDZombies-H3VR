#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class MysteryBox : MonoBehaviour, IPurchasable
    {
        public static Action<WeaponData> WeaponSpawnedEvent;

        public int Cost = 950;
        public int PurchaseCost { get { return Cost; } }

        public List<WeaponData> LootId;
        public List<WeaponData> LimitedAmmoLootId;

        public ObjectSpawnPoint WeaponSpawner;
        public ObjectSpawnPoint AmmoSpawner;

        public AudioClip RollSound;

        [HideInInspector] public bool InUse = false;

        private MysteryBoxMover _mysteryBoxMover;

        private void Awake()
        {
            _mysteryBoxMover = GetComponent<MysteryBoxMover>();
        }

        public void SpawnWeapon()
        {
            if (InUse)
                return;

            if (!GameManager.Instance.TryRemovePoints(Cost))
                return;

            InUse = true;
            AudioManager.Instance.Play(RollSound, .25f);

            StartCoroutine(DelayedSpawn());
        }

        private IEnumerator DelayedSpawn()
        {
            yield return new WaitForSeconds(5.5f);

            if (_mysteryBoxMover.TryTeleport())
            {
                _mysteryBoxMover.StartTeleportAnim();
                GameManager.Instance.AddPoints(Cost);
            }
            else
            {
                if (GameSettings.LimitedAmmo)
                {
                    int random = Random.Range(0, LimitedAmmoLootId.Count);
                    WeaponData rolledWeapon = LimitedAmmoLootId[random];

                    WeaponSpawner.ObjectId = rolledWeapon.DefaultSpawners[0];
                    WeaponSpawner.Spawn();

                    AmmoSpawner.ObjectId = rolledWeapon.DefaultSpawners[1];
                    for (int i = 0; i < rolledWeapon.LimitedAmmoMagazineCount; i++)
                    {
                        AmmoSpawner.Spawn();
                    }

                    if (WeaponSpawnedEvent != null)
                        WeaponSpawnedEvent.Invoke(rolledWeapon);
                }
                else
                {
                    int random = Random.Range(0, LootId.Count);
                    WeaponData rolledWeapon = LootId[random];

                    WeaponSpawner.ObjectId = rolledWeapon.DefaultSpawners[0];
                    AmmoSpawner.ObjectId = rolledWeapon.DefaultSpawners[1];

                    WeaponSpawner.Spawn();
                    AmmoSpawner.Spawn();

                    if (WeaponSpawnedEvent != null)
                        WeaponSpawnedEvent.Invoke(rolledWeapon);
                }

                InUse = false;

                _mysteryBoxMover.CurrentRoll++;
            }
        }
    }
}
#endif