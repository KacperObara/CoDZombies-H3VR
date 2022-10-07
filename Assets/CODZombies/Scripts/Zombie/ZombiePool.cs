#if H3VR_IMPORTED
using System.Collections.Generic;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using UnityEngine;

namespace CODZombies.Scripts.Zombie
{
    public class ZombiePool : MonoBehaviour
    {
        public Transform DespawnedWaypoint;

        public List<CustomZombieController> AvailableZombies;

        private void Awake()
        {
            RoundManager.OnGameStarted += OnGameStart;
        }

        private void Start()
        {
            for (int i = 0; i < AvailableZombies.Count; i++)
            {
                AvailableZombies[i].gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= OnGameStart;
        }

        private void OnGameStart()
        {
            if (!GameSettings.UseCustomEnemies)
                gameObject.SetActive(false);
        }


        public void Spawn()
        {
            if (AvailableZombies.Count == 0)
            {
                Debug.LogWarning("Trying to spawn too many zombies!");
                return;
            }

            AvailableZombies[0].gameObject.SetActive(true);

            ZombieManager.Instance.OnZombieSpawned(AvailableZombies[0]);

            if (AvailableZombies[0].Ragdoll)
                AvailableZombies[0].Ragdoll.ResetRagdoll();

           //AvailableZombies[0].Animator.enabled = true;

            AvailableZombies.Remove(AvailableZombies[0]);
        }

        public void Despawn(CustomZombieController customZombie)
        {
            if (customZombie.Ragdoll)
                customZombie.Ragdoll.DeactivateRagdoll();

            AvailableZombies.Add(customZombie);
            customZombie.transform.position = DespawnedWaypoint.position;

            customZombie.gameObject.SetActive(false);
        }
    }
}
#endif