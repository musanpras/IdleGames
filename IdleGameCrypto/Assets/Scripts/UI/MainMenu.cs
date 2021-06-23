using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MarketMenu
{

    public enum eQuestMode
    {
        NONE,
        COMPLETED,
        IN_PROGRESS
    }

   [SerializeField]
    private CanvasGroup _prestigeArea;
   
    [SerializeField]
    private CanvasGroup _bottomButtonsArea;
   
    [SerializeField]
    private GameObject _multiplierMarkerActive;

    [SerializeField]
    private CanvasGroup _codiZoneArea;
   
    [SerializeField]
    private GameObject _multiplierMarkerInactive;
  
    [SerializeField]
    private TextMeshProUGUI _multiplierTimeLeftLabel;
   
    [SerializeField]
    private Button _settingsButton;
    
    [SerializeField]
    private Button _googleButton;
   
    [SerializeField]
    private Image _googleIcon;
    
    [SerializeField]
    private Button _prestigeButton;
    
    [SerializeField]
    private Button _statsButton;
   
    [SerializeField]
    private GameObject _evolutionNotification;
   
    [SerializeField]
    private Button _questButton;
    
    [SerializeField]
    private Image _questButtonIcon;

    [SerializeField]
    private Button _multiplyButton;
   
    [SerializeField]
    private Button _toolsButton;

    [SerializeField]
    private Button _shopButton;
   
    [SerializeField]
    private Button _generalManagerOnButton;
  
    [SerializeField]
    private Button _generalManagerOffButton;
   
    [SerializeField]
    private Button _saleButton;
   
    [SerializeField]
    private CanvasGroup _questInfoWidget;
 
    [SerializeField]
    private CanvasGroup _boostersArea;
   
    [SerializeField]
    private Button _busBoosterButton;
  
    [SerializeField]
    private Button _shopsBoosterButton;
 
    [SerializeField]
    private Button _checkoutsBoosterButton;
 
    [SerializeField]
    private CanvasGroup _boostersTimersArea;
    
    [SerializeField]
    private CanvasGroup _shopsTimer;
    
    [SerializeField]
    private TextMeshProUGUI _shopsTimerLabel;
   
    [SerializeField]
    private CanvasGroup _checkoutsTimer;
  
    [SerializeField]
    private TextMeshProUGUI _checkoutsTimerLabel;




    public void InitializeInternal()
    {
       
        ShowAnimatedInternal();
        RefreshMultiplierButton();
        /*
        _googlePlayButtonConnectedColor = _colorLibrary.GetAssociatedColor(“googlePlayButtonConnected”);
        _googlePlayButtonDisconnectedColor = _colorLibrary.GetAssociatedColor(“googlePlayButtonDisconnected”);
        CheckFirstSession();
        _questController.SetListener(RefreshQuestButton);
        RefreshManagerButtons();
        _busBoosterButton.Initialize();
        _shopsBoosterButton.Initialize();
        _checkoutsBoosterButton.Initialize();
        RefreshBoostersButtons();
        _isShopsBoosterTimerVisible = true;
        _isCheckoutsBoosterTimerButtonVisible = true;
        HideShopsBoosterTimer();
        HideCheckoutsBoosterTimer();
        _codizoneWidgets.Initialize();
        */
    }



    protected override Sequence HideAnimatedInternal(float duration = 0.2F)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetId("UI");
        sequence.Append(UIAnimations.HideVerticalOut(_bottomButtonsArea, duration, -100f, isSameInDirection: false));
        sequence.Join(UIAnimations.HideGroupFadeOut(_bottomButtonsArea, duration));
        sequence.Join(UIAnimations.HideVerticalOut(_prestigeArea, duration, 100f, isSameInDirection: false));
        sequence.Join(UIAnimations.HideGroupFadeOut(_prestigeArea, duration));
        sequence.Join(UIAnimations.HideGroupFadeOut(_boostersArea, duration));
        sequence.Join(UIAnimations.HideGroupFadeOut(_boostersTimersArea, duration));
        sequence.Join(UIAnimations.HideGroupFadeOut(_codiZoneArea, duration));
        return sequence;
    }

    protected override Sequence ShowAnimatedInternal(float duration = 0.2F)
    {
        //RefreshManagerButtons();
        //RefreshSocialStatus();
        Sequence sequence = DOTween.Sequence();
        sequence.SetId("UI");
        float num = 100f;
        //float num2 = (!MenusBackgroundController.HasNotch) ? (-50) : 0;
        float num2 = 0;
        _bottomButtonsArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f - num);
        sequence.Join(UIAnimations.ShowVerticalIn(_bottomButtonsArea, duration, num + num2));
        sequence.Join(UIAnimations.ShowGroupFadeIn(_bottomButtonsArea, duration));
        _prestigeArea.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, num);
        sequence.Join(UIAnimations.ShowVerticalIn(yOffset: 0f - num + (float)((!MenusBackgroundController.HasNotch) ? 100 : 0), group: _prestigeArea, duration: duration));
        sequence.Join(UIAnimations.ShowGroupFadeIn(_prestigeArea, duration));
        sequence.Join(UIAnimations.ShowGroupFadeIn(_boostersArea, duration));
        sequence.Join(UIAnimations.ShowGroupFadeIn(_boostersTimersArea, duration));
        sequence.Join(UIAnimations.ShowGroupFadeIn(_codiZoneArea, duration));
       // sequence.AppendCallback(RefreshQuestButton);
       // sequence.AppendCallback(RefreshCodiZone);
        return sequence;
    }

    protected override void SubscribeToEventsInternal()
    {
        _shopButton.onClick.AddListener(OnShopButtonClick);
        _toolsButton.onClick.AddListener(OnToolsButtonClick);
        _multiplyButton.onClick.AddListener(OnMultiEarningButtonClick);
        _settingsButton.onClick.AddListener(OnSettingClick);

    }

    protected override void UnSubscribeToEventsInternal()
    {
        _shopButton.onClick.RemoveListener(OnShopButtonClick);
        _toolsButton.onClick.RemoveListener(OnToolsButtonClick);
        _multiplyButton.onClick.RemoveListener(OnMultiEarningButtonClick);
        _settingsButton.onClick.RemoveListener(OnSettingClick);
    }

    private void OnEnable()
    {
        SubscribeToEventsInternal();
    }

    private void OnDisable()
    {
        UnSubscribeToEventsInternal();
    }

    private void OnShopButtonClick()
    {
        UISystem.instance.ShowShopMenu();
    }

    private void OnToolsButtonClick()
    {
        UISystem.instance.ShowPreminumResearchMenu();
    }

    private void OnMultiEarningButtonClick()
    {
        UISystem.instance.ShowMultiplyEarningMenu();
    }

    private void OnSettingClick()
    {
        UISystem.instance.ShowSettingMenu();
    }

    private void RefreshMultiplierButton()
    {
        int videoMultiplierTimeLeft = GamePlaySytem.instance.GetVideoMultiplierTimeLeft();

        if(videoMultiplierTimeLeft > 0)
        {
            _multiplierMarkerInactive.SetActive(true);
            _multiplierMarkerActive.SetActive(false);
            _multiplierTimeLeftLabel.text = videoMultiplierTimeLeft.ToShortReadableTime(2);
            GamePlaySytem.instance._doubleMoney = 1.0f;
        }
        else
        {
            _multiplierMarkerInactive.SetActive(false);
            _multiplierMarkerActive.SetActive(true);
            GamePlaySytem.instance._doubleMoney = 0.0f;
        }
    }

    public void Tick()
    {
      
        RefreshMultiplierButton();
    }

    public void RefreshManagerButtons()
    {
         bool hasGeneralManager = GamePlaySytem.instance.marketSystem._view.HasGeneralManager;
        _generalManagerOffButton.gameObject.SetActive(!hasGeneralManager);
        _generalManagerOnButton.gameObject.SetActive(hasGeneralManager);
    }

}
