using System;
using UnityEngine;
using UnityEngine.UI;


    public class SellingPointHireEmployeesDisplayer : MonoBehaviour
    {
        [SerializeField]
        private ButtonPriceReactive _hireButton;
        [SerializeField]
        private Image _icon;
        public Action OnPersonalHiredAction;

        private GeneratorModel _model;
    
      
        public void ShowInternal(GeneratorModel element)
        {
            _model = element;
            _hireButton.requiredPrice = element.GetCostNextEmployee();
            _icon.sprite = GamePlaySytem.instance._atlas.GetSprite("icon_add_" + element.NameKey.ToLower() + "_clerk");
        }
        public  void Refresh()
        {
            if (_model.CanBuyEmployees)
            {
                _hireButton.ShowAsNormal();
            }
            else
            {
                _hireButton.ShowAsMaxReached();
            }
            _hireButton.Refresh();
        }
        protected void SubscribeToEventsInternal()
        {
            _hireButton.button.onClick.AddListener(OnHireButtonClicked);
        }
        protected void UnSubscribeToEventsInternal()
        {
            _hireButton.button.onClick.RemoveListener(OnHireButtonClicked);
        }
        private void OnHireButtonClicked()
        {
           
            GamePlaySytem.instance.marketSystem._view.BuyEmployeeToGenerator(_model.id);
            ShowInternal(_model);
            if (OnPersonalHiredAction != null)
            {
                OnPersonalHiredAction();
            }
        }


        public void Initialize()
        {
            //_model = element;
            SubscribeToEventsInternal();

        }

      
    }

