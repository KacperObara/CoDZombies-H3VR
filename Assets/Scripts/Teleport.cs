using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Managers;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

public class Teleport : MonoBehaviour, IPurchasable, IRequiresPower
{
	public Location TargetLocation;
	public Transform PlayerTeleportWaypoint;
	public Text CostText;
	public int Cost;
	public int PurchaseCost { get { return Cost; } }
	[SerializeField] private bool _isOneTimeOnly;
	public bool IsOneTimeOnly { get { return _isOneTimeOnly; } }

	private bool _alreadyBought;
	public bool AlreadyBought { get { return _alreadyBought; } }
	public bool IsPowered { get { return GameManager.Instance.PowerEnabled; } }

	private void OnValidate()
	{
		if (CostText != null)
			CostText.text = PurchaseCost.ToString();
	}

	public void OnLeverPull()
	{
		if (IsPowered && GameManager.Instance.TryRemovePoints(Cost))
		{
			ZombieManager.Instance.ChangeLocation(TargetLocation);
			GM.CurrentMovementManager.TeleportToPoint(PlayerTeleportWaypoint.position, true);
		}
	}

}
