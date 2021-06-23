using System;
using UnityEngine;

   public class ParkingUpgradeDisplayer : MonoBehaviour
    {
        [SerializeField]
        private ButtonPriceReactive _upgradeButton;

        public Action OnUpgradedAction;

        private ParkingVisualModel _parking;

        public MarketSystem MarketSystem;
        
        protected  void SubscribeToEventsInternal()
        {

        _upgradeButton.button.onClick.AddListener(OnUpgradeButtonClicked);
       
        }
        public  void UnSubscribeToEventsInternal()
        {
            _upgradeButton.button.onClick.RemoveListener(OnUpgradeButtonClicked);
        }
        public  void ShowInternal(ParkingVisualModel element)
        {
        
            _parking = element;
            _upgradeButton.name = "UpgradeParkingButton";
            _upgradeButton.requiredPrice = element.GetCostForUpgrade();
            Refresh();
            
        }
        public void Refresh()
        {
        
            if (_parking.GetCostForUpgrade() < double.MaxValue)
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

        if (MarketSystem.GetMoney() >= _parking.GetCostForUpgrade())
            {
           
               MarketSystem._view.BuyEmployeeToGenerator(_parking.id);
              _parking = MarketSystem._view._parkingVisualModel;
               ShowInternal(_parking);
          
                if (OnUpgradedAction != null)
                {
                    OnUpgradedAction();
                }
            }
            
        }

       public void Initialize()
       {
        SubscribeToEventsInternal();
        
       }
}






