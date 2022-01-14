using CustomScripts.Player;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class PHDFlopperPerkBottle : MonoBehaviour, IModifier
    {
        public void ApplyModifier()
        {
            PlayerData.Instance.PHDFlopperPerkActivated = true;
            AudioManager.Instance.DrinkSound.Play();
            Destroy(gameObject);
        }
    }
}