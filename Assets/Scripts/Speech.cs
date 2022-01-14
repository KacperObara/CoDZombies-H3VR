using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using CustomScripts.Gamemode;
using CustomScripts.Objects.Weapons;
using CustomScripts.Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class Speech : MonoBehaviour
{
	public AudioClip RaygunRolled;
	public AudioClip PerkConsumed;
	public AudioClip GettingHit;
	public AudioClip BeingRevived;
	public AudioClip DoublePointsPickedUp;

	private void Awake()
	{
		MysteryBox.WeaponSpawnedEvent += OnWeaponSpawned;
		JuggerNogPerkBottle.ConsumedEvent += OnJuggernogConsumed;
		PlayerData.GettingHitEvent += OnGettingHit;
		PlayerSpawner.BeingRevivedEvent += OnBeingRevived;
		PowerUpDoublePoints.PickedUpEvent += OnDoublePointsPickedUp;
	}

	private void OnWeaponSpawned(WeaponData weapon)
	{
		bool active = Random.Range(0, 850) != 0;
		if (!active)
			return;

		if (weapon.Id == "aftr_raygunmk1")
		{
			AudioManager.Instance.Play(RaygunRolled, .5f);
			MysteryBox.WeaponSpawnedEvent -= OnWeaponSpawned;
		}
	}

	private void OnJuggernogConsumed()
	{
		bool active = Random.Range(0, 1000) != 0;
		if (!active)
			return;

		AudioManager.Instance.Play(PerkConsumed, .5f);
		JuggerNogPerkBottle.ConsumedEvent -= OnJuggernogConsumed;
	}

	private void OnGettingHit()
	{
		bool active = Random.Range(0, 2000) != 0;
		if (!active)
			return;

		AudioManager.Instance.Play(GettingHit, .5f);
		PlayerData.GettingHitEvent -= OnGettingHit;
	}

	private void OnBeingRevived()
	{
		bool active = Random.Range(0, 1000) != 0;
		if (!active)
			return;

		AudioManager.Instance.Play(BeingRevived, .5f);
		PlayerSpawner.BeingRevivedEvent -= OnBeingRevived;
	}

	private void OnDoublePointsPickedUp()
	{
		bool active = Random.Range(0, 1200) != 0;
		if (!active)
			return;

		AudioManager.Instance.Play(DoublePointsPickedUp, .5f);
		PowerUpDoublePoints.PickedUpEvent -= OnDoublePointsPickedUp;
	}


	private void OnDestroy()
	{
		MysteryBox.WeaponSpawnedEvent -= OnWeaponSpawned;
		JuggerNogPerkBottle.ConsumedEvent -= OnJuggernogConsumed;
		PlayerData.GettingHitEvent -= OnGettingHit;
		PlayerSpawner.BeingRevivedEvent -= OnBeingRevived;
		PowerUpDoublePoints.PickedUpEvent -= OnDoublePointsPickedUp;
	}
}
