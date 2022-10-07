#if H3VR_IMPORTED
using System;
using System.Collections;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers.Sound;
using FistVR;
using UnityEngine;

namespace CODZombies.Scripts.Managers
{
    public class RoundManager : MonoBehaviourSingleton<RoundManager>
    {
        public static Action RoundStarted;
        public static Action RoundEnded;

        public static Action OnRoundChanged;
        public static Action OnZombiesLeftChanged;
        public static Action<GameObject> OnZombieKilled;

        public static Action OnGameStarted;

        public int ZombieFastWalkRound = 2;
        public int ZombieRunRound = 6;
        public int HardModeFastWalkRound = 0;
        public int HardModeRunRound = 3;
        public int SpecialRoundInterval;

        [HideInInspector] public int RoundNumber = 0;

        private Coroutine _roundDelayCoroutine;

        public bool IsRoundSpecial
        {
            get
            {
                if (GameSettings.SpecialRoundDisabled) return false;
                return RoundNumber % SpecialRoundInterval == 0;
            }
        }

        public bool IsFastWalking
        {
            get
            {
                if (GameSettings.HardMode)
                    return RoundNumber >= HardModeFastWalkRound;
                return RoundNumber >= ZombieFastWalkRound;
            }
        }

        public bool IsRunning
        {
            get
            {
                if (GameSettings.HardMode)
                    return RoundNumber >= HardModeRunRound;
                return RoundNumber >= ZombieRunRound;
            }
        }

        public void StartGame()
        {
            if (!Application.isEditor)
            {
                GM.CurrentMovementManager.TeleportToPoint(GameManager.Instance.StartGameWaypoint.position, true);
                GM.CurrentSceneSettings.IsSpawnLockingEnabled = !GameSettings.LimitedAmmo;
            }

            GameManager.Instance.GameStarted = true;

            RoundNumber = 0;

            AdvanceRound();

            if (OnGameStarted != null)
                OnGameStarted.Invoke();

            if (GameManager.Instance.FirstShop)
            {
                GameManager.Instance.FirstShop.IsFree = true;
                GameManager.Instance.FirstShop.TryBuying();
            }
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