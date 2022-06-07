#if H3VR_IMPORTED

using System;
using System.Collections.Generic;
using UnityEngine;
namespace CustomScripts.Objects.Weapons
{
    [CreateAssetMenu(fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string Id
        {
            get { return DefaultSpawners[0]; }
        }

        [Tooltip("Used by WallShop")]
        public string DisplayName;

        [Tooltip("No real use in the code btw")]
        public string Description;

        [Tooltip("First is always weapon, second is always ammo (if exists)")]
        public List<string> DefaultSpawners;

        [Tooltip("Result of pack a punching")]
        public WeaponData UpgradedWeapon;

        [Tooltip("How many magazines spawn in the limited ammo mode")]
        public int LimitedAmmoMagazineCount = 0;

        [Tooltip("How much does the weapon cost in the Wallshop")]
        public int Price;

        [Tooltip("How much does ammo cost in the limited ammo mode (Wall rebuy)")]
        public int AmmoBuyCost = 0;
    }
}
#endif