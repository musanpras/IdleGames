using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class WelcomeBackMenu : MarketMenu
    {
        [SerializeField]
        private CanvasGroup _header;
        [SerializeField]
        private CanvasGroup _body;
        [SerializeField]
        private GameObject _x2Frame;
        [SerializeField]
        private GameObject _x3Frame;
        [SerializeField]
        private TextMeshProUGUI _moneyEarnedLabel;
        [SerializeField]
        private TextMeshProUGUI _moneyUnitLabel;
        [SerializeField]
        private GameObject _videoButton;
    
        [SerializeField]
        private ButtonPriceReactive _premiumReactive;
    
        [SerializeField]
        private GameObject _premiumButton;
    
        [SerializeField]
        private TextMeshProUGUI _premiumAmountLabel;
        [SerializeField]
        private TextMeshProUGUI _premiumLabel;
    
        [SerializeField]
        private TextMeshProUGUI _incomeNormalTimeLabel;
        [SerializeField]
        private TextMeshProUGUI _incomeMaxTimeLabel;
        [SerializeField]
        private TextMeshProUGUI _multipliedLabel;
        [SerializeField]
        private GameObject _normalUserWidget;
    /*
        [SerializeField]
        private Slider _normalSlider;
        [SerializeField]
        private GameObject _premiumUserWidget;
        [SerializeField]
        private Slider _premiumSlider;
        */
        private int _videoStartTimestamp;
      
        private int _timeLapsed;
        private double _moneyEarned;
        private bool _firstMultiplierBought;
        private bool _secondMultiplierBought;
        private double _oldMoney;
        private Color _moneyEarnedBasicColor;
        private Color _moneyUnitBasicColor;
        private Color _moneyEarnedMultipliedColor;
        private Color _moneyUnitMultipliedColor;
        private Color _goldLabelColor;
        private Color _brownLabelColor;

        public void GetFocus()
        {
            base.GetFocus();
            _firstMultiplierBought = false;
            _secondMultiplierBought = false;         
            string format = Language.Get("UI_WELCOMEBACK_MENU_MAX_TIME", "UI");
            format = string.Format(format, 2);
            _incomeNormalTimeLabel.text = format;
           // _incomeMaxTimeLabel.text = format;
            RefreshMaxTimeSlider();
            RefreshWelcomeBackMultiplier();
        }
        public override void LoseFocus()
        {
            base.LoseFocus();
            ShowFlyingResources();
            GamePlaySytem.instance.marketSystem._view.moneyPending = 0.0;
        }
        public void InitializeInternal()
        {
            _moneyEarnedBasicColor = Color.black;
            _moneyUnitBasicColor = Color.black;
            _moneyEarnedMultipliedColor = Color.black;
            _moneyUnitMultipliedColor = Color.black;
            _goldLabelColor = Color.yellow;
            _brownLabelColor = Color.yellow;
             GetFocus();
        }
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.ShowMenu(_header, new CanvasGroup[1]
            {
                 _body
            }, duration);
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.HideMenu(_header, new CanvasGroup[1]
            {
                 _body
            }, duration);
        }
        protected override void SubscribeToEventsInternal()
        {
        }
        protected override void UnSubscribeToEventsInternal()
        {
        }
        public void SetOldMoney(double newOldMoney)
        {
            _oldMoney = newOldMoney;
        }
        public void SetTimeLapsed(int newTimeLapsed)
        {
            _timeLapsed = newTimeLapsed;
        }
        public void SetMoneyEarned(double newMoneyEarned)
        {
            _moneyEarned = newMoneyEarned;
        }
        private void RefreshMaxTimeSlider()
        {
            //_normalSlider.value = (float)Math.Min(_timeLapsed, 7200) * 1f / 36000f;
           // _premiumSlider.value = (float)_timeLapsed * 1f / 36000f;
        }
        private void RefreshWelcomeBackMultiplier()
        {
            SetPremiumPrice();
            RefreshButtonColors();
            if (_firstMultiplierBought && !_secondMultiplierBought)
            {
                _x2Frame.SetActive(value: true);
                _x3Frame.SetActive(value: false);
                _moneyEarnedLabel.color = _moneyEarnedMultipliedColor;
                _moneyUnitLabel.color = _moneyUnitMultipliedColor;
                _videoButton.SetActive(value: false);
                _premiumButton.SetActive(value: false);
                _multipliedLabel.gameObject.SetActive(value: true);
                string format = Language.Get("UI_WELCOMEBACK_MENU_MULTIPLIED_LABEL", "UI");
                _multipliedLabel.text = string.Format(format, "x2");
            }
            else if (_secondMultiplierBought)
            {
                _x2Frame.SetActive(value: false);
                _x3Frame.SetActive(value: true);
                _moneyEarnedLabel.color = _moneyEarnedMultipliedColor;
                _moneyUnitLabel.color = _moneyUnitMultipliedColor;
                _videoButton.SetActive(value: false);
               _premiumButton.SetActive(value: false);
                _multipliedLabel.gameObject.SetActive(value: true);
                string format2 = Language.Get("UI_WELCOMEBACK_MENU_MULTIPLIED_LABEL", "UI");
                _multipliedLabel.text = string.Format(format2, "x3");
             }
            else
            {
                _x2Frame.SetActive(value: false);
                _x3Frame.SetActive(value: false);
                _moneyEarnedLabel.color = _moneyEarnedBasicColor;
                _moneyUnitLabel.color = _moneyUnitBasicColor;
                _videoButton.SetActive(value: true);
                _premiumButton.SetActive(value: true);
                _multipliedLabel.gameObject.SetActive(value: false);
                string format3 = Language.Get("UI_WELCOMEBACK_MENU_PREMIUM_AMOUNT", "UI");
                _premiumAmountLabel.text = string.Format(format3, MarketConfigurationStaticModel.multiplyCost);
            }
            string unitStr = "";
            string text = _moneyEarned.ToLongUnit(out unitStr);
            _moneyEarnedLabel.text = text;
            _moneyUnitLabel.text = unitStr;
        }
        private void SetPremiumPrice()
        {
            _premiumReactive.requiredPrice = MarketConfigurationStaticModel.multiplyCost;
        }
        private void RefreshButtonColors()
        {
        
            if (!_premiumReactive.interactable)
            {
                _premiumLabel.color = Color.white;
                _premiumAmountLabel.color = Color.white;
            }
            else
            {
                _premiumLabel.color = _brownLabelColor;
                _premiumAmountLabel.color = _goldLabelColor;
            }
        
        }
        public void OnMultiplyButtonClick()
        {
       
           GamePlaySytem.instance._saveSystem.Save();
          _videoStartTimestamp = GamePlaySytem.instance._serverTimer.GetServerTimestamp();
           AdsControl.Instance.ShowRewardVideo(AdsControl.ADS_STATE.DOUBLE_REWARD);
         // OnVideoShown(true);
            //_videoSystem.ShowVideo("welcome_back", OnVideoShown);
        }
        public void OnContinueButtonClick()
        {
            if (base.IsVisible)
            {
                UISystem.instance.HideCurrentMenu();
              
                GamePlaySytem.instance.marketSystem._view.money += _moneyEarned;
                GamePlaySytem.instance._saveSystem.Save();
                
            }
        }
        public override void OnNativeBackButtonClick()
        {
            OnContinueButtonClick();
        }
        public override void OnBackButtonClicked()
        {
            OnContinueButtonClick();
        }
        private void FirstMultiplyEarnings()
        {
            if (!_firstMultiplierBought)
            {
                _firstMultiplierBought = true;
                _secondMultiplierBought = false;
                _moneyEarned *= 2.0;
               //  GamePlaySytem.instance.marketSystem._view.money += _moneyEarned;
               //  GamePlaySytem.instance._saveSystem.Save();
                RefreshWelcomeBackMultiplier();
            }
        }
        public void SecondMultiplyEarnings()
        {
            if (!_secondMultiplierBought)
            {
                 GamePlaySytem.instance.marketSystem._view.gems -= MarketConfigurationStaticModel.multiplyCost;
                _firstMultiplierBought = false;
                _secondMultiplierBought = true;
                _moneyEarned *= 3.0;
                // GamePlaySytem.instance.marketSystem._view.money += _moneyEarned;
                // GamePlaySytem.instance._saveSystem.Save();
                 RefreshWelcomeBackMultiplier();
            }
        }
        
       
        public void OnVideoShown(bool success)
        {
            if (!success && _videoStartTimestamp + 15 > GamePlaySytem.instance._serverTimer.GetServerTimestamp())
            {
                OnBackButtonClicked();
            }
            else
            {
                FirstMultiplyEarnings();
            }
        }
        private void ShowFlyingResources()
        {
              //UISystem.instance.Effects.ShowMoneyExplosion();
             // UISystem.instance.Effects.AddMoneyAnimated(_moneyEarned, Vector2.zero);
        }
    }

