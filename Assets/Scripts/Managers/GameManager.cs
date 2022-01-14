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

        public EndPanel EndPanel;

        [HideInInspector] public int Points;
        [HideInInspector] public int TotalPoints; // for highscore

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