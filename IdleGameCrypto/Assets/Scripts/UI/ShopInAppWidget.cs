using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopInAppWidget : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _amountLabel;

    [SerializeField]
    private TextMeshProUGUI _priceLabel;

    [SerializeField]
    private TextMeshProUGUI _titleLabel;

    private int _id;

    [SerializeField]
    private Image _icon;


    public void LoadData(InAppStaticModel _model)
    {
        _amountLabel.text = _model.reward.ToString();
        _priceLabel.text = _model.price.ToString();
        _titleLabel.text = _model.name.ToString();
        _id = _model.id;
        _icon.sprite = GamePlaySytem.instance._atlas.GetSprite(_model.icon);
    }

    public void BuyInApp()
    {
        IAPManager.instance.BuyProductConsume(_id);
    }
}
