using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class MultiplyEarningsMenu : MarketMenu
    {
        [SerializeField]
        private TextMeshProUGUI _descriptionLabel;
        [SerializeField]
        private TextMeshProUGUI _maxDurationLabel;
        [SerializeField]
        private TextMeshProUGUI _timeLeftLabel;
        [SerializeField]
        private MarketButton _watchMoreButton;
       // [SerializeField]
       // private TextMeshProUGUI _comebackLabel;
       // [SerializeField]
        //private TextMeshProUGUI _nextLabel;
        [SerializeField]
        private CanvasGroup _headerArea;
        [SerializeField]
        private CanvasGroup _descArea;
        [SerializeField]
        private CanvasGroup _videoArea;
       // [SerializeField]
       // private TextMeshProUGUI timeLabels;
       
        private int _videoStartTimestamp;
        private Color _barColorFilled;
        private Color _barColorEmpty;
        private Color _textColor;
       
        public void InitializeInternal()
        {
       // Debug.Log("MULTI INIT");
            _barColorFilled = Color.green;
            _barColorEmpty = Color.white;
            _textColor = Color.gray;
             GetFocus();
        }
        public void GetFocus()
        {

            _descriptionLabel.text = string.Format(Language.Get("UI_MULTIPLY_EARNINGS_DESC", "UI"), MarketConfigurationStaticModel.videoPowerUp, GamePlaySytem.instance.GetVideoBonusDuration().ToShortReadableTime(2));
            _maxDurationLabel.text = string.Format(Language.Get("UI_MULTIPLY_MAX_TIME", "UI"), GamePlaySytem.instance.GetMaxVideoPowerUpDuration().ToShortReadableTime(2));         
            _watchMoreButton.gameObject.SetActive(value: true);
           
        }
      
        
        public void OnWatchMoreButtonClick()
        {
            //_saveSystem.LocalSave();
            _videoStartTimestamp = GamePlaySytem.instance._serverTimer.GetServerTimestamp();
            AdsControl.Instance.ShowRewardVideo(AdsControl.ADS_STATE.MULTI_EARNING);
            // OnVideoShown(true);
            //_videoSystem.ShowVideo("boost", OnVideoShown);
        }
        
        public void OnVideoShown(bool success)
        {
            if (!success && _videoStartTimestamp + 15 > GamePlaySytem.instance._serverTimer.GetServerTimestamp())
            {
                OnBackButtonClicked();
            }
            else
            {
            GamePlaySytem.instance.AddVideoPowerUp();
            }
        }
        private void Update()
        {
            if (canvas.isActiveAndEnabled)
            {
                int value = 0;
                int finishVideoPowerUpTimestamp = GamePlaySytem.instance.marketSystem._view.finishVideoPowerUpTimer;
                int serverTimestamp = GamePlaySytem.instance._serverTimer.GetServerTimestamp();
                if (finishVideoPowerUpTimestamp > serverTimestamp)
                {
                    value = finishVideoPowerUpTimestamp - serverTimestamp;
                }
                _timeLeftLabel.text = value.ToShortReadableTime();
            }
        }
      
        
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.ShowMenu(_headerArea, new CanvasGroup[2]
            {
                 _descArea,
                 _videoArea
            }, duration);
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.HideMenu(_headerArea, new CanvasGroup[2]
            {
                 _descArea,
                 _videoArea
            }, duration);
        }
        protected override void SubscribeToEventsInternal()
        {
            _watchMoreButton.onClick.AddListener(OnWatchMoreButtonClick);
            
        }
        protected override void UnSubscribeToEventsInternal()
        {
            _watchMoreButton.onClick.RemoveListener(OnWatchMoreButtonClick);
           
        }
       
    }

