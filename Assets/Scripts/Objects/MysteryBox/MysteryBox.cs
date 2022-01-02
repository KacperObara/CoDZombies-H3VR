#if H3VR_IMPORTED
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;
namespace CustomScripts
{
    public class MysteryBox : MonoBehaviour
    {
        public int Cost = 950;

        public List<WeaponData> LootId;
        public List<WeaponData> LimitedAmmoLootId;

        public ObjectSpawnPoint WeaponSpawner;
        public ObjectSpawnPoint AmmoSpawner;

        public AudioSource SpawnAudio;

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

            if (!GameManager.Instance.TryRemovePoints(950))
                return;

            InUse = true;
            SpawnAudio.Play();

            StartCoroutine(DelayedSpawn());
        }

        private IEnumerator DelayedSpawn()
        {
            yield return new WaitForSeconds(5.5f);

            if (_mysteryBoxMover.TryTeleport())
            {
                _mysteryBoxMover.StartTeleportAnim();
                GameManager.Instance.AddPoints(950);
            }
            else
            {
                if (GameSettings.LimitedAmmo)
                {
                    int random = Random.Range(0, LimitedAmmoLootId.Count);

                    WeaponSpawner.ObjectId = LimitedAmmoLootId[random].DefaultSpawners[0];
                    WeaponSpawner.Spawn();

                    AmmoSpawner.ObjectId = LimitedAmmoLootId[random].DefaultSpawners[1];
                    for (int i = 0; i < LimitedAmmoLootId[random].LimitedAmmoMagazineCount; i++)
                    {
                        AmmoSpawner.Spawn();
                    }
                }
                else
                {
                    int random = Random.Range(0, LootId.Count);

                    WeaponSpawner.ObjectId = LootId[random].DefaultSpawners[0];
                    AmmoSpawner.ObjectId = LootId[random].DefaultSpawners[1];

                    WeaponSpawner.Spawn();
                    AmmoSpawner.Spawn();
                }

                InUse = false;

                _mysteryBoxMover.CurrentRoll++;
            }
        }
    }
}
#endif