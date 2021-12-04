using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts.Objects.Weapons;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups
{
    public class PackAPunch : MonoBehaviour
    {
        public int Cost;

        public List<WeaponData> WeaponsData;
        public List<ItemSpawner> Spawners;

        [HideInInspector] public bool InUse = false;

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<FVRPhysicalObject>()) // This is not actually expensive, since it's rarely called
            {
                FVRPhysicalObject fvrPhysicalObject = other.GetComponent<FVRPhysicalObject>();

                if (fvrPhysicalObject.ObjectWrapper != null)
                {
                    TryBuying(fvrPhysicalObject);
                }
            }
        }

        public void TryBuying(FVRPhysicalObject fvrPhysicalObject)
        {
            WeaponData weapon = WeaponsData.FirstOrDefault(x => x.Id == fvrPhysicalObject.ObjectWrapper.ItemID);
            if (!weapon)
                return;

            if (GameManager.Instance.TryRemovePoints(Cost))
            {
                if (InUse)
                    return;
                InUse = true;

                fvrPhysicalObject.ForceBreakInteraction();
                fvrPhysicalObject.IsPickUpLocked = true;
                Destroy(fvrPhysicalObject.gameObject);

                StartCoroutine(DelayedSpawn(weapon));

                AudioManager.Instance.BuySound.Play();
                AudioManager.Instance.PackAPunchUpgradeSound.Play();
            }
        }

        private IEnumerator DelayedSpawn(WeaponData weapon)
        {
            yield return new WaitForSeconds(5f);

            for (int i = 0; i < weapon.UpgradedWeapon.DefaultSpawners.Count; i++)
            {
                //TODO: New itemspawner doesnt have ObjectId field
                // Spawners[i].ObjectId = weapon.UpgradedWeapon.DefaultSpawners[i];
                Spawners[i].SpawnItem();
            }

            if (GameSettings.LimitedAmmo)
            {
                for (int i = 0; i < weapon.UpgradedWeapon.LimitedAmmoMagazineCount - 1; i++)
                {
                    //TODO: New itemspawner doesnt have ObjectId field
                    // Spawners[1].ObjectId = weapon.UpgradedWeapon.DefaultSpawners[1];
                    Spawners[1].SpawnItem();
                }
            }

            InUse = false;
        }
    }
}