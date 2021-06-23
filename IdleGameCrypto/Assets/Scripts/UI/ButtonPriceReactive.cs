using TMPro;
using UnityEngine;

    public class ButtonPriceReactive : MonoBehaviour
    {
        public enum Currency
        {
            Normal,
            Premium
        }
      
        private MarketSystem _marketSystem;
    
        //private UserController _userController;
        [SerializeField]
        private TextMeshProUGUI _textPrice;
        [SerializeField]
        public MarketButton button;
        [SerializeField]
        private GameObject _normalArea;
        [SerializeField]
        private GameObject _maxArea;
        private double _price;
        private bool _isMaxMode;
        [SerializeField]
        private Currency _currency;
        public bool interactable
        {
            get;
            private set;
        }
        public double requiredPrice
        {
            set
            {
                _price = value;
                SetPrice();
            }
        }
        public void Refresh()
        {
        if (((_currency == Currency.Normal) ? GamePlaySytem.instance.marketSystem.GetMoney() : ((double)GamePlaySytem.instance.marketSystem._view.gems)) < _price || _isMaxMode)
        {
            button.interactable = false;
            interactable = false;
        }
        else
        {
            button.interactable = true;
            interactable = true;
        }
    }
        public void ShowAsMaxReached()
        {
            _isMaxMode = true;
            _maxArea.SetActive(value: true);
            _normalArea.SetActive(value: false);
            button.interactable = false;
        }
        public void ShowAsNormal()
        {
            _isMaxMode = false;
            _maxArea.SetActive(value: false);
            _normalArea.SetActive(value: true);
            Refresh();
        }
        private void SetPrice()
        {
            string unitStr = "";
            _textPrice.text = ((_currency == Currency.Normal) ? _price.ToShortUnitWithUnits() : _price.ToShortNoDecimalsUnit(out unitStr));
            Refresh();
        }
    }
