using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Player;
using CODZombies.Scripts.Zombie;
using FistVR;
using UnityEngine;

namespace CODZombies.Scripts.Perks
{
    public class DoubleTapPerkBottle : MonoBehaviour, IModifier
    {
        public float DamageMultiplier = 1.5f;

        public void ApplyModifier()
        {
            PlayerData.Instance.DoubleTapPerkActivated = true;
            if (!GameSettings.UseCustomEnemies)
            {
                for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
                {
                    (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
                }
            }

            FVRFireArm heldWeapon = PlayerData.Instance.LeftHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();

            heldWeapon = PlayerData.Instance.RightHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();


            PlayerData.Instance.DamageModifier *= DamageMultiplier;
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(gameObject);
        }
    }
}