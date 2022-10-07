using System.Collections;
using System.Collections.Generic;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Objects;
using CODZombies.Scripts.Zombie;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts.Player
{
    public class ElectroTrap : MonoBehaviour, IPurchasable, IRequiresPower, ITrap
    {
        public Blockade RequiredBlockade;
        public GameObject LeversCanvases;

        public int Cost;
        public int PurchaseCost { get { return Cost; } }
        [SerializeField] private bool _isOneTimeOnly;
        public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

        private bool _alreadyBought;
        public bool AlreadyBought { get { return _alreadyBought; } }
        public bool IsPowered { get { return GameManager.Instance.PowerEnabled; } }

        public float EnabledTime;
        public float PlayerTouchDamage;

        public ParticleSystem ElectricityPS;
        public AudioSource ElectricityAudio;
        public Collider DamageTrigger;
        public GameObject PlayerBlocker;

        public List<Text> CanvasCostTexts;

        private bool _activated = false;
        private bool _damageThrottled = false;

        private void OnValidate()
        {
            for (int i = 0; i < CanvasCostTexts.Count; i++)
            {
                CanvasCostTexts[i].text = Cost.ToString();
            }
        }

        private void Awake()
        {
            if (RequiredBlockade)
            {
                LeversCanvases.SetActive(false);
                RequiredBlockade.OnPurchase += EnableLevers;
            }
        }

        private void EnableLevers()
        {
            LeversCanvases.SetActive(true);
            RequiredBlockade.OnPurchase -= EnableLevers;
        }

        // I can't put this into OnTriggerEnter, because zosigs don't call it
        public void OnEnemyEntered(ZombieController controller)
        {
            controller.OnHit(99999, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<PlayerCollider>())
            {
                if (_damageThrottled)
                    return;

                AudioManager.Instance.Play(AudioManager.Instance.PlayerHitSound);
                GM.CurrentPlayerBody.Health -= PlayerTouchDamage;

                if (GM.CurrentPlayerBody.Health <= 0)
                    GameManager.Instance.KillPlayer();

                StartCoroutine(ThrottleDamage());
            }
        }

        public void OnLeverPull()
        {
            if (_activated)
                return;

            if (IsPowered && GameManager.Instance.TryRemovePoints(Cost))
            {
                ActivateTrap();
            }
        }

        private void ActivateTrap()
        {
            _activated = true;
            ElectricityPS.Play(true);
            ElectricityAudio.Play();
            PlayerBlocker.SetActive(true);
            DamageTrigger.enabled = true;

            StartCoroutine(DelayedDeactivateTrap());
        }

        private IEnumerator DelayedDeactivateTrap()
        {
            yield return new WaitForSeconds(EnabledTime);

            _activated = false;
            ElectricityPS.Stop(true);
            ElectricityAudio.Stop();
            PlayerBlocker.SetActive(false);
            DamageTrigger.enabled = false;
        }

        private IEnumerator ThrottleDamage()
        {
            _damageThrottled = true;
            yield return new WaitForSeconds(1.5f);
            _damageThrottled = false;
        }

        private void OnDestroy()
        {
            if (RequiredBlockade)
                RequiredBlockade.OnPurchase -= EnableLevers;
        }
    }
}