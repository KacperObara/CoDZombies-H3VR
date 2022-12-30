using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Objects.Weapons;
using FistVR;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CODZombies.Scripts.Powerups
{
    public class PackAPunch : MonoBehaviour, IPurchasable, IRequiresPower
    {
        public static Action PurchaseEvent;

        public int Cost;
        public int PurchaseCost { get { return Cost; } }

        [SerializeField] private bool _isOneTimeOnly;
        public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

        private bool _alreadyBought;
        public bool AlreadyBought { get { return _alreadyBought; } }
        public bool IsPowered { get { return GameManager.Instance.PowerEnabled; } }

        public List<WeaponData> WeaponsData;
        public List<CustomItemSpawner> Spawners;

        public AudioClip UpgradeSound;
        [HideInInspector] public bool InUse = false;

        private void Awake()
        {
            RoundManager.OnGameStarted += LoadWeaponPool;
        }

        private void LoadWeaponPool()
        {
            WeaponsData.AddRange(GameSettings.Instance.CurrentLootPool.PackAPunchPool.ToList());
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<FVRPhysicalObject>())
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
            if (fvrPhysicalObject as FVRFireArm == null)
                return;

            WeaponWrapper weaponWrapper = fvrPhysicalObject.GetComponent<WeaponWrapper>();

            // Disabling minigun since it could break the DeathMachine
            if (fvrPhysicalObject.ObjectWrapper.ItemID == "M134Minigun")
                return;

            if (weaponWrapper == null)
                return;
            if (weaponWrapper.PackAPunchDeactivated)
                return;

            WeaponData weapon = WeaponsData.FirstOrDefault(x => x.Id == fvrPhysicalObject.ObjectWrapper.ItemID);

            if (weapon && !weapon.UpgradedWeapon)
                Debug.LogWarning("Weapon recognized by the PaP, but there is no Upgraded weapon specified, applying basic upgrade");

            if (weapon) // Normal behavior with gun changes
            {
                if (IsPowered && GameManager.Instance.TryRemovePoints(Cost) && weapon.UpgradedWeapon != null)
                {
                    if (InUse)
                        return;
                    InUse = true;

                    _alreadyBought = true;

                    fvrPhysicalObject.ForceBreakInteraction();
                    fvrPhysicalObject.IsPickUpLocked = true;
                    Destroy(fvrPhysicalObject.gameObject);

                    StartCoroutine(DelayedSpawn(weapon));

                    AudioManager.Instance.Play(AudioManager.Instance.BuySound, .5f);
                    AudioManager.Instance.Play(UpgradeSound, .3f);

                    weaponWrapper.BlockPackAPunchUpgrade();
                }
            }
            else // Alternative behavior for unforeseen guns and subsequent Re pack a punching
            {
                if (IsPowered && GameManager.Instance.TryRemovePoints(Cost))
                {
                    if (InUse)
                        return;
                    InUse = true;

                    _alreadyBought = true;

                    fvrPhysicalObject.ForceBreakInteraction();

                    StartCoroutine(DelayedReturn(fvrPhysicalObject));

                    AudioManager.Instance.Play(AudioManager.Instance.BuySound, .5f);
                    AudioManager.Instance.Play(UpgradeSound, .3f);

                    weaponWrapper.IncreaseFireRate(1.4f);

                    weaponWrapper.BlockPackAPunchUpgrade();

                    if (fvrPhysicalObject.GetComponent<Rigidbody>())
                        fvrPhysicalObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    weaponWrapper.gameObject.SetActive(false);
                }
            }

            if (PurchaseEvent != null)
                PurchaseEvent.Invoke();
        }

        private IEnumerator DelayedSpawn(WeaponData weapon)
        {
            yield return new WaitForSeconds(5f);

            for (int i = 0; i < weapon.UpgradedWeapon.DefaultSpawners.Count; i++)
            {
                Spawners[i].ObjectId = weapon.UpgradedWeapon.DefaultSpawners[i];
                Spawners[i].Spawn();
            }

            if (GameSettings.LimitedAmmo)
            {
                for (int i = 0; i < weapon.UpgradedWeapon.LimitedAmmoMagazineCount - 1; i++)
                {
                    Spawners[1].ObjectId = weapon.UpgradedWeapon.DefaultSpawners[1];
                    Spawners[1].Spawn();
                }
            }

            InUse = false;
        }

        private IEnumerator DelayedReturn(FVRPhysicalObject weapon)
        {
            yield return new WaitForSeconds(5f);

            weapon.transform.position = Spawners[0].transform.position;
            weapon.transform.rotation = Quaternion.identity;
            weapon.gameObject.SetActive(true);

            FVRFireArm fireArm = weapon as FVRFireArm;
            fireArm.GetComponent<WeaponWrapper>().OnPackAPunched();

            List<FVRObject> compatibleRounds = IM.OD[fireArm.ObjectWrapper.ItemID].CompatibleSingleRounds;
            List<FVRObject> compatibleMags = IM.OD[fireArm.ObjectWrapper.ItemID].CompatibleMagazines;
            List<FVRObject> compatibleClips = IM.OD[fireArm.ObjectWrapper.ItemID].CompatibleClips;
            List<FVRObject> compatibleSpeedLoaders = IM.OD[fireArm.ObjectWrapper.ItemID].CompatibleSpeedLoaders;

            // Randomizing ammo
            FVRFireArmRound randomRound = null;
            if (compatibleRounds.Count > 0)
            {
                int random = Random.Range(0, compatibleRounds.Count);
                randomRound = compatibleRounds[random].GetGameObject().GetComponent<FVRFireArmRound>();
            }

            int ammoContainersToSpawn = 1;
            if (compatibleMags.Count > 0)
            {
                // Spawning new magazine
                FVRObject newMagazine = compatibleMags
                    .OrderByDescending(x => x.MagazineCapacity)
                    .FirstOrDefault();

                if (GameSettings.LimitedAmmo)
                    ammoContainersToSpawn = 2;

                for (int i = 0; i < ammoContainersToSpawn; i++)
                {
                    GameObject magObject = Instantiate(newMagazine.GetGameObject(), Spawners[1].transform.position,
                        Quaternion.identity);

                    //magObject.AddComponent<MagazineWrapper>().RoundClass = randomRound.RoundClass;
                    FVRFireArmMagazine magazine = magObject.GetComponent<FVRFireArmMagazine>();
                    magObject.AddComponent<MagazineWrapper>().InitialzieWithAmmo(magazine, randomRound.RoundClass);

                    magazine.ReloadMagWithType(randomRound.RoundClass);
                }
            }
            else if (compatibleClips.Count > 0)
            {
                // not sure if clips use magazine capacity, or if it does matter,
                // are there even more than 1 available clips per gun anyway?
                FVRObject newClip = compatibleClips
                    .OrderByDescending(x => x.MagazineCapacity)
                    .FirstOrDefault();

                if (GameSettings.LimitedAmmo)
                    ammoContainersToSpawn = 4;

                for (int i = 0; i < ammoContainersToSpawn; i++)
                {
                    GameObject clipObject = Instantiate(newClip.GetGameObject(), Spawners[1].transform.position,
                        Quaternion.identity);

                    FVRFireArmClip clip = clipObject.GetComponent<FVRFireArmClip>();
                    clipObject.AddComponent<MagazineWrapper>().InitialzieWithAmmo(clip, randomRound.RoundClass);

                    clip.ReloadClipWithType(randomRound.RoundClass);
                }
            }
            else if (compatibleSpeedLoaders.Count > 0)
            {
                FVRObject newSpeedLoader = compatibleSpeedLoaders
                    .OrderByDescending(x => x.MagazineCapacity)
                    .FirstOrDefault();

                if (GameSettings.LimitedAmmo)
                    ammoContainersToSpawn = 4;

                for (int i = 0; i < ammoContainersToSpawn; i++)
                {
                    GameObject speedLoaderObject = Instantiate(newSpeedLoader.GetGameObject(),
                        Spawners[1].transform.position,
                        Quaternion.identity);


                    Speedloader speedLoader = speedLoaderObject.GetComponent<Speedloader>();
                    speedLoaderObject.AddComponent<MagazineWrapper>().InitialzieWithAmmo(speedLoader, randomRound.RoundClass);

                    speedLoader.ReloadClipWithType(randomRound.RoundClass);
                }
            }

            // reloading existing magazine
            FVRFireArmMagazine loadedMag = fireArm.Magazine;
            if (loadedMag && randomRound != null)
            {
                if (!loadedMag.GetComponent<MagazineWrapper>())
                    loadedMag.gameObject.AddComponent<MagazineWrapper>().InitialzieWithAmmo(loadedMag, randomRound.RoundClass);

                loadedMag.ReloadMagWithType(randomRound.RoundClass);
            }

            FVRFireArmClip loadedClip = fireArm.Clip;
            if (loadedClip && randomRound != null)
            {
                if (!loadedClip.GetComponent<MagazineWrapper>())
                    loadedClip.gameObject.AddComponent<MagazineWrapper>().InitialzieWithAmmo(loadedClip, randomRound.RoundClass);

                loadedClip.ReloadClipWithType(randomRound.RoundClass);
            }

            InUse = false;
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= LoadWeaponPool;
        }
    }
}