using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts.Gamemode.GMDebug;
using FistVR;
using UnityEngine;

public class AmmoBoxLoader : MonoBehaviour
{
	public MagazineBox MagazineBox;

	public CustomItemSpawner ItemSpawner;

	private void Start()
	{
		for (int i = 0; i < 5; i++)
		{
			ItemSpawner.Spawn();
			MagazineBox.AddMagazine(ItemSpawner.SpawnedObject.GetComponent<FVRFireArmMagazine>());
		}
	}
}
