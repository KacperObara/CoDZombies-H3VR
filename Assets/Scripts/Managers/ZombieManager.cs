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

        public List<Transform> CustomZombieSpawnPoints;
        public List<CustomSosigSpawnPoint> ZosigSpawnPoints;

        private Transform _zombieTarget;

        public override void Awake()
        {
            base.Awake();

            On.FistVR.Sosig.ProcessDamage_Damage_SosigLink += OnGetHit;
            On.FistVR.Sosig.SosigDies += OnSosigDied;
        }

        public void SpawnZombie(float delay)
        {
            StartCoroutine(DelayedCustomZombieSpawn(delay));
        }

        public void OnZombieSpawned(ZombieController controller)
        {
            Transform spawnPoint =
                CustomZombieSpawnPoints[Random.Range(0, CustomZombieSpawnPoints.Count)];

            controller.transform.position = spawnPoint.position;

            Window targetWindow = spawnPoint.GetComponent<ZombieSpawner>().WindowWaypoint;
            if (targetWindow != null)
                _zombieTarget = targetWindow.ZombieWaypoint;

            controller.Initialize(_zombieTarget);
            ExistingZombies.Add(controller);
        }

        public void OnZosigSpawned(Sosig zosig)
        {
            ZombieController controller = zosig.gameObject.AddComponent<ZosigZombieController>();

            controller.Initialize(_zombieTarget);
            ExistingZombies.Add(controller);
        }

        public void SpawnZosig()
        {
            CustomSosigSpawnPoint spawner =
                ZosigSpawnPoints[Random.Range(0, ZosigSpawnPoints.Count)];

            Window targetWindow = spawner.GetComponent<ZombieSpawner>().WindowWaypoint;
            if (targetWindow != null)
                _zombieTarget = targetWindow.ZombieWaypoint;

            spawner.Spawn();
        }

        public void OnZombieDied(ZombieController controller)
        {
            if (GameSettings.UseCustomEnemies)
                StartCoroutine(DelayedCustomZombieDespawn(controller.GetComponent<CustomZombieController>()));

            ExistingZombies.Remove(controller);

            RoundManager.Instance.ZombiesLeft--;

            if (RoundManager.Instance.ZombiesLeft <= 0)
            {
                RoundManager.Instance.EndRound();
            }

            if (RoundManager.OnZombiesLeftChanged != null)
                RoundManager.OnZombiesLeftChanged.Invoke();
            if (RoundManager.OnZombieKilled != null)
                RoundManager.OnZombieKilled.Invoke(controller.gameObject);
        }

        private IEnumerator DelayedCustomZombieSpawn(float delay)
        {
            yield return new WaitForSeconds(delay);

            _zombieTarget = GameReferences.Instance.Player;

            if (!GameSettings.UseCustomEnemies)
            {
                SpawnZosig();
            }
            else
            {
                ZombiePool.Instance.Spawn();
            }
        }

        private IEnumerator DelayedCustomZombieDespawn(CustomZombieController controller)
        {
            yield return new WaitForSeconds(5f);
            ZombiePool.Instance.Despawn(controller);
        }

        private void OnSosigDied(On.FistVR.Sosig.orig_SosigDies orig, Sosig self, Damage.DamageClass damclass, Sosig.SosigDeathType deathtype)
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