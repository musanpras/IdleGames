using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class SettingsMenu : MarketMenu
    {
        [SerializeField]
        private CanvasGroup _header;
        [SerializeField]
        private CanvasGroup _optionsWidget;
       
        [SerializeField]
        private CanvasGroup _supportWidget;
  
        [SerializeField]
        private TextMeshProUGUI _versionLabel;
    
        [SerializeField]
        private CheckboxButton _musicCheckbox;
        [SerializeField]
        private CheckboxButton _sfxCheckbox;

       [SerializeField]
        private AudioSystem _audioSystem;

        private static Color _authenticatedColor;

        private static Color _notAuthenticatedColor;

        private static bool _isStaticInitialized;
        public  void GetFocus()
        {
           
            _versionLabel.text = "v" + Application.version;
            _musicCheckbox.Show();
            _sfxCheckbox.Show();
            RefreshSocialStatus();
            RefreshCheckbox(_musicCheckbox, AudioSystem.ECategory.Music);
            RefreshCheckbox(_sfxCheckbox, AudioSystem.ECategory.Fx);
        }
        public void InitializeInternal()
        {
            _authenticatedColor = new Color32(0, 193, 68, byte.MaxValue);
            _notAuthenticatedColor = new Color32(87, 85, 112, byte.MaxValue);
            GetFocus();
        }
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.ShowMenu(_header, new CanvasGroup[2]
            {
                 _optionsWidget,       
                 _supportWidget
            }, duration);
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.HideMenu(_header, new CanvasGroup[2]
            {
                 _optionsWidget,          
                 _supportWidget
            }, duration);
        }
        protected override void SubscribeToEventsInternal()
        {
        }
        protected override void UnSubscribeToEventsInternal()
        {
            UnsubscribeAppStorePurchase();
        }
        private void RefreshSocialStatus()
        {
            
        }
        public void OnSocialConnectedButtonClick()
        {
            OnSocialConnectButtonClick();
        }
        public void OnSocialConnectButtonClick()
        {
            
        }
        private void LogOut()
        {
            
        }
        public void CheckLink()
        {
            
        }
        public void OnContactButtonClick()
        {
        //UISystem.instance.Popups.ShowFAQPopup();
        Application.OpenURL("mailto:" + "ponystudio92@gmail.com" + "?subject=" + "your questions" + "&body=" + "Dear ponygames! ....");
    }
        private string EscapeURL(string url)
        {
            return WWW.EscapeURL(url).Replace("|+", "% 20");
        }
        public void OnMusicCheckboxClick()
        {
            ToggleCheckbox(_musicCheckbox, AudioSystem.ECategory.Music);
        }
        public void OnSfxCheckboxClick()
        {
            ToggleCheckbox(_sfxCheckbox, AudioSystem.ECategory.Ui);
        }
        private bool ToggleCheckbox(CheckboxButton soundCheckbox, AudioSystem.ECategory category)
        {
            bool num = _audioSystem.IsMute(category);
            if (num)
            {
                _audioSystem.Unmute(category);
            }
            else
            {
                _audioSystem.Mute(category);
            }
            RefreshCheckbox(soundCheckbox, category);
            return num;
        }
        private void RefreshCheckbox(CheckboxButton soundCheckbox, AudioSystem.ECategory category)
        {
            if (_audioSystem.IsMute(category))
            {
                soundCheckbox.Deselect();
            }
            else
            {
                soundCheckbox.Select();
            }
        }
        public void OnRestorePurchasesButtonClick()
        {
        IAPManager.instance.RestorePurchases();
        }
        private void UnsubscribeAppStorePurchase()
        {
        }
        private void OnPurchaseSuccess(string sku)
        {
            
        }
    }
