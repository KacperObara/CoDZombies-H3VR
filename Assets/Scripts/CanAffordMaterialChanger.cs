using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts;
using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts
{
    public class CanAffordMaterialChanger : MonoBehaviour
    {
        public bool RequiresPower = true;
        private IPurchasable _purchasable;
        [SerializeField] private List<Text> _texts;

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
                for (int i = 0; i < _texts.Count; i++)
                {
                    _texts[i].color = GameReferences.Instance.CanBuyColor;
                }
            }
            else
            {
                for (int i = 0; i < _texts.Count; i++)
                {
                    _texts[i].color = GameReferences.Instance.CannotBuyColor;
                }
            } //
        }

        private void OnDestroy()
        {
            GameManager.OnPointsChanged -= OnPointsChanged;
        }
    }
}