﻿using System;
using System.Collections;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Gamemode
{
    public class PlayerSpawner : MonoBehaviourSingleton<PlayerSpawner>
    {
        public static Action BeingRevivedEvent;

        public Transform EndGameSpawnerPos;

        public override void Awake()
        {
            base.Awake();

            On.FistVR.GM.BringBackPlayer += OnPlayerRespawn;

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

                transform.position = GameReferences.Instance.Player.position;

                GM.CurrentPlayerBody.ActivatePower(PowerupType.Invincibility, PowerUpIntensity.High, PowerUpDuration.VeryShort,
                    false, false);

                StartCoroutine(PlayerData.Instance.ActivateInvincibility(10f));

                GM.CurrentPlayerBody.HealPercent(.5f);

                if (BeingRevivedEvent != null)
                    BeingRevivedEvent.Invoke();
            }
        }

        private void OnPlayerRespawn(On.FistVR.GM.orig_BringBackPlayer orig, GM self)
        {
            OnPlayerDeath();

            orig.Invoke(self);

            transform.position = EndGameSpawnerPos.position;
        }

        private void OnDestroy()
        {
            On.FistVR.GM.BringBackPlayer -= OnPlayerRespawn;
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