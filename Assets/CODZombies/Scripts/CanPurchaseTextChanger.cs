using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CustomScripts;
using UnityEngine;
using UnityEngine.UI;

namespace CustomScripts
{
    [RequireComponent(typeof(IPurchasable))]
    public class CanPurchaseTextChanger : MonoBehaviour
    {
        private IPurchasable _purchasable;
        private IRequiresPower _requiresPower;
        [SerializeField] private List<Text> _texts;

        private void Start()
        {
            _purchasable = GetComponent<IPurchasable>();
            _requiresPower = GetComponent<IRequiresPower>();

            GameManager.OnPointsChanged += UpdateTexts;
            GameManager.OnPowerEnabled += UpdateTexts;

            UpdateTexts();
        }

        private void UpdateTexts()
        {
            if (_requiresPower != null)
            {
                if (!_requiresPower.IsPowered)
                {
                    for (int i = 0; i < _texts.Count; i++)
                    {
                        _texts[i].text = "No power";
                        _texts[i].color = GameReferences.Instance.CannotBuyColor;
                    }
                    return;
                }
            }

            if (_purchasable.IsOneTimeOnly && _purchasable.AlreadyBought)
            {
                for (int i = 0; i < _texts.Count; i++)
                {
                    _texts[i].text = "Out of stock";
                    _texts[i].color = GameReferences.Instance.CannotBuyColor;
                }
                return;
            }

            if (GameManager.Instance.Points >= _purchasable.PurchaseCost)
            {
                for (int i = 0; i < _texts.Count; i++)
                {
                    _texts[i].text = _texts[i].text = _purchasable.PurchaseCost.ToString();
                    _texts[i].color = GameReferences.Instance.CanBuyColor;
                }
            }
            else
            {
                for (int i = 0; i < _texts.Count; i++)
                {
                    _texts[i].text = _texts[i].text = _purchasable.PurchaseCost.ToString();
                    _texts[i].color = GameReferences.Instance.CannotBuyColor;
                }
            }
        }

        private void OnDestroy()
        {
            GameManager.OnPointsChanged -= UpdateTexts;
            GameManager.OnPowerEnabled -= UpdateTexts;
        }

    }
}