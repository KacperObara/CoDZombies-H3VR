#if H3VR_IMPORTED
using System;
using System.Collections;
using CustomScripts.Managers;
using UnityEngine;
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

        public int ZombieFastWalkRound = 4;
        public int ZombieRunRound = 6;
        private const int zombieLimit = 20;

        public bool IsFastWalking
        {
            get { return RoundNumber >= ZombieFastWalkRound; }
        }

        public bool IsRunning
        {
            get { return RoundNumber >= ZombieRunRound; }
        }

        public void StartGame()
        {
            StartButton.SetActive(false);

            GameManager.Instance.FirstShop.IsFree = true;
            GameManager.Instance.FirstShop.TryBuying();

            RoundNumber = 0;

            AdvanceRound();

            if (OnGameStarted != null)
                OnGameStarted.Invoke();
        }

        public void AdvanceRound()
        {
            if (GameManager.Instance.GameEnded)
                return;

            RoundNumber++;

            int zombiesToSpawn = 0;


            if (GameSettings.MoreEnemies)
                zombiesToSpawn = Mathf.CeilToInt(ZombieManager.Instance.ZombieCountCurve.Evaluate(RoundNumber) + 3);
            else
                zombiesToSpawn = Mathf.CeilToInt(ZombieManager.Instance.ZombieCountCurve.Evaluate(RoundNumber));

            if (zombiesToSpawn > zombieLimit)
                zombiesToSpawn = zombieLimit;

            for (int i = 0; i < zombiesToSpawn; i++)
            {
                ZombieManager.Instance.SpawnZombie(2f + i);
            }

            ZombiesLeft = zombiesToSpawn;

            AudioManager.Instance.RoundStartSound.PlayDelayed(1);

            if (OnZombiesLeftChanged != null)
                OnZombiesLeftChanged.Invoke();
            if (OnRoundChanged != null)
                OnRoundChanged.Invoke();
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
#endif