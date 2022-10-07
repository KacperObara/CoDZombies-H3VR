#if H3VR_IMPORTED
using System.Collections;
using System.Collections.Generic;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Powerups;
using UnityEngine;

namespace CODZombies.Scripts.Managers
{
    public class PowerUpManager : MonoBehaviourSingleton<PowerUpManager>
    {
        public float ChanceForAmmo = 2.5f;
        public float ChanceForPowerUp = 6f;

        public float AmmoCooldownTime;
        public float PowerUpCooldownTime;

        public PowerUpMaxAmmo MaxAmmo;
        public List<PowerUp> PowerUps;

        private readonly List<int> _randomIndexes = new List<int>();

        private bool _isPowerUpCooldown;
        private bool _isMaxAmmoCooldown;

        public override void Awake()
        {
            base.Awake();

            RoundManager.OnZombieKilled -= RollForPowerUp;
            RoundManager.OnZombieKilled += RollForPowerUp;

            ShuffleIndexes();
        }

        private void OnDestroy()
        {
            RoundManager.OnZombieKilled -= RollForPowerUp;
        }

        public void RollForPowerUp(GameObject spawnPos)
        {
            // Chance for Max Ammo
            float chance = Random.Range(0f, 100f);
            if (GameSettings.LimitedAmmo && !_isMaxAmmoCooldown)
            {
                if (chance < ChanceForAmmo)
                {
                    StartCoroutine(PowerUpMaxAmmoCooldown());
                    SpawnPowerUp(MaxAmmo, spawnPos.transform.position);
                    return;
                }
            }

            if (_isPowerUpCooldown)
                return;

            // Chance for other power ups
            chance = Random.Range(0f, 100f);
            if (chance < ChanceForPowerUp)
            {
                if (_randomIndexes.Count == 0)
                    ShuffleIndexes();

                StartCoroutine(PowerUpMaxAmmoCooldown());
                SpawnPowerUp(PowerUps[_randomIndexes[0]], spawnPos.transform.position);

                _randomIndexes.RemoveAt(0);
            }
        }

        /// <summary>
        /// Power ups have randomized order. If one spawns,
        /// it cannot spawn again before all others have spawned too
        /// </summary>
        private void ShuffleIndexes()
        {
            _randomIndexes.Clear();

            for (int i = 0; i < PowerUps.Count; i++)
            {
                _randomIndexes.Add(i);
            }

            Extensions.Shuffle(_randomIndexes);
        }

        public void SpawnPowerUp(PowerUp powerUp, Vector3 pos)
        {
            if (powerUp == null)
            {
                Debug.LogWarning("PowerUp spawn failed! PowerUp == null");
                return;
            }

            StartCoroutine(PowerUpCooldown());
            powerUp.Spawn(pos + Vector3.up);
        }

        private IEnumerator PowerUpCooldown()
        {
            _isPowerUpCooldown = true;
            yield return new WaitForSeconds(PowerUpCooldownTime);
            _isPowerUpCooldown = false;
        }

        private IEnumerator PowerUpMaxAmmoCooldown()
        {
            _isMaxAmmoCooldown = true;
            yield return new WaitForSeconds(AmmoCooldownTime);
            _isMaxAmmoCooldown = false;
        }
    }
}
#endif