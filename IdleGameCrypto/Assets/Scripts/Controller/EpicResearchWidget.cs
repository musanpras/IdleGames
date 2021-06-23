using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EpicResearchWidget : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI _titleLabel;

    [SerializeField]
    private TextMeshProUGUI _desLabel;

    [SerializeField]
    private Image _icon;

    [SerializeField]
    private TextMeshProUGUI _priceTitleLabel;

    [SerializeField]
    private TextMeshProUGUI _priceValueLabel;

    [SerializeField]
    private ButtonPriceReactive _priceReactiveBtn;

    [SerializeField]
    private MarketButton _upgradeButton;

    private bool _isComplete;

    [SerializeField]
    private GameObject _completeObject;

    [SerializeField]
    private GameObject _inProgressObject;

    private Action _onPurchaseCompleted;

    private PremiumResearchStaticModel _model;

    public void SetPurchaseCallback(Action callback)
    {
        _onPurchaseCompleted = callback;
    }

    public void LoadData(PremiumResearchStaticModel _model)
    {
        _titleLabel.text = Language.Get(_model.id.ToUpper(), "RESEARCHES");
        string format = Language.Get(_model.type + "_DESC", "RESEARCHES").ToLower();
        string arg = Language.Get(_model.shop, "SHOPS").ToLower();
        _desLabel.text = string.Format(format, _model._bonus , arg);
        _icon.sprite = GamePlaySytem.instance._atlas.GetSprite(_model.icon);
        _priceReactiveBtn.requiredPrice = _model.prices;
        this._model = _model;
        RefreshButtonColor();
        RefreshVisuals();
    }
    
    private void RefreshButtonColor()
    {
        _priceTitleLabel.text = "UPGRADE";
        if(!_priceReactiveBtn.interactable)
        {
            _priceTitleLabel.color = Color.white;
            _priceValueLabel.color = Color.white;
        }
        else
        {
            _priceTitleLabel.color = Color.yellow;
            _priceValueLabel.color = Color.yellow;
        }
    }

    public void RefreshVisuals()
    {
        switch(_model.id)
        {
            case "COMPULSIVE_CLIENTS":
                if (PlayerPrefs.GetInt("ClientSpeed") == 1)
                    _isComplete = true;
                break;
            case "ROBOTIC_ASSISTANCE":
                if (PlayerPrefs.GetInt("CheckoutSpeed") == 1)
                    _isComplete = true;
                break;
            case "LABOUR_SUBSIDY":
                if (PlayerPrefs.GetInt("SalaryCost") == 1)
                    _isComplete = true;
                break;
            case "PROVIDER_AGREEMENT":
                if (PlayerPrefs.GetInt("CashierCost") == 1)
                    _isComplete = true;
                break;
            case "MARKETING_VIP":
                if (PlayerPrefs.GetInt("RewardVip") == 1)
                    _isComplete = true;
                break;
            case "TAX_REDUCTIONS":
                if (PlayerPrefs.GetInt("CityEarning") == 1)
                    _isComplete = true;
                break;
            case "PRIME_TIME":
                if (PlayerPrefs.GetInt("Marketing") == 1)
                    _isComplete = true;
                break;
        }
        if(_isComplete)
        {
            _completeObject.SetActive(true);
            _upgradeButton.gameObject.SetActive(false);
            //_inProgressObject.SetActive(false);
        }
        else
        {
            _completeObject.SetActive(false);
            _upgradeButton.gameObject.SetActive(true);
            //_inProgressObject.SetActive(true);
        }
        RefreshButtonColor();
    }    

    public void BuyInGame()
    {
        if(GamePlaySytem.instance.marketSystem._view.gems >= _model.prices)
        {
            GamePlaySytem.instance.marketSystem._view.gems -= _model.prices;
            ProcessResearchUpgrade(_model.id);
            _isComplete = true;
            GamePlaySytem.instance._saveSystem.Save();
            RefreshButtonColor();
            RefreshVisuals();
        }
    }

    public void ProcessResearchUpgrade(string _id)
    {
        switch(_id)
        {
            case "COMPULSIVE_CLIENTS":
                GamePlaySytem.instance.IncreaseClientSpeed();
                break;
            case "ROBOTIC_ASSISTANCE":
                GamePlaySytem.instance.IncreaseCheckoutSpeed();
                break;
            case "LABOUR_SUBSIDY":
                GamePlaySytem.instance.ReduceSalaryCost();
                break;
            case "PROVIDER_AGREEMENT":
                GamePlaySytem.instance.ReduceCashierCost();
                break;
            case "MARKETING_VIP":
                GamePlaySytem.instance.IncreaseRewardVip();
                break;
            case "TAX_REDUCTIONS":
                GamePlaySytem.instance.IncreaseCityEarning();
                break;
            case "PRIME_TIME":
                GamePlaySytem.instance.IncreaseMarketing();
                break;

        }
    }
}
