#if H3VR_IMPORTED
using System;
using System.Collections;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using CODZombies.Scripts.Powerups;
using CODZombies.Scripts.Zombie;
using FistVR;
using HarmonyLib;
using UnityEngine;

namespace CODZombies.Scripts.Player
{
    public class PlayerData : MonoBehaviourSingleton<PlayerData>
    {
        public static Action GettingHitEvent;

        public PowerUpIndicator DoublePointsPowerUpIndicator;
        public PowerUpIndicator InstaKillPowerUpIndicator;
        public PowerUpIndicator DeathMachinePowerUpIndicator;

        public float DamageModifier = 1f;
        [HideInInspector] public float MoneyModifier = 1f;

        public float LargeItemSpeedMult;
        public float MassiveItemSpeedMult;
        private float _currentSpeedMult = 1f;

        [HideInInspector] public bool InstaKill;
        [HideInInspector] public bool IsInvincible;

        [HideInInspector] public bool DeadShotPerkActivated;
        [HideInInspector] public bool DoubleTapPerkActivated;
        [HideInInspector] public bool SpeedColaPerkActivated;
        [HideInInspector] public bool QuickRevivePerkActivated;
        [HideInInspector] public bool StaminUpPerkActivated;
        [HideInInspector] public bool PHDFlopperPerkActivated;
        [HideInInspector] public bool ElectricCherryPerkActivated;

        public FVRViveHand LeftHand
        {
            get { return GM.CurrentMovementManager.Hands[0]; }
        }

        public FVRViveHand RightHand
        {
            get { return GM.CurrentMovementManager.Hands[1]; }
        }

        public override void Awake()
        {
            base.Awake();

            RoundManager.OnRoundChanged += OnRoundAdvance;

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

        [HarmonyPatch(typeof(FVRPlayerHitbox), "Damage", new Type[] { typeof(Damage) })]
        [HarmonyPrefix]
        private static void OnBeforePlayerHit(Damage d)
        {
            if (Instance.PHDFlopperPerkActivated && d.Class == Damage.DamageClass.Explosive)
            {
                d.Dam_TotalKinetic *= .3f;
                d.Dam_TotalEnergetic *= .3f;
            }
        
            if (d.Source_IFF != GM.CurrentSceneSettings.DefaultPlayerIFF && GettingHitEvent != null)
                GettingHitEvent.Invoke();
        }


        /// <summary>
        /// Place in which weapon or magazine wrapper classes are added to the objects
        /// </summary>
        [HarmonyPatch(typeof(FVRPhysicalObject), "BeginInteraction")]
        [HarmonyPostfix]
        private static void OnPhysicalObjectStartInteraction(FVRPhysicalObject __instance, FVRViveHand hand)
        {
            if (__instance as FVRFireArm)
            {
                WeaponWrapper wrapper = __instance.GetComponent<WeaponWrapper>();
                if (wrapper == null)
                {
                    wrapper = __instance.gameObject.AddComponent<WeaponWrapper>();
                    wrapper.Initialize((FVRFireArm) __instance);
                }
        
                wrapper.OnWeaponGrabbed();
            }
            else if (__instance as FVRFireArmMagazine)
            {
                MagazineWrapper wrapper = __instance.GetComponent<MagazineWrapper>();
                if (wrapper == null)
                {
                    wrapper = __instance.gameObject.AddComponent<MagazineWrapper>();
                    wrapper.Initialize((FVRFireArmMagazine) __instance);
                }
            }
        }
        
        [HarmonyPatch(typeof(FVRFireArmMagazine), "Release")]
        [HarmonyPostfix]
        private static void OnMagRelease(FVRFireArmMagazine __instance, bool PhysicalRelease = false)
        {
            //////// Electric cherry
            if (Instance.ElectricCherryPerkActivated && __instance.m_numRounds == 0)
            {
                if (!Instance.stunThrottle)
                    Instance.StartCoroutine(Instance.ActivateStun());
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