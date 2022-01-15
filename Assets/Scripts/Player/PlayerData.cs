#if H3VR_IMPORTED
using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Gamemode;
using CustomScripts.Managers;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;

namespace CustomScripts.Player
{
    public class PlayerData : MonoBehaviourSingleton<PlayerData>
    {
        public static Action GettingHitEvent;

        public PowerUpIndicator DoublePointsPowerUpIndicator;
        public PowerUpIndicator InstaKillPowerUpIndicator;
        public PowerUpIndicator DeathMachinePowerUpIndicator;

        public float DamageModifier = 1f;
        public float MoneyModifier = 1f;

        public float LargeItemSpeedMult;
        public float MassiveItemSpeedMult;
        private float currentSpeedMult = 1f;

        public bool InstaKill;
        public bool IsInvincible;

        public bool DeadShotPerkActivated;
        public bool DoubleTapPerkActivated;
        public bool SpeedColaPerkActivated;
        public bool QuickRevivePerkActivated;
        public bool StaminUpPerkActivated;
        public bool PHDFlopperPerkActivated;
        public bool ElectricCherryPerkActivated;

        public FVRViveHand LeftHand { get { return GM.CurrentMovementManager.Hands[0]; } }
        public FVRViveHand RightHand { get { return GM.CurrentMovementManager.Hands[1]; } }

        public override void Awake()
        {
            base.Awake();

            RoundManager.OnRoundChanged += OnRoundAdvance;

            On.FistVR.FVRPhysicalObject.BeginInteraction += OnPhysicalObjectStartInteraction;
            On.FistVR.FVRPhysicalObject.EndInteraction += OnPhysicalObjectEndInteraction;
            On.FistVR.FVRPhysicalObject.EndInteractionIntoInventorySlot += OnPhysicalObjectEndInteractionIntoInventorySlot;
            On.FistVR.FVRPlayerHitbox.Damage_Damage += OnPlayerHit;

            On.FistVR.FVRFireArmMagazine.Release += OnMagRelease;

            DeadShotPerkActivated = false;
            DoubleTapPerkActivated = false;
            SpeedColaPerkActivated = false;
            QuickRevivePerkActivated = false;
            StaminUpPerkActivated = false;
        }

        private void OnRoundAdvance()
        {
            GM.CurrentPlayerBody.HealPercent(1f);
        }

        private void OnPlayerHit(On.FistVR.FVRPlayerHitbox.orig_Damage_Damage orig, FistVR.FVRPlayerHitbox self,
            Damage d)
        {
            if (PHDFlopperPerkActivated && d.Class == Damage.DamageClass.Explosive)
            {
                d.Dam_TotalKinetic *= .3f;
                d.Dam_TotalEnergetic *= .3f;
            }

            if (d.Source_IFF != GM.CurrentSceneSettings.DefaultPlayerIFF && GettingHitEvent != null)
                GettingHitEvent.Invoke();

            orig.Invoke(self, d);
        }

        private void OnPhysicalObjectStartInteraction(On.FistVR.FVRPhysicalObject.orig_BeginInteraction orig,
            FVRPhysicalObject self, FVRViveHand hand)
        {
            orig.Invoke(self, hand);

            OnItemHeldChange();
            if (self as FVRFireArm)
            {
                WeaponWrapper wrapper = self.GetComponent<WeaponWrapper>();
                if (wrapper == null)
                {
                    wrapper = self.gameObject.AddComponent<WeaponWrapper>();
                    wrapper.Initialize((FVRFireArm) self);
                }

                wrapper.OnWeaponGrabbed();
            }
        }

        private void OnPhysicalObjectEndInteraction(On.FistVR.FVRPhysicalObject.orig_EndInteraction orig,
            FVRPhysicalObject self, FVRViveHand hand)
        {
            orig.Invoke(self, hand);
            StartCoroutine(DelayedItemChange());
        }

        private void OnPhysicalObjectEndInteractionIntoInventorySlot(
            On.FistVR.FVRPhysicalObject.orig_EndInteractionIntoInventorySlot orig, FVRPhysicalObject self,
            FVRViveHand hand, FVRQuickBeltSlot slot)
        {
            orig.Invoke(self, hand, slot);
            StartCoroutine(DelayedItemChange());
        }

        private IEnumerator DelayedItemChange()
        {
            yield return new WaitForSeconds(.1f);

            OnItemHeldChange();
        }

        // Called on FVRFireArm grabbed or released
        private void OnItemHeldChange()
        {
            if (!StaminUpPerkActivated)
            {
                FVRPhysicalObject.FVRPhysicalObjectSize heaviestItem = FVRPhysicalObject.FVRPhysicalObjectSize.Small;

                if (LeftHand.CurrentInteractable != null && LeftHand.CurrentInteractable as FVRPhysicalObject != null)
                {
                    if (((FVRPhysicalObject) LeftHand.CurrentInteractable).Size > heaviestItem)
                        heaviestItem = ((FVRPhysicalObject) LeftHand.CurrentInteractable).Size;
                }

                if (RightHand.CurrentInteractable != null && RightHand.CurrentInteractable as FVRPhysicalObject != null)
                {
                    if (((FVRPhysicalObject) RightHand.CurrentInteractable).Size > heaviestItem)
                        heaviestItem = ((FVRPhysicalObject) RightHand.CurrentInteractable).Size;
                }

                switch (heaviestItem)
                {
                    case FVRPhysicalObject.FVRPhysicalObjectSize.Large:
                        currentSpeedMult = LargeItemSpeedMult;
                        break;
                    case FVRPhysicalObject.FVRPhysicalObjectSize.Massive:
                        currentSpeedMult = MassiveItemSpeedMult;
                        break;
                    default:
                        currentSpeedMult = 1f;
                        break;
                }
            }
            else
            {
                currentSpeedMult = 1.1f;
            }

            for (int i = 0; i < GM.Options.MovementOptions.ArmSwingerBaseSpeeMagnitudes.Length; i++)
            {
                GM.Options.MovementOptions.ArmSwingerBaseSpeeMagnitudes[i] = .75f * currentSpeedMult;
            }

            GM.CurrentSceneSettings.UsesMaxSpeedClamp = true;
            GM.CurrentSceneSettings.MaxSpeedClamp = 5.5f * currentSpeedMult;
        }


        private void OnMagRelease(On.FistVR.FVRFireArmMagazine.orig_Release orig, FVRFireArmMagazine self, bool physicalrelease)
        {
            orig.Invoke(self, physicalrelease);

            //////// Electric cherry
            if (ElectricCherryPerkActivated && self.m_numRounds == 0)
            {
                if (!stunThrottle)
                    StartCoroutine(ActivateStun());
            }
        }

        private bool stunThrottle = false;
        public IEnumerator ActivateStun()
        {
            stunThrottle = true;

            if (GameSettings.UseCustomEnemies)
            {
                for (int i = 0; i < ZombieManager.Instance.AllCustomZombies.Count; i++)
                {
                    StartCoroutine((ZombieManager.Instance.AllCustomZombies[i] as CustomZombieController).Stun());
                }
            }
            else
            {
                for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
                {
                    (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).Stun(2f);
                }
            }

            yield return new WaitForSeconds(5f);
            stunThrottle = false;
        }

        public IEnumerator ActivateInvincibility(float time)
        {
            IsInvincible = true;

            yield return new WaitForSeconds(10f);

            IsInvincible = false;
        }

        private void OnDestroy()
        {
            RoundManager.OnRoundChanged -= OnRoundAdvance;

            On.FistVR.FVRPhysicalObject.BeginInteraction -= OnPhysicalObjectStartInteraction;
            On.FistVR.FVRPhysicalObject.EndInteraction -= OnPhysicalObjectEndInteraction;
            On.FistVR.FVRPhysicalObject.EndInteractionIntoInventorySlot -= OnPhysicalObjectEndInteractionIntoInventorySlot;

            On.FistVR.FVRPlayerHitbox.Damage_Damage -= OnPlayerHit;

            On.FistVR.FVRFireArmMagazine.Release -= OnMagRelease;

            GM.Options.MovementOptions.ArmSwingerBaseSpeeMagnitudes = new float[6]
            {
                0.0f,
                0.15f,
                0.25f,
                0.5f,
                0.8f,
                1.2f
            };
        }
    }
}
#endif