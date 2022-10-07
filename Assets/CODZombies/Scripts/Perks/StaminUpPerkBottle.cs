using System;
using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Player;
using FistVR;
using UnityEngine;

namespace CODZombies.Scripts.Perks
{
    public class StaminUpPerkBottle : MonoBehaviour, IModifier
    {
        public static Action ConsumedEvent;

        public void ApplyModifier()
        {
            PlayerData.Instance.StaminUpPerkActivated = true;
            GM.CurrentSceneSettings.MaxSpeedClamp = 6f;

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(gameObject);
        }
    }
}