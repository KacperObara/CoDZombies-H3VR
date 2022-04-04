using System;
using System.Collections;
using System.Collections.Generic;
using CustomScripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class DoorBanging : MonoBehaviour
{
    public AudioSource BangingSound;
    private float _timer;
    private bool _activated;

    private void Awake()
    {
        if (Random.Range(0, 20) != 0)
        {
            Destroy(this);
            return;
        }

        RoundManager.OnGameStarted += OnGameStart;
        _timer = Time.time;
    }

    private void Update()
    {
        if (!_activated && _timer + 120 <= Time.time)
        {
            BangingSound.Play();
            _activated = true;
        }
    }

    public void OnGameStart()
    {
        Destroy(this);
    }

    private void OnDestroy()
    {
        RoundManager.OnGameStarted -= OnGameStart;
    }
}