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
            if (GameSettings.UseZosigs)
            {
                for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
                {
                    (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
                }
            }

            PlayerData.Instance.DamageModifier = DamageMultiplier;
            AudioManager.Instance.DrinkSound.Play();
            Destroy(gameObject);
        }
    }
}