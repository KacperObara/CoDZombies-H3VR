using CustomScripts.Player;
using FistVR;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class QuickRevivePerkBottle : MonoBehaviour, IModifier
    {
        public void ApplyModifier()
        {
            PlayerData.Instance.QuickRevivePerkActivated = true;
            AudioManager.Instance.DrinkSound.Play();
            Destroy(gameObject);
        }
    }
}