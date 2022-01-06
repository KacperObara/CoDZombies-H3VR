#if H3VR_IMPORTED
using System;
using System.Collections;
using CustomScripts.Objects;
using CustomScripts.Player;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class GameManager : MonoBehaviourSingleton<GameManager>
    {
        public static Action OnPointsChanged;

        [Tooltip("Where the player should respawn on death")]
        public Transform RespawnWaypoint;

        public EndPanel EndPanel;

        [HideInInspector] public int Points;
        [HideInInspector] public int TotalPoints; // for highscore

        [HideInInspector] public bool GameStarted = false;
        [HideInInspector] public bool GameEnded = false;

        public WallShop FirstShop;

        public void AddPoints(int amount)
        {
            float newAmount = amount * PlayerData.Instance.MoneyModifier;

            PlayerData.Instance.MoneyModifier.ToString();

            amount = (int) newAmount;

            Points += amount;
            TotalPoints += amount;

            if (OnPointsChanged != null)
                OnPointsChanged.Invoke();
        }

        public bool TryRemovePoints(int amount)
        {
            if (Points >= amount)
            {
                Points -= amount;

                if (OnPointsChanged != null)
                    OnPointsChanged.Invoke();
                return true;
            }

            return false;
        }

        // public void SpawnZombie(CustomZombieController customZombie)
        // {
        //     int random = Random.Range(0, ZombieSpawnPoints.Count);
        //
        //     customZombie.transform.position = ZombieSpawnPoints[random].transform.position;
        //
        //     customZombie.Initialize();
        //
        //     ExistingZombies.Add(customZombie);
        // }

        // public void OnZombieDied(CustomZombieController controller)
        // {
        //     StartCoroutine(DelayedZombieDespawn(controller));
        //
        //     ExistingZombies.Remove(controller);
        //
        //     RoundManager.Instance.ZombiesLeft--;
        //
        //     RoundManager.OnZombiesLeftChanged.Invoke();
        //     RoundManager.OnZombieKilled.Invoke(controller.gameObject);
        //
        //
        //     if (ExistingZombies.Count <= 0)
        //     {
        //         RoundManager.Instance.EndRound();
        //
        //         for (int i = ExistingZombies.Count - 1; i >= 0; i--)
        //         {
        //             ExistingZombies[i].OnHit(9999);
        //             Debug.LogWarning("Round ended, but there are still zombies existing!");
        //         }
        //     }
        // }

        private IEnumerator DelayedZombieDespawn(CustomZombieController controller)
        {
            yield return new WaitForSeconds(5f);
            ZombiePool.Instance.Despawn(controller);
        }

        public void StartGame()
        {
            CustomZombieController.Playertouches = 0;
            CustomZombieController.IsBeingHit = false;

            GM.CurrentMovementManager.TeleportToPoint(GameStart.Instance.transform.position, false);

            if (GameStarted)
                return;
            GameStarted = true;

            RoundManager.Instance.StartGame();
        }

        public void KillPlayer()
        {
            if (GameEnded)
                return;

            GM.CurrentPlayerBody.KillPlayer(false);
        }

        public void EndGame()
        {
            if (GameEnded)
                return;

            GameEnded = true;

            AudioManager.Instance.EndMusic.PlayDelayed(1f);

            EndPanel.UpdatePanel();

            if (!GameSettings.ItemSpawnerSpawned)
                if (PlayerPrefs.GetInt("BestScore") < TotalPoints)
                    PlayerPrefs.SetInt("BestScore", TotalPoints);
        }
    }
}
#endif