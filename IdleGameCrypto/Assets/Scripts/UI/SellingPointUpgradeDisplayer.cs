 using System;
 using UnityEngine;

    public class SellingPointUpgradeDisplayer : MonoBehaviour
    {
        [SerializeField]
        private ButtonPriceReactive _upgradeButton;
        [SerializeField]
        private TextLocalize _titleLabel;
        [SerializeField]
        private TextLocalize _descriptionLabel;
        public Action OnUpgradedAction;

        private GeneratorModel _model;

        public MarketSystem MarketSystem;
        
        protected void SubscribeToEventsInternal()
        {
            _upgradeButton.button.onClick.AddListener(OnUpgradeButtonClicked);
            _upgradeButton.button.onClickReleased.AddListener(OnUpgradeButtonReleased);
        }
        protected void UnSubscribeToEventsInternal()
        {
            _upgradeButton.button.onClick.RemoveListener(OnUpgradeButtonClicked);
            _upgradeButton.button.onClickReleased.RemoveListener(OnUpgradeButtonReleased);
        }
        public void ShowInternal(GeneratorModel element)
        {
             _model = element;
            _titleLabel.key = $"{ element.NameKey}_UPGRADE";
            _descriptionLabel.key = $"{ element.NameKey}_UPGRADE_DESCRIPTION";
            _titleLabel.Localize();
            _descriptionLabel.Localize();
            _upgradeButton.name = "UpgradeProductButton";
            _upgradeButton.requiredPrice = element.GetLevelUpPrice();
            Refresh();
        }
        
        public void Refresh()
        {
            if (_model.CanLevelUp)
            {
                _upgradeButton.ShowAsNormal();
            }
            else
            {
                _upgradeButton.ShowAsMaxReached();
            }
            _upgradeButton.Refresh();
        }
        private void OnUpgradeButtonClicked()
        {
            if (MarketSystem.GetMoney() >= _model.GetLevelUpPrice())
            {
                MarketSystem._view.LevelUpGenerator(_model.id, updateVisuals: false);
                ShowInternal(_model);
                if (OnUpgradedAction != null)
                {
                    OnUpgradedAction();
                }
            }
        }
        private void OnUpgradeButtonReleased()
        {
            UnityEngine.Debug.Log("#Input# Released upgrade button. Refreshing visuals.");
            // MarketSystem.RefresGeneratorVisual(_model.id);
        }

        public void Initialize()
        {

          SubscribeToEventsInternal();

        }
    }






