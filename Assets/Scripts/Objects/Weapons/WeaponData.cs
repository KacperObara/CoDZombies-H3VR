using System.Collections.Generic;
using UnityEngine;

namespace CustomScripts.Objects.Weapons
{
    [CreateAssetMenu(fileName = "WeaponData")]
    public class WeaponData : ScriptableObject
    {
        public string Description;

        public string Id => DefaultSpawners[0];

        // First is always weapon, second is always ammo (if exists) in both lists
        public List<string> DefaultSpawners;

        //public List<string> UpgradedSpawners;
        public WeaponData UpgradedWeapon;
        public string UpgradedAmmo;

        public int LimitedAmmoMagazineCount = 0;
        public int AmmoBuyCost = 0;
    }
}