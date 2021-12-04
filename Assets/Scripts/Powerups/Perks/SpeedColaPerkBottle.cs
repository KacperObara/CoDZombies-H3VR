using CustomScripts.Player;
using UnityEngine;

namespace CustomScripts.Powerups.Perks
{
    public class SpeedColaPerkBottle : MonoBehaviour, IModifier
    {
        public void ApplyModifier()
        {
            PlayerData.Instance.SpeedColaPerkActivated = true;

            AudioManager.Instance.DrinkSound.Play();
            Destroy(gameObject);
        }
    }
}