using System;
using UnityEngine;
using WurstMod.MappingComponents.Generic;

namespace CustomScripts
{
    public class StartSpawner : MonoBehaviour
    {
        public ItemSpawner WeaponSpawner;
        public ItemSpawner AmmoSpawner;

        public void Spawn()
        {
            WeaponSpawner.Spawn();
            AmmoSpawner.Spawn();

            if (GameSettings.LimitedAmmo)
            {
                AmmoSpawner.Spawn();
                AmmoSpawner.Spawn();
            }
        }
    }
}