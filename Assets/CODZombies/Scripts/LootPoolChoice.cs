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

	// Used by button
	public void ChangePoolToThis()
	{
		GameSettings.Instance.ChangeLootPool(this);
		IsEnabled = true;
	}
}
