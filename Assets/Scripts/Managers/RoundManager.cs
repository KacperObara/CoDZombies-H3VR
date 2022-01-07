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

        public int ZombieStartHp = 2;

        [Tooltip("How much zombie HP is added per round")]
        public int ZombieRoundHpIncrement = 1;

        public int ZombieStartCount = 2;

        public int ZombieFastWalkRound = 4;
        public int ZombieRunRound = 6;
        private readonly int _zombieLimit = 20;


        private GameManager _gameManager;

        [Tooltip("How much zombies are added per round")]
        private int _zombieRoundCountIncrement; // Make public and expand

        public int ZombieHp
        {
            get
            {
                return ZombieStartHp + RoundNumber * ZombieRoundHpIncrement;
                // 3 4 5 6 7...
            }
        }

        public int WeakerZombieHp
        {
            get
            {
                return ZombieStartHp + RoundNumber / 2;
                // 2 3 3 4 4 5 5...
            }
        }

        public bool IsFastWalking
        {
            get { return RoundNumber >= ZombieFastWalkRound; }
        }

        public bool IsRunning
        {
            get { return RoundNumber >= ZombieRunRound; }
        }

        private void Start()
        {
            _gameManager = GameManager.Instance;
        }

        public void StartGame()
        {
            StartButton.SetActive(false);
            GameReferences.Instance.Respawn.position = _gameManager.RespawnWaypoint.position;

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

            if (GameSettings.MoreEnemies)
                _zombieRoundCountIncrement = 2;
            else
                _zombieRoundCountIncrement = 1;

            ZombieManager.Instance.CleanZombies();

            var zombiesToSpawn = ZombieStartCount + RoundNumber * _zombieRoundCountIncrement;
            if (zombiesToSpawn > _zombieLimit)
                zombiesToSpawn = _zombieLimit;

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