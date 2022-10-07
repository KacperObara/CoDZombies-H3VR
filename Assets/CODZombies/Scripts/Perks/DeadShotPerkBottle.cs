using CODZombies.Scripts.Common;
using CODZombies.Scripts.Gamemode;
using CODZombies.Scripts.Managers;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Player;
using CODZombies.Scripts.Zombie;
using UnityEngine;

namespace CODZombies.Scripts.Perks
{
    public class DeadShotPerkBottle : MonoBehaviour, IModifier
    {
        public void ApplyModifier()
        {
            PlayerData.Instance.DeadShotPerkActivated = true;

            if (!GameSettings.UseCustomEnemies)
            {
                for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
                {
                    (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
                }
            }
            else
            {
                for (int i = 0; i < ZombieManager.Instance.AllCustomZombies.Count; i++)
                {
                    (ZombieManager.Instance.AllCustomZombies[i] as CustomZombieController).HeadObject
                        .GetComponent<CapsuleCollider>().radius *= 1.375f;
                }
            }
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(gameObject);
        }
    }
}