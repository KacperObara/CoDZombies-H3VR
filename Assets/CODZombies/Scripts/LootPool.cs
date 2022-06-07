using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Objects.Weapons;
using UnityEngine;

/// <summary>
/// Id of the Wallshop is the element in the WallShopsPool and WallShopsPrices lists.
/// The first shop will spawn the weapon at the start of the game
/// </summary>
[CreateAssetMenu]
public class LootPool : ScriptableObject
{
	public string LootPoolTitle;

	[Header("Elements in the list corresponds to the ID of the Wallshops.")]
	[Header("The first shop will spawn the weapon for free at the start of the game")]

	public List<WeaponData> WallShopsPool;

	public List<WeaponData> MysteryBoxPool;
	public List<WeaponData> LimitedAmmoMysteryBoxPool;

	public HashSet<WeaponData> PackAPunchPool = new HashSet<WeaponData>();

	private void Awake()
	{
		foreach (var weapon in WallShopsPool)
		{
			PackAPunchPool.Add(weapon);
		}

		foreach (var weapon in MysteryBoxPool)
		{
			PackAPunchPool.Add(weapon);
		}

		foreach (var weapon in LimitedAmmoMysteryBoxPool)
		{
			PackAPunchPool.Add(weapon);
		}
	}
}
