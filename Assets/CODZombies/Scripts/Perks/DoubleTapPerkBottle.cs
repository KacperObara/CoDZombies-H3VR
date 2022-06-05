using CustomScripts.Gamemode;
using CustomScripts.Managers;
using CustomScripts.Player;
using CustomScripts.Zombie;
using FistVR;
using UnityEngine;

namespace CustomScripts
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