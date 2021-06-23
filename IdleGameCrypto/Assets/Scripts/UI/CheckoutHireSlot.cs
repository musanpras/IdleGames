using System;
using TMPro;
using UnityEngine;

    public class CheckoutHireSlot : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField]
        private Color _unlockedColor;
        [SerializeField]
        private Color _lockedColor;
        [Header("Refences")]
        [SerializeField]
        private ButtonPriceReactive _button;
        [SerializeField]
        private TextMeshProUGUI _descriptionLabel;
        private GeneratorStaticModel _model;
        private Action<GeneratorStaticModel> _hireCallback;

        private bool reachRequirement;
        
        public void SetModel(GeneratorStaticModel model, Action<GeneratorStaticModel> hireCallback)
        {
            _model = model;
            reachRequirement = true;
            _hireCallback = hireCallback;
            _descriptionLabel.text = Language.Get("CHECKOUT_HIRE_DESCRIPTION", "UI");
            _descriptionLabel.color = _unlockedColor;
            _button.button.onClick.RemoveAllListeners();
            _button.button.onClick.AddListener(delegate
            {
                _hireCallback(_model);
            });
            Refresh();
        }
        public void Refresh()
        {
       
            if (_model.id != null && reachRequirement)
            {
                _button.requiredPrice = _model.build_cost;
                _button.Refresh();
            }
        
            
        }
        public void SetRequirementNotMet(GeneratorStaticModel nextCashierRequirement, bool _reach)
        {
        
              reachRequirement = _reach;
             _model = nextCashierRequirement;
            _button.button.onClick.RemoveAllListeners();
            _button.requiredPrice = nextCashierRequirement.build_cost;
            _descriptionLabel.text = string.Format(Language.Get("CHECKOUT_HIRE_NOT_MET", "UI"), Language.Get(nextCashierRequirement.requirement, "SHOPS"));
            _descriptionLabel.color = _lockedColor;
            _button.button.interactable = false;
        }
    }






