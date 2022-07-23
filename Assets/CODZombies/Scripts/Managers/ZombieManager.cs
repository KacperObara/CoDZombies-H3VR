#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Zombie;
using FistVR;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CustomScripts.Managers
{
    //TODO Need to refactor AI classes, it became quite a monster from having to support 4 types of enemies in 2 modes
    public class ZombieManager : MonoBehaviourSingleton<ZombieManager>
    {
        public static Action LocationChangedEvent;

        public Location CurrentLocation;

        public ZombiePool NormalZombiePool;
        public ZombiePool SpecialZombiePool;

        public AnimationCurve ZombieCountCurve;
        public AnimationCurve CustomZombieHPCurve;
        public AnimationCurve ZosigHPCurve;
        public AnimationCurve ZosigLinkIntegrityCurve;
        public AnimationCurve ZosigPerRoundSpeed;

        public int CustomZombieDamage = 2000;
        public int PointsOnHit = 10;
        public int PointsOnKill = 100;

        public List<ZombieController> AllCustomZombies;
        [HideInInspector] public List<ZombieController> ExistingZombies;

        private Transform _zombieTarget;

        public ParticleSystem HellhoundExplosionPS;

        public int ZombieAtOnceLimit = 20;
        [HideInInspector] public int ZombiesRemaining;

        private int ZombiesToSpawnThisRound
        {
            get
            {
                if (GameSettings.HardMode)
                    return Mathf.CeilToInt(ZombieCountCurve.Evaluate(RoundManager.Instance.RoundNumber) + 1);
                else
                    return Mathf.CeilToInt(ZombieCountCurve.Evaluate(RoundManager.Instance.RoundNumber));
            }
        }

        private Coroutine _spawningCoroutine;


#region Spawning

        public void BeginSpawningEnemies()
        {
            ZombiesRemaining = ZombiesToSpawnThisRound;

            if (RoundManager.Instance.IsRoundSpecial)
            {
                StartSpawningZombies(6f);

                AudioManager.Instance.Play(AudioManager.Instance.HellHoundRoundStartSound, 0.35f, 0f);
            }
            else
            {
                StartSpawningZombies(2f);

                AudioManager.Instance.Play(AudioManager.Instance.RoundStartSound, 0.2f, 1f);
            }


        }

        public void StartSpawningZombies(float initialDelay)
        {
            _spawningCoroutine = StartCoroutine(DelayedZombieSpawn(initialDelay));
        }

        public void OnZombieSpawned(ZombieController controller)
        {
            StartCoroutine(DelayedCustomZombieSpawn(controller));
        }

        private IEnumerator DelayedZombieSpawn(float delay)
        {
            yield return new WaitForSeconds(delay);

            while (ZombiesRemaining > ExistingZombies.Count)
            {
                if (ExistingZombies.Count >= ZombieAtOnceLimit)
                {
                    yield return new WaitForSeconds(5);
                    continue;
                }

                _zombieTarget = GameReferences.Instance.Player;

                if (!GameSettings.UseCustomEnemies)
                {
                    SpawnZosig();
                }
                else
                {
                    if (RoundManager.Instance.IsRoundSpecial)
                        SpecialZombiePool.Spawn();
                    else
                        NormalZombiePool.Spawn();
                }

                yield return new WaitForSeconds(2);
            }
        }

        private IEnumerator DelayedCustomZombieSpawn(ZombieController controller)
        {
            yield return null;

            Transform spawnPoint = null;
            if (RoundManager.Instance.IsRoundSpecial)
            {
                spawnPoint = CurrentLocation.SpecialZombieSpawnPoints[Random.Range(0, CurrentLocation.SpecialZombieSpawnPoints.Count)].transform;
            }
            else
            {
                spawnPoint = CurrentLocation.ZombieSpawnPoints[Random.Range(0, CurrentLocation.ZombieSpawnPoints.Count)];
            }

            if (spawnPoint.GetComponent<ZombieSpawner>() != null)
            {
                Window targetWindow = spawnPoint.GetComponent<ZombieSpawner>().WindowWaypoint;
                if (targetWindow != null)
                    _zombieTarget = targetWindow.ZombieWaypoint;
            }

            ExistingZombies.Add(controller);

            if (RoundManager.Instance.IsRoundSpecial)
            {
                spawnPoint.GetComponent<CustomSosigSpawnPoint>().SpawnPS.Play(true);

                if (RoundManager.Instance.IsRoundSpecial)
                    AudioManager.Instance.Play(AudioManager.Instance.HellHoundSpawnSound, volume:.4f, delay:.25f);

                yield return new WaitForSeconds(2f);
            }

            if (ExistingZombies.Contains(controller))
            {
                controller.transform.position = spawnPoint.position;
                controller.Initialize(_zombieTarget);

                if (RoundManager.Instance.IsRoundSpecial)
                    controller.InitializeSpecialType();
            }
        }

        public void SpawnZosig()
        {
            if (RoundManager.Instance.IsRoundSpecial)
            {
                CustomSosigSpawnPoint spawner =
                    CurrentLocation.SpecialZombieSpawnPoints[Random.Range(0, CurrentLocation.SpecialZombieSpawnPoints.Count)].GetComponent<CustomSosigSpawnPoint>();

                spawner.Spawn();
            }
            else
            {
                CustomSosigSpawnPoint spawner =
                    CurrentLocation.ZombieSpawnPoints[Random.Range(0, CurrentLocation.ZombieSpawnPoints.Count)].GetComponent<CustomSosigSpawnPoint>();

                Window targetWindow = spawner.GetComponent<ZombieSpawner>().WindowWaypoint;
                if (targetWindow != null)
                    _zombieTarget = targetWindow.ZombieWaypoint;

                spawner.Spawn();
            }
        }

        public void OnZosigSpawned(Sosig zosig)
        {
            ZombieController controller = zosig.gameObject.AddComponent<ZosigZombieController>();

            controller.Initialize(_zombieTarget);
            ExistingZombies.Add(controller);

            if (RoundManager.Instance.IsRoundSpecial)
            {
                controller.InitializeSpecialType();
            }
        }

        #endregion

        public void ChangeLocation(Location newLocation)
        {
            if (_spawningCoroutine != null)
                StopCoroutine(_spawningCoroutine);

            CurrentLocation = newLocation;

            for (int i = ExistingZombies.Count - 1; i >= 0; i--)
            {
                ExistingZombies[i].OnKill(false);
            }

            StartSpawningZombies(5f);

            if (LocationChangedEvent != null)
                LocationChangedEvent.Invoke();
        }

        public void OnZombieDied(ZombieController controller, bool awardKill = true)
        {
            GameManager.Instance.Kills++;

            if (GameSettings.UseCustomEnemies)
                StartCoroutine(DelayedCustomZombieDespawn(controller.GetComponent<CustomZombieController>()));

            ExistingZombies.Remove(controller);

            if (!awardKill)
                return;

            ZombiesRemaining--;

            if (ZombiesRemaining <= 0)
            {
                if (RoundManager.Instance.IsRoundSpecial && GameSettings.LimitedAmmo)
                    PowerUpManager.Instance.SpawnPowerUp(PowerUpManager.Instance.MaxAmmo, controller.transform.position);

                RoundManager.Instance.EndRound();
            }

            if (RoundManager.OnZombiesLeftChanged != null)
                RoundManager.OnZombiesLeftChanged.Invoke();
            if (RoundManager.OnZombieKilled != null)
                RoundManager.OnZombieKilled.Invoke(controller.gameObject);
        }

        private IEnumerator DelayedCustomZombieDespawn(CustomZombieController controller)
        {
            bool specialEnemy = RoundManager.Instance.IsRoundSpecial;

            if (specialEnemy)
            {
                yield return new WaitForSeconds(0);
                SpecialZombiePool.Despawn(controller);
            }
            else
            {
                yield return new WaitForSeconds(5f);
                NormalZombiePool.Despawn(controller);
            }
        }

        [HarmonyPatch(typeof (Sosig), "SosigDies")]
        [HarmonyPostfix]
        private static void OnSosigDied(Damage.DamageClass damClass, Sosig.SosigDeathType deathType)
        {
            Instance.GetComponent<ZosigZombieController>().OnKill();
        }

        [HarmonyPatch(typeof(Sosig), "ProcessDamage")]
        [HarmonyPrefix]
        private static void BeforeZombieHit(Damage d, SosigLink link)
        {
            if (d.Class == Damage.DamageClass.Melee &&
                d.Source_IFF != GM.CurrentSceneSettings.DefaultPlayerIFF)
            {
                d.Dam_Blinding = 0;
                d.Dam_TotalKinetic = 0;
                d.Dam_TotalEnergetic = 0;
                d.Dam_Blunt = 0;
                d.Dam_Chilling = 0;
                d.Dam_Cutting = 0;
                d.Dam_Thermal = 0;
                d.Dam_EMP = 0;
                d.Dam_Piercing = 0;
                d.Dam_Stunning = 0;
            }
        }

        [HarmonyPatch(typeof(Sosig), "ProcessDamage")]
        [HarmonyPostfix]
        private static void AfterZombieHit(Damage d, SosigLink link)
        {
            Instance.GetComponent<ZosigZombieController>().OnHit(d);
        }
    }
}
#endif