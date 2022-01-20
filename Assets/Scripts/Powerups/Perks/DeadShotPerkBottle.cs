using CustomScripts.Managers;
using CustomScripts.Player;
using CustomScripts.Zombie;
using UnityEngine;

namespace CustomScripts
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
                        .GetComponent<BoxCollider>().size = new Vector3(1.25f, 1.25f, 1.25f);
                }
            }
            AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
            Destroy(gameObject);
        }
    }
}