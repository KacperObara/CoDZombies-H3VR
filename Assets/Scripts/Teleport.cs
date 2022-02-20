using System.Collections;
using System.Collections.Generic;
using CustomScripts.Managers;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	public Location TargetLocation;

	public void OnLeverPull()
	{
		ZombieManager.Instance.ChangeLocation(TargetLocation);
	}
}
