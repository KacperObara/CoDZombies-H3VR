using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Managers;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour, IPurchasable
{
	public Location TargetLocation;
	public Transform PlayerTeleportWaypoint;
	public Text CostText;
	public int Cost;
	public int PurchaseCost
	{
		get { return Cost; }
	}

	private void OnValidate()
	{
		if (CostText != null)
			CostText.text = PurchaseCost.ToString();
	}

	public void OnLeverPull()
	{
		if (GameManager.Instance.TryRemovePoints(Cost))
		{
			ZombieManager.Instance.ChangeLocation(TargetLocation);
			GM.CurrentMovementManager.TeleportToPoint(PlayerTeleportWaypoint.position, true);
		}
	}

}
