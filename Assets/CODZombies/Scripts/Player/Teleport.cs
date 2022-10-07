using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers;
using CODZombies.Scripts.Managers.Sound;
using FistVR;
using UnityEngine;
using UnityEngine.UI;

namespace CODZombies.Scripts.Player
{
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
				AudioManager.Instance.Play(AudioManager.Instance.TeleportingSound, .5f);
			}
		}

	}
}
