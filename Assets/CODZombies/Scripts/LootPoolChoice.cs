using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;
using UnityEngine.UI;

public class LootPoolChoice : MonoBehaviour
{
	public LootPool LootPool;
	public Text LootChoiceText;
	[HideInInspector] public bool IsEnabled;

	private void Awake()
	{
		LootChoiceText.text = LootPool.LootPoolTitle;
	}

	// Used by button
	public void ChangePoolToThis()
	{
		GameSettings.Instance.ChangeLootPool(this);
		IsEnabled = true;
	}
}
