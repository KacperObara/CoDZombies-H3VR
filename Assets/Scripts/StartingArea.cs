using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;

public class StartingArea : MonoBehaviour
{
	public AudioSource StartingMusic;

	private void Awake()
	{
		RoundManager.OnGameStarted += OnGameStart;
	}

	public void OnGameStart()
	{
		StartingMusic.Stop();
		Destroy(gameObject);
	}

	private void OnDestroy()
	{
		RoundManager.OnGameStarted -= OnGameStart;
	}
}
