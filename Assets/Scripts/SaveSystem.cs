using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;

public class SaveSystem : MonoBehaviourSingleton<SaveSystem>
{
	public string MapID;

	public override void Awake()
	{
		base.Awake();

		RoundManager.OnGameStarted += SaveStartSettings;
		MapID += "CodZombies";

		LoadStartSettings();
	}

	public void SaveHighscore(int score)
	{
		if (PlayerPrefs.GetInt(MapID + "HighScore") < score)
			PlayerPrefs.SetInt(MapID + "HighScore", score);
	}

	public int GetHighscore()
	{
		return PlayerPrefs.GetInt(MapID + "HighScore");
	}

	public void SaveStartSettings()
	{
		PlayerPrefs.SetInt(MapID + "HardMode", GameSettings.HardMode ? 1 : 0);
		PlayerPrefs.SetInt(MapID + "UseCustomEnemies", GameSettings.UseCustomEnemies ? 1 : 0);
		PlayerPrefs.SetInt(MapID + "LimitedAmmo", GameSettings.LimitedAmmo ? 1 : 0);
		PlayerPrefs.SetInt(MapID + "SpecialRoundDisabled", GameSettings.SpecialRoundDisabled ? 1 : 0);
		PlayerPrefs.SetInt(MapID + "ItemSpawnerSpawned", GameSettings.ItemSpawnerEnabled ? 1 : 0);
		PlayerPrefs.SetInt(MapID + "WeakerEnemiesEnabled", GameSettings.WeakerEnemiesEnabled ? 1 : 0);
	}

	public void LoadStartSettings()
	{
		GameSettings.HardMode =            PlayerPrefs.GetInt(MapID + "HardMode") > 0;
		GameSettings.UseCustomEnemies =    PlayerPrefs.GetInt(MapID + "UseCustomEnemies") > 0;
		GameSettings.LimitedAmmo =         PlayerPrefs.GetInt(MapID + "LimitedAmmo") > 0;
		GameSettings.SpecialRoundDisabled = PlayerPrefs.GetInt(MapID + "SpecialRoundDisabled") > 0;
		GameSettings.ItemSpawnerEnabled =  PlayerPrefs.GetInt(MapID + "ItemSpawnerSpawned") > 0;
		GameSettings.WeakerEnemiesEnabled =  PlayerPrefs.GetInt(MapID + "WeakerEnemiesEnabled") > 0;

		if (GameSettings.OnSettingsChanged != null)
			GameSettings.OnSettingsChanged.Invoke();
	}

	private void OnDestroy()
	{
		RoundManager.OnGameStarted -= SaveStartSettings;
	}
}
