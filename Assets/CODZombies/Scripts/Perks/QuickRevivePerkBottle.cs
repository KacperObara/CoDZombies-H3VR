using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Player;
using UnityEngine;

namespace CODZombies.Scripts.Perks
{
    public class QuickRevivePerkBottle : MonoBehaviour, IModifier
    {
        public void ApplyModifier()
        {
            PlayerData.Instance.QuickRevivePerkActivated = true;
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(gameObject);
        }
    }
}