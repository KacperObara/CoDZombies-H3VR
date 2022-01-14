﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts;
using UnityEngine;
using UnityEngine.UI;

public class CanAffordMaterialChanger : MonoBehaviour
{
	private IPurchasable _purchasable;
	[SerializeField] private List<Renderer> _renderers;
	[SerializeField] private List<Text> _texts;

	// private void Awake()
	// {
	// 	_purchasable = GetComponent<IPurchasable>();
	// 	GameManager.OnPointsChanged += OnPointsChanged;
	//
	// 	OnPointsChanged();
	// }

	private void Start()
	{
		_purchasable = GetComponent<IPurchasable>();
		GameManager.OnPointsChanged += OnPointsChanged;

		OnPointsChanged();
	}

	private void OnPointsChanged()
	{
		if (GameManager.Instance.Points >= _purchasable.PurchaseCost)
		{
			for (int i = 0; i < _renderers.Count; i++)
			{
				_renderers[i].materials[1] = GameReferences.Instance.CanBuyMat;
			}
			for (int i = 0; i < _texts.Count; i++)
			{
				_texts[i].color = GameReferences.Instance.CanBuyColor;
			}
		}
		else
		{
			for (int i = 0; i < _renderers.Count; i++)
			{
				_renderers[i].materials[1] = GameReferences.Instance.CannotBuyMat;
			}
			for (int i = 0; i < _texts.Count; i++)
			{
				_texts[i].color = GameReferences.Instance.CannotBuyColor;
			}
		}
	}

	private void OnDestroy()
	{
		GameManager.OnPointsChanged -= OnPointsChanged;
	}
}