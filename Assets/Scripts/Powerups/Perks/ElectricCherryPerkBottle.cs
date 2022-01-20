using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Player;
using On.FistVR;
using UnityEngine;

public class ElectricCherryPerkBottle : MonoBehaviour, IModifier
{
	public void ApplyModifier()
	{
		PlayerData.Instance.ElectricCherryPerkActivated = true;
		AudioManager.Instance.Play(AudioManager.Instance.DrinkSound);
		Destroy(gameObject);
	}
}
