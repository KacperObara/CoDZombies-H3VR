using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Gamemode;
using CustomScripts.Objects.Weapons;
using CustomScripts.Player;
using CustomScripts.Powerups;
using CustomScripts.Powerups.Perks;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace CustomScripts
{
    public class Speech : MonoBehaviour
    {
        public AudioClip RaygunRolled;
        public AudioClip T33Rolled;
        public AudioClip JuggernogConsumed;

        public AudioClip GettingHit;

        public AudioClip BeingRevived;
        public AudioClip BuyingStaminUp;
        public AudioClip DoublePointsPickedUp;

        public AudioClip UsingPackAPunch;

        public AudioClip Barricading;
        public AudioClip RoundEnd;
        public AudioClip StartingGame;

        public AudioClip SwitchingPower;

        private void Awake()
        {
            MysteryBox.WeaponSpawnedEvent += OnWeaponSpawned;
            JuggerNogPerkBottle.ConsumedEvent += OnJuggernogConsumed;
            StaminUpPerkBottle.ConsumedEvent += OnStaminUpConsumed;
            PlayerData.GettingHitEvent += OnGettingHit;
            PlayerSpawner.BeingRevivedEvent += OnBeingRevived;
            PowerUpDoublePoints.PickedUpEvent += OnDoublePointsPickedUp;
            PackAPunch.PurchaseEvent += OnPackAPunchPurchase;
            RoundManager.OnRoundChanged += OnRoundChanged;
            RoundManager.OnGameStarted += OnGameStarted;
            GameManager.OnPowerEnabled += OnPowerTurnedOn;
        }

        private void Start()
        {
            Window.BarricadedEvent += OnBarricading;
        }

        private void OnGameStarted()
        {
            bool active = Random.Range(0, 2000) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(StartingGame, .5f, 9f);
            RoundManager.OnGameStarted -= OnGameStarted ;
        }

        private void OnRoundChanged()
        {
            bool active = Random.Range(0, 2500) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(RoundEnd, .5f, 9f);
            RoundManager.OnRoundChanged -= OnRoundChanged;
        }

        private void OnBarricading()
        {
            bool active = Random.Range(0, 3000) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(Barricading, .5f, 1f);
            Window.BarricadedEvent -= OnBarricading;
        }

        private void OnPackAPunchPurchase()
        {
            bool active = Random.Range(0, 1500) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(UsingPackAPunch, .5f, 1f);
            PackAPunch.PurchaseEvent -= OnPackAPunchPurchase;
        }

        private void OnWeaponSpawned(WeaponData weapon)
        {
            bool active = Random.Range(0, 850) == 0;
            if (!active)
                return;

            if (weapon.Id == "aftr_raygunmk1")
            {
                AudioManager.Instance.Play(RaygunRolled, .5f);
                MysteryBox.WeaponSpawnedEvent -= OnWeaponSpawned;
            }
            if (weapon.Id == "TT33")
            {
                AudioManager.Instance.Play(T33Rolled, .5f);
                MysteryBox.WeaponSpawnedEvent -= OnWeaponSpawned;
            }
        }

        private void OnJuggernogConsumed()
        {
            bool active = Random.Range(0, 1000) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(JuggernogConsumed, .5f, 3f);
            JuggerNogPerkBottle.ConsumedEvent -= OnJuggernogConsumed;
        }

        private void OnStaminUpConsumed()
        {
            bool active = Random.Range(0, 1000) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(BuyingStaminUp, .5f, 3f);
            StaminUpPerkBottle.ConsumedEvent -= OnStaminUpConsumed;
        }

        private void OnGettingHit()
        {
            bool active = Random.Range(0, 2000) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(GettingHit, .5f, 1f);
            PlayerData.GettingHitEvent -= OnGettingHit;
        }

        private void OnBeingRevived()
        {
            bool active = Random.Range(0, 1000) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(BeingRevived, .5f, 1f);
            PlayerSpawner.BeingRevivedEvent -= OnBeingRevived;
        }

        private void OnDoublePointsPickedUp()
        {
            bool active = Random.Range(0, 1500) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(DoublePointsPickedUp, .5f, 1.5f);
            PowerUpDoublePoints.PickedUpEvent -= OnDoublePointsPickedUp;
        }

        private void OnPowerTurnedOn()
        {
            bool active = Random.Range(0, 1000) == 0;
            if (!active)
                return;

            AudioManager.Instance.Play(SwitchingPower, .5f, 1.5f);
            GameManager.OnPowerEnabled -= OnPowerTurnedOn;
        }


        private void OnDestroy()
        {
            MysteryBox.WeaponSpawnedEvent -= OnWeaponSpawned;
            JuggerNogPerkBottle.ConsumedEvent -= OnJuggernogConsumed;
            StaminUpPerkBottle.ConsumedEvent -= OnStaminUpConsumed;
            PlayerData.GettingHitEvent -= OnGettingHit;
            PlayerSpawner.BeingRevivedEvent -= OnBeingRevived;
            PowerUpDoublePoints.PickedUpEvent -= OnDoublePointsPickedUp;
            PackAPunch.PurchaseEvent -= OnPackAPunchPurchase;
            Window.BarricadedEvent -= OnBarricading;
            RoundManager.OnRoundChanged -= OnRoundChanged;
            RoundManager.OnGameStarted -= OnGameStarted;
            GameManager.OnPowerEnabled -= OnPowerTurnedOn;
        }
    }
}
