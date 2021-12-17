#if H3VR_IMPORTED
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

            if (GameSettings.UseZosigs)
            {
                for (int i = 0; i < ZombieManager.Instance.ExistingZombies.Count; i++)
                {
                    (ZombieManager.Instance.ExistingZombies[i] as ZosigZombieController).CheckPerks();
                }
            }
            else
            {
                for (int i = 0; i < ZombieManager.Instance.AllZombies.Count; i++)
                {
                    (ZombieManager.Instance.AllZombies[i] as CustomZombieController).HeadObject
                        .GetComponent<BoxCollider>().size = new Vector3(1.25f, 1.25f, 1.25f);
                }
            }

            AudioManager.Instance.DrinkSound.Play();
            Destroy(gameObject);
        }
    }
}
#endif