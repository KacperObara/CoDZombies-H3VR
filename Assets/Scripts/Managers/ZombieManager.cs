#if H3VR_IMPORTED
using System.Collections;
using System.Collections.Generic;
using Atlas.MappingComponents.Sandbox;
using CustomScripts.Gamemode.GMDebug;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

namespace CustomScripts.Managers
{
    public class ZombieManager : MonoBehaviourSingleton<ZombieManager>
    {
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

        public List<Transform> ZombieSpawnPoints;
        public List<Transform> SpecialZombieSpawnPoints;

        private Transform _zombieTarget;

        public int ZombieAtOnceLimit = 20;
        [HideInInspector] public int ZombiesRemaining;
        private int _zombiesWaitingToSpawn;
        private int ZombiesToSpawnThisRound
        {
            get
            {
                if (GameSettings.MoreEnemies)
                    return Mathf.CeilToInt(ZombieCountCurve.Evaluate(RoundManager.Instance.RoundNumber) + 3);
                else
                    return Mathf.CeilToInt(ZombieCountCurve.Evaluate(RoundManager.Instance.RoundNumber));
            }
        }

        public override void Awake()
        {
            base.Awake();

            On.FistVR.Sosig.ProcessDamage_Damage_SosigLink += OnGetHit;
            On.FistVR.Sosig.SosigDies += OnSosigDied;
        }

#region Spawning

        public void BeginSpawningEnemies()
        {
            int zombiesToSpawn = ZombiesToSpawnThisRound;
            if (zombiesToSpawn > ZombieAtOnceLimit)
                zombiesToSpawn = ZombieAtOnceLimit;

            _zombiesWaitingToSpawn = ZombiesToSpawnThisRound;

            for (int i = 0; i < zombiesToSpawn; i++)
            {
                SpawnZombie(2f + i);
            }

            ZombiesRemaining = ZombiesToSpawnThisRound;

            AudioManager.Instance.Play(AudioManager.Instance.RoundStartSound, 0.2f, 1f);
        }

        public void SpawnZombie(float delay)
        {
            StartCoroutine(DelayedZombieSpawn(delay));
        }

        public void OnZombieSpawned(ZombieController controller)
        {
            StartCoroutine(DelayedCustomZombieSpawn(controller));
        }

        private IEnumerator DelayedZombieSpawn(float delay)
        {
            _zombiesWaitingToSpawn--;

            yield return new WaitForSeconds(delay);

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
        }

        private IEnumerator DelayedCustomZombieSpawn(ZombieController controller)
        {
            yield return null;

            Transform spawnPoint = null;
            if (RoundManager.Instance.IsRoundSpecial)
            {
                spawnPoint = SpecialZombieSpawnPoints[Random.Range(0, SpecialZombieSpawnPoints.Count)].transform;
            }
            else
            {
                spawnPoint = ZombieSpawnPoints[Random.Range(0, ZombieSpawnPoints.Count)];
            }

            if (RoundManager.Instance.IsRoundSpecial)
            {
                spawnPoint.GetComponent<CustomSosigSpawnPoint>().SpawnPS.Play(true);

                if (RoundManager.Instance.IsRoundSpecial)
                    AudioManager.Instance.Play(AudioManager.Instance.HellHoundSpawnSound, delay:.25f);

                yield return new WaitForSeconds(2f);
            }

            controller.transform.position = spawnPoint.position;

            if (spawnPoint.GetComponent<ZombieSpawner>() != null)
            {
                Window targetWindow = spawnPoint.GetComponent<ZombieSpawner>().WindowWaypoint;
                if (targetWindow != null)
                    _zombieTarget = targetWindow.ZombieWaypoint;
            }

            controller.Initialize(_zombieTarget);
            ExistingZombies.Add(controller);

            if (RoundManager.Instance.IsRoundSpecial)
            {
                controller.InitializeSpecialType();
            }
        }

        public void SpawnZosig()
        {
            if (RoundManager.Instance.IsRoundSpecial)
            {
                CustomSosigSpawnPoint spawner =
                    SpecialZombieSpawnPoints[Random.Range(0, SpecialZombieSpawnPoints.Count)].GetComponent<CustomSosigSpawnPoint>();

                spawner.Spawn();
            }
            else
            {
                CustomSosigSpawnPoint spawner =
                    ZombieSpawnPoints[Random.Range(0, ZombieSpawnPoints.Count)].GetComponent<CustomSosigSpawnPoint>();

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

        public void OnZombieDied(ZombieController controller)
        {
            if (GameSettings.UseCustomEnemies)
                StartCoroutine(DelayedCustomZombieDespawn(controller.GetComponent<CustomZombieController>()));

            ExistingZombies.Remove(controller);

            if (_zombiesWaitingToSpawn > 0)
                SpawnZombie(2f);

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

        private void OnPlayerChangeLocation(Location location)
        {

        }

        private void OnSosigDied(On.FistVR.Sosig.orig_SosigDies orig, Sosig self, Damage.DamageClass damclass,
            Sosig.SosigDeathType deathtype)
        {
            orig.Invoke(self, damclass, deathtype);
            self.GetComponent<ZosigZombieController>().OnKill();
        }

        private void OnGetHit(On.FistVR.Sosig.orig_ProcessDamage_Damage_SosigLink orig, Sosig self, Damage d,
            SosigLink link)
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

            orig.Invoke(self, d, link);
            self.GetComponent<ZosigZombieController>().OnHit(d);
        }

        private void OnDestroy()
        {
            On.FistVR.Sosig.SosigDies -= OnSosigDied;
            On.FistVR.Sosig.ProcessDamage_Damage_SosigLink -= OnGetHit;
        }
    }
}
#endif