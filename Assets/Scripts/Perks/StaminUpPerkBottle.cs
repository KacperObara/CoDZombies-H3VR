using System;
using CustomScripts.Player;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class StaminUpPerkBottle : MonoBehaviour, IModifier
    {
        public static Action ConsumedEvent;

        public void ApplyModifier()
        {
            PlayerData.Instance.StaminUpPerkActivated = true;

            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(gameObject);
        }
    }
}