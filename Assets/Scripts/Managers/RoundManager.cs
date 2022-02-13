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
        public static Action RoundStarted;
        public static Action RoundEnded;

        public static Action OnRoundChanged;
        public static Action OnZombiesLeftChanged;
        public static Action<GameObject> OnZombieKilled;

        public static Action OnGameStarted;

        public GameObject StartButton;

        public int ZombieFastWalkRound = 4;
        public int ZombieRunRound = 6;
        public int SpecialRoundInterval;

        [HideInInspector] public int RoundNumber = 0;


        public bool IsRoundSpecial { get { return RoundNumber % SpecialRoundInterval == 0; }}
        public bool IsFastWalking { get { return RoundNumber >= ZombieFastWalkRound; } }
        public bool IsRunning { get { return RoundNumber >= ZombieRunRound; } }

        private Coroutine _roundDelayCoroutine;

        public void StartGame()
        {
            StartButton.SetActive(false);

            GameManager.Instance.GameStarted = true;
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

            ZombieManager.Instance.BeginSpawningEnemies();

            if (OnZombiesLeftChanged != null)
                OnZombiesLeftChanged.Invoke();
            if (OnRoundChanged != null)
                OnRoundChanged.Invoke();
        }

        // private void SpawnEnemies()
        // {
        //     int zombiesToSpawn = 0;
        //
        //     if (GameSettings.MoreEnemies)
        //         zombiesToSpawn = Mathf.CeilToInt(ZombieManager.Instance.ZombieCountCurve.Evaluate(RoundNumber) + 3);
        //     else
        //         zombiesToSpawn = Mathf.CeilToInt(ZombieManager.Instance.ZombieCountCurve.Evaluate(RoundNumber));
        //
        //     if (zombiesToSpawn > ZombieAtOnceLimit)
        //         zombiesToSpawn = ZombieAtOnceLimit;
        //
        //     for (int i = 0; i < zombiesToSpawn; i++)
        //     {
        //         ZombieManager.Instance.SpawnZombie(2f + i);
        //     }
        //
        //     ZombiesLeft = zombiesToSpawn;
        //
        //     AudioManager.Instance.Play(AudioManager.Instance.RoundStartSound, 0.2f, 1f);
        // }
        //
        // public void OnEnemyDied()
        // {
        //
        // }

        public void EndRound()
        {
            AudioManager.Instance.Play(AudioManager.Instance.RoundEndSound, 0.2f, 1f);

            if (RoundEnded != null)
                RoundEnded.Invoke();

            if (GameSettings.LimitedAmmo)
                _roundDelayCoroutine = StartCoroutine(DelayedAdvanceRound(20f));
            else
                _roundDelayCoroutine = StartCoroutine(DelayedAdvanceRound(17f));
        }

        private IEnumerator DelayedAdvanceRound(float delay)
        {
            yield return new WaitForSeconds(delay);

            AdvanceRound();

            if (RoundStarted != null)
                RoundStarted.Invoke();
        }

        public void PauseGame()
        {
            StopCoroutine(_roundDelayCoroutine);
        }

        public void ResumeGame()
        {
            _roundDelayCoroutine = StartCoroutine(DelayedAdvanceRound(0f));
        }
    }
}
#endif