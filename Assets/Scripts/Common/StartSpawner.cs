#if H3VR_IMPORTED

using FistVR;
using UnityEngine;
namespace CustomScripts
{
    public class StartSpawner : MonoBehaviour
    {
        public ItemSpawner WeaponSpawner;
        public ItemSpawner AmmoSpawner;

        //TODO: I just changed Spawn() to SpawnItem() which is wrong and bad 
        public void Spawn()
        {
            WeaponSpawner.SpawnItem();
            AmmoSpawner.SpawnItem();

            if (GameSettings.LimitedAmmo)
            {
                AmmoSpawner.SpawnItem();
                AmmoSpawner.SpawnItem();
            }
        }
    }
}

#endif