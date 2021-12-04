using System;
using System.Collections;
using CustomScripts.Managers;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class RoundManager : MonoBehaviourSingleton<RoundManager>
    {
        public static Action OnRoundChanged;
        public static Action OnZombiesLeftChanged;
        public static Action<GameObject> OnZombieKilled;

        public static Action OnGameStarted;

        public GameObject StartButton;

        [HideInInspector] public int RoundNumber = 0;
        [HideInInspector] public int ZombiesLeft;

        public int ZombieStartHp = 2;

        [Tooltip("How much zombie HP is added per round")]
        public int ZombieRoundHpIncrement = 1;

        public int ZombieStartCount = 2;

        [Tooltip("How much zombies are added per round")]
        private int ZombieRoundCountIncrement; // Make public and expand

        public int ZombieFastWalkRound = 4;
        public int ZombieRunRound = 6;

        public int ZombieHP => ZombieStartHp + (RoundNumber * ZombieRoundHpIncrement); // 3 4 5 6 7...
        public int WeakerZombieHP => ZombieStartHp + (RoundNumber / 2); // 2 3 3 4 4 5 5...
        public bool IsFastWalking => RoundNumber >= ZombieFastWalkRound;
        public bool IsRunning => RoundNumber >= ZombieRunRound;


        private GameManager gameManager;
        private int zombieLimit = 20;

        private void Start()
        {
            gameManager = GameManager.Instance;
        }

        public void StartGame()
        {
            StartButton.SetActive(false);
            GameReferences.Instance.Respawn.position = gameManager.RespawnWaypoint.position;

            GameManager.Instance.FirstShop.IsFree = true;
            GameManager.Instance.FirstShop.TryBuying();

            RoundNumber = 0;

            if (Random.Range(0, 30) == 0)
                Debug.Log("Are you sure your front doors are locked?");


            AdvanceRound();

            OnGameStarted?.Invoke();

            // if (GameSettings.UseZosigs)
            // {
            //     GM.CurrentPlayerBody.SetHealthThreshold(1000f);
            //     GM.CurrentPlayerBody.ResetHealth();
            // }
        }

        public void AdvanceRound()
        {
            if (GameManager.Instance.GameEnded)
                return;

            RoundNumber++;

            if (GameSettings.MoreEnemies)
                ZombieRoundCountIncrement = 2;
            else
                ZombieRoundCountIncrement = 1;

            ZombieManager.Instance.CleanZombies();

            int zombiesToSpawn = ZombieStartCount + (RoundNumber * ZombieRoundCountIncrement);
            if (zombiesToSpawn > zombieLimit)
                zombiesToSpawn = zombieLimit;

            for (int i = 0; i < zombiesToSpawn; i++)
            {
                ZombieManager.Instance.SpawnZombie(2f + i);
            }

            ZombiesLeft = zombiesToSpawn;

            AudioManager.Instance.RoundStartSound.PlayDelayed(1);

            OnZombiesLeftChanged?.Invoke();
            OnRoundChanged?.Invoke();
        }

        public void EndRound()
        {
            AudioManager.Instance.RoundEndSound.PlayDelayed(1);
            StartCoroutine(DelayedAdvanceRound());
        }


        private IEnumerator DelayedAdvanceRound()
        {
            if (GameSettings.LimitedAmmo)
                yield return new WaitForSeconds(20f);
            else
                yield return new WaitForSeconds(17f);

            AdvanceRound();
        }
    }
}