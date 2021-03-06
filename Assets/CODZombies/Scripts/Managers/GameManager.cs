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
        public static Action OnPowerEnabled;

        [HideInInspector] public int Points;
        [HideInInspector] public int TotalPoints; // for highscore

        [HideInInspector] public bool GameStarted = false;
        [HideInInspector] public bool GameEnded = false;

        public Transform StartGameWaypoint;

        public WallShop FirstShop;

        public bool PowerEnabled;

        [HideInInspector]public int Kills;

        public void TurnOnPower()
        {
            if (PowerEnabled)
                return;

            PowerEnabled = true;
            AudioManager.Instance.Play(AudioManager.Instance.PowerOnSound, .8f);
            if (OnPowerEnabled != null)
                OnPowerEnabled.Invoke();
        }

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

            AudioManager.Instance.PlayMusic(AudioManager.Instance.EndMusic, 0.25f, 1f);

            EndPanel.Instance.UpdatePanel();

            if (!GameSettings.ItemSpawnerEnabled)
                SaveSystem.Instance.SaveHighscore(TotalPoints);
        }
    }
}
#endif