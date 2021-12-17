#if H3VR_IMPORTED
using System.Collections.Generic;
using CustomScripts.Managers;
using CustomScripts.Zombie;
using UnityEngine;
namespace CustomScripts
{
    public class ZombiePool : MonoBehaviourSingleton<ZombiePool>
    {
        public Transform DespawnedWaypoint;

        public List<CustomZombieController> AvailableZombies;

        public override void Awake()
        {
            base.Awake();

            RoundManager.OnGameStarted -= OnGameStart;
            RoundManager.OnGameStarted += OnGameStart;
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= OnGameStart;
        }

        private void OnGameStart()
        {
            if (GameSettings.UseZosigs)
                gameObject.SetActive(false);
        }


        public void Spawn()
        {
            if (AvailableZombies.Count == 0)
            {
                Debug.LogWarning("Trying to spawn too many zombies!");
                return;
            }

            ZombieManager.Instance.OnZombieSpawned(AvailableZombies[0]);
            AvailableZombies.Remove(AvailableZombies[0]);
        }

        public void Despawn(CustomZombieController customZombie)
        {
            AvailableZombies.Add(customZombie);
            customZombie.transform.position = DespawnedWaypoint.position;
        }
    }
}
#endif