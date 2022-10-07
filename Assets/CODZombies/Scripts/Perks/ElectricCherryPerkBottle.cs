using CODZombies.Scripts.Common;
using CODZombies.Scripts.Managers.Sound;
using CODZombies.Scripts.Player;
using UnityEngine;

namespace CODZombies.Scripts.Perks
{
	public class ElectricCherryPerkBottle : MonoBehaviour, IModifier
	{
		public void ApplyModifier()
		{
			PlayerData.Instance.ElectricCherryPerkActivated = true;
			AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
			Destroy(gameObject);
		}
	}
}
