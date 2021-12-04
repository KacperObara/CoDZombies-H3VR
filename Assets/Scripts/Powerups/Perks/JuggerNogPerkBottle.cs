using FistVR;
using UnityEngine;

namespace CustomScripts
{
    public class JuggerNogPerkBottle: MonoBehaviour, IModifier
    {
        public float NewHealth = 10000;

        public void ApplyModifier()
        {
            // if (GameSettings.UseZosigs)
            // {
            //     GM.CurrentPlayerBody.SetHealthThreshold(2000f);
            // }
            // else
            // {
            //
            // }
            GM.CurrentPlayerBody.SetHealthThreshold(NewHealth);
            GM.CurrentPlayerBody.ResetHealth();


            AudioManager.Instance.DrinkSound.Play();
            Destroy(gameObject);
        }
    }
}