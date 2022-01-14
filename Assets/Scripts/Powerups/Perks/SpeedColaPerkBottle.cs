using CustomScripts.Gamemode;
using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class SpeedColaPerkBottle : MonoBehaviour, IModifier
    {
        public void ApplyModifier()
        {
            PlayerData.Instance.SpeedColaPerkActivated = true;

            FVRFireArm heldWeapon = PlayerData.Instance.LeftHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();

            heldWeapon = PlayerData.Instance.RightHand.CurrentInteractable as FVRFireArm;
            if (heldWeapon != null)
                heldWeapon.GetComponent<WeaponWrapper>().OnWeaponGrabbed();

            AudioManager.Instance.DrinkSound.Play();
            Destroy(gameObject);
        }
    }
}