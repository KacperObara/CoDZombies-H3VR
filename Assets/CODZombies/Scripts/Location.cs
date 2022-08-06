using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;

public class Location : MonoBehaviour
{
	public List<Transform> ZombieSpawnPoints;
	public List<Transform> SpecialZombieSpawnPoints;

	private void Start()
	{
		if (ZombieSpawnPoints.Count == 0)
			Debug.LogError("ZombieSpawnPoints is empty, zombies don't have a place to spawn");

		if (SpecialZombieSpawnPoints.Count == 0)
			Debug.LogError("SpecialZombieSpawnPoints is empty, special zombies don't have a place to spawn");
	}
}
