using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Gamemode.GMDebug;
using FistVR;
using UnityEngine;
using UnityEngine.Serialization;

// TODO Auto loading to the box Not working
public class AmmoBoxLoader : MonoBehaviour
{
	//public MagazineBox MagazineBox;

	//public CustomItemSpawner GunItemSpawner;
	public List<CustomItemSpawner> AmmmoItemSpawners;

	private IEnumerator Start()
	{
		//GunItemSpawner.Spawn();
		for (int i = 0; i < AmmmoItemSpawners.Count; i++)
		{
			AmmmoItemSpawners[i].Spawn();

			yield return null;

			//
			// MagazineBox.AddMagazine(AmmmoItemSpawner.SpawnedObject.GetComponent<FVRFireArmMagazine>());
		}
	}
}
