using System.Collections;
using System.Collections.Generic;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Gamemode
{
    /// <summary>
    /// Weapon script that gets added to every weapon grabbed by the player
    /// </summary>
    public class WeaponWrapper : MonoBehaviour
    {
        public bool DoubleTapActivated = false;
        public bool SpeedColaActivated = false;

        private FVRFireArm weapon;

        public bool PackAPunchDeactivated = false;
        public int TimesPackAPunched = 0;

        public void Initialize(FVRFireArm weapon)
        {
            this.weapon = weapon;
        }

        public void OnPackAPunched()
        {
            TimesPackAPunched++;
        }

        // Called when the weapon is in hand
        public void OnWeaponGrabbed()
        {
            if (!DoubleTapActivated && PlayerData.Instance.DoubleTapPerkActivated)
            {
                DoubleTapActivated = true;
                IncreaseFireRate(1.33f);
            }

            if (!SpeedColaActivated && PlayerData.Instance.SpeedColaPerkActivated)
            {
                SpeedColaActivated = true;
                AddSpeedColaEffect();
            }

            if (PackAPunchDeactivated)
            {
                StartCoroutine(DelayedReActivation());
            }
        }

        public void IncreaseFireRate(float amount)
        {
            if (weapon as Handgun)
            {
                Handgun handgun = (Handgun) weapon;
                handgun.Slide.SpringStiffness *= amount;
                handgun.Slide.Speed_Forward *= amount;
                handgun.Slide.Speed_Rearward *= amount;
            }

            if (weapon as ClosedBoltWeapon)
            {
                ClosedBoltWeapon closedBolt = (ClosedBoltWeapon) weapon;
                closedBolt.Bolt.SpringStiffness *= amount;
                closedBolt.Bolt.Speed_Forward *= amount;
                closedBolt.Bolt.Speed_Rearward *= amount;
            }

            if (weapon as OpenBoltReceiver)
            {
                OpenBoltReceiver openBolt = (OpenBoltReceiver) weapon;
                openBolt.Bolt.BoltSpringStiffness *= amount;
                openBolt.Bolt.BoltSpeed_Forward *= amount;
                openBolt.Bolt.BoltSpeed_Rearward *= amount;
            }
        }

        private void AddSpeedColaEffect()
        {
            if (weapon as Handgun)
            {
                Handgun handgun = (Handgun) weapon;
                handgun.HasMagReleaseButton = true;
            }

            if (weapon as ClosedBoltWeapon)
            {
                ClosedBoltWeapon closedBolt = (ClosedBoltWeapon) weapon;
                closedBolt.HasMagReleaseButton = true;
            }

            if (weapon as OpenBoltReceiver)
            {
                OpenBoltReceiver openBolt = (OpenBoltReceiver) weapon;
                openBolt.HasMagReleaseButton = true;
            }
        }

        // Temporary disabling upgrading to avoid weapon colliding with pack a punch again after spawning.
        // Reactivated 2 seconds after grabbing a weapon.
        public void BlockPackAPunchUpgrade()
        {
            PackAPunchDeactivated = true;
        }

        private IEnumerator DelayedReActivation()
        {
            yield return new WaitForSeconds(2f);

            PackAPunchDeactivated = false;
        }
    }
}