using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WatchVipVideoPopup : MonoBehaviour
{
    [SerializeField]
    private GameObject _moneyArea, _gemArea;
    [SerializeField]
    private TextMeshProUGUI _moneyLabel;
    [SerializeField]
    private TextMeshProUGUI _gemsLabel;
    [SerializeField]
    private Button _watchVideoButton;

    public Button _closeButton;

    private bool _hasWatchedVideo;

    public UISystem _uiSystem;

    public CarVipSystem _carVipSystem;

    private bool _hasGems;

    private double moneyTotal;

    private int gemsTotal;

    public Vector3 _vipCarPos = Vector3.zero;

   public void Init(bool _gems)
    {
        _hasWatchedVideo = false;
        _hasGems = _gems;
    }

    private void OnEnable()
    {
        _watchVideoButton.onClick.AddListener(OnWatchVideoClickedEvent);
        _closeButton.onClick.AddListener(OnCloseButtonClickInternal);
    }

    private void OnDisable()
    {
        _watchVideoButton.onClick.RemoveListener(OnWatchVideoClickedEvent);
        _closeButton.onClick.RemoveListener(OnCloseButtonClickInternal);
    }

    public void RewardMoney(double _money)
    {
        _moneyArea.SetActive(true);
        _gemArea.SetActive(false);
        moneyTotal = _money;
        _moneyLabel.text = _money.ToShortUnitWithUnits();
    }

    public void RewardGems(int amount)
    {
        _moneyArea.SetActive(false);
        _gemArea.SetActive(true);
        gemsTotal = amount;
        _moneyLabel.text = amount.ToString();
    }

    public void OnCloseButtonClickInternal()
    {
        if (!_hasWatchedVideo)
            CarLeaveParking();
    }

    private void CarLeaveParking()
    {
        _carVipSystem.LeaveCarParking();
    }

    private void OnWatchVideoClickedEvent()
    {
       // OnVideoFinished(true);
        AdsControl.Instance.ShowRewardVideo(AdsControl.ADS_STATE.REWARD_VIP);
    }

    public void OnVideoFinished(bool success)
    {

        if(!success)
        {
            _hasWatchedVideo = false;
            _uiSystem._popupSystem.HidePopup();
        }
        else
        {
            _hasWatchedVideo = false;
            ClaimCarReward();
            _uiSystem._popupSystem.HidePopup();
        }

    }

    void ClaimCarReward()
    {
        if(_hasGems)
        {
            GamePlaySytem.instance.marketSystem.AddGems(gemsTotal);
        }
        else
        {
            GamePlaySytem.instance.marketSystem.AddMoney(moneyTotal, _vipCarPos);
        }

        CarLeaveParking();
    }
}
