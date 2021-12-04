using System.Collections;
using System.Collections.Generic;
using CustomScripts.Powerups;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class PowerUpManager : MonoBehaviourSingleton<PowerUpManager>
    {
        public float ChanceForAmmo = 5f;
        public float ChanceForPowerUp = 10f;

        public bool IsPowerUpCooldown = false;
        public bool IsMaxAmmoCooldown = false;

        public PowerUpMaxAmmo MaxAmmo;

        public List<PowerUp> PowerUps;
        private List<int> randomIndexes = new List<int>();


        public override void Awake()
        {
            base.Awake();

            RoundManager.OnZombieKilled -= RollForPowerUp;
            RoundManager.OnZombieKilled += RollForPowerUp;

            ShuffleIndexes();
        }

        public void RollForPowerUp(GameObject spawnPos)
        {
            int chance = Random.Range(0, 200);
            if (GameSettings.LimitedAmmo && !IsMaxAmmoCooldown)
            {
                if (chance < ChanceForAmmo)
                {
                    StartCoroutine(PowerUpMaxAmmoCooldown());
                    SpawnPowerUp(MaxAmmo, spawnPos.transform.position);
                    return;
                }
            }

            if (IsPowerUpCooldown) //30 sec cooldown between power ups
                return;

            chance = Random.Range(0, 200);
            if (chance < ChanceForPowerUp)
            {
                if (randomIndexes.Count == 0)
                    ShuffleIndexes();

                StartCoroutine(PowerUpMaxAmmoCooldown());
                SpawnPowerUp(PowerUps[randomIndexes[0]], spawnPos.transform.position);

                randomIndexes.RemoveAt(0);
                return;
            }
        }

        private void ShuffleIndexes()
        {
            randomIndexes.Clear();

            for (int i = 0; i < PowerUps.Count; i++)
            {
                randomIndexes.Add(i);
            }

            randomIndexes.Shuffle();
        }

        public void SpawnPowerUp(PowerUp powerUp, Vector3 pos)
        {
            if (powerUp == null)
            {
                Debug.LogWarning("PowerUp spawn failed! PowerUp == null Tell Kodeman");
                return;
            }

            StartCoroutine(PowerUpCooldown());
            powerUp.Spawn(pos + Vector3.up);
        }

        private IEnumerator PowerUpCooldown()
        {
            IsPowerUpCooldown = true;
            yield return new WaitForSeconds(30f);
            IsPowerUpCooldown = false;
        }

        private IEnumerator PowerUpMaxAmmoCooldown()
        {
            IsMaxAmmoCooldown = true;
            yield return new WaitForSeconds(30f);
            IsMaxAmmoCooldown = false;
        }

        private void OnDestroy()
        {
            RoundManager.OnZombieKilled -= RollForPowerUp;
        }
    }
}