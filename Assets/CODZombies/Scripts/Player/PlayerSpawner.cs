using System;
using System.Collections;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using FistVR;
using HarmonyLib;
using UnityEngine;

namespace CODZombies.Scripts.Player
{
    public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner>
    {
        public static Action BeingRevivedEvent;

        public Transform EndGameSpawnerPos;

        private bool _quickReviveJustUsed = false;
        private Vector3 _playerPosOnDeath;

        public override void Awake()
        {
            base.Awake();

            RoundManager.OnGameStarted += OnGameStarted;
        }

        private void OnGameStarted()
        {
            transform.position = EndGameSpawnerPos.position;
        }

        private IEnumerator Start()
        {
            // Wait one frame so that everything is all setup
            yield return null;

            GM.CurrentSceneSettings.DeathResetPoint = transform;
            GM.CurrentMovementManager.TeleportToPoint(transform.position, true, transform.position + transform.forward);
        }
        
        private void OnPlayerDeath()
        {
            if (PlayerData.Instance.QuickRevivePerkActivated)
            {
                PlayerData.Instance.QuickRevivePerkActivated = false;
                _quickReviveJustUsed = true;

                transform.position = GameReferences.Instance.Player.position;

                GM.CurrentPlayerBody.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.High, PowerUpDuration.VeryShort,
                    false, false);

                StartCoroutine(PlayerData.Instance.ActivateInvincibility(10f));

                GM.CurrentPlayerBody.HealPercent(.5f);

                if (BeingRevivedEvent != null)
                    BeingRevivedEvent.Invoke();
            }
            else
            {
                Instance.transform.position = Instance.EndGameSpawnerPos.position;
            }
        }

        [HarmonyPatch(typeof(GM), "BringBackPlayer")]
        [HarmonyPrefix]
        private static void BeforePlayerDeath()
        {
            Instance._playerPosOnDeath = GameReferences.Instance.Player.position;
            Instance.OnPlayerDeath();
        }

        [HarmonyPatch(typeof(GM), "BringBackPlayer")]
        [HarmonyPostfix]
        private static void AfterPlayerDeath()
        {
            GM.CurrentMovementManager.TeleportToPoint(Instance.EndGameSpawnerPos.position, true, Instance.transform.position + Instance.transform.forward);
            GM.CurrentPlayerBody.transform.position = Instance.EndGameSpawnerPos.position;

            if (Instance._quickReviveJustUsed == true)
            {
                Instance._quickReviveJustUsed = false;
                GM.CurrentMovementManager.TeleportToPoint(Instance._playerPosOnDeath, true, Instance._playerPosOnDeath + Instance.transform.forward);
                GM.CurrentPlayerBody.transform.position = Instance._playerPosOnDeath;
            }
        }

        private void OnDestroy()
        {
            RoundManager.OnGameStarted -= OnGameStarted;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0.2f, 0.8f, 0.2f, 0.5f);
            Gizmos.DrawSphere(transform.position, 0.1f);
            Gizmos.DrawLine(transform.position, transform.position + transform.forward * 0.25f);
        }
    }
}