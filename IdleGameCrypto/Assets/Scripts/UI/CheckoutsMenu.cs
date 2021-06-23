using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class CheckoutsMenu : MarketMenu
    {
        [SerializeField]
        private MarketSystem _marketSystem;
        [SerializeField]
        private CheckoutSystem _checkoutSystem;
        [SerializeField]
        private TextMeshProUGUI _labelTotalCashiers;
        [SerializeField]
        private TextMeshProUGUI _labelClientsPerMinute;
        [SerializeField]
        private CanvasGroup _title;
        [SerializeField]
        private CanvasGroup _header;
        [SerializeField]
        private List<CanvasGroup> _animatedSlots;
        [SerializeField]
        private CheckoutUpgradeSlot _upgradeSlotPrefab;
        [SerializeField]
        private CheckoutHireSlot _hireSlotPrefab;
        [SerializeField]
        private VerticalLayoutGroup _verticalLayout;
        private bool isPopulated;
        private CheckoutUpgradeSlot[] _upgradeSlots;
        public void Populate()
        {
            if (!isPopulated)
            {
                _upgradeSlots = new CheckoutUpgradeSlot[MarketConfigurationStaticModel.maxCashiers];
                for (int i = 0; i < MarketConfigurationStaticModel.maxCashiers; i++)
                {
                    Transform transform = Object.Instantiate(_upgradeSlotPrefab.transform, _upgradeSlotPrefab.transform.parent);
                    //_container.InjectGameObject(transform.gameObject);
                    _upgradeSlots[i] = transform.GetComponent<CheckoutUpgradeSlot>();
                    transform.GetComponent<CanvasGroup>().alpha = 0f;
                }
                isPopulated = true;
            }
            _upgradeSlotPrefab.gameObject.SetActive(value: false);
            _hireSlotPrefab.transform.SetAsLastSibling();
            _hireSlotPrefab.GetComponent<CanvasGroup>().alpha = 0f;
            _upgradeSlotPrefab.transform.SetAsLastSibling();
            RefreshData();
        }
        private void RefreshData(bool _forceSlotsVisible = false)
        {
            int currentMaxCashiers = _checkoutSystem.GetCurrentMaxCashiers();
            int currentCountCashiers = _checkoutSystem.GetCurrentCountCashiers();
            Debug.Log("max " + currentMaxCashiers + " current" + currentCountCashiers);
            _animatedSlots.Clear();
            _labelTotalCashiers.text = string.Format(Language.Get("CHECKOUT_NUMBER", "UI"), currentCountCashiers, currentMaxCashiers);
            float num = 0f;
            for (int i = 0; i < currentCountCashiers; i++)
            {
               
                CheckoutModel generatorModel = GamePlaySytem.instance.marketSystem._view.checkOutSystem.checkOutList[i];
                num += (float)generatorModel.GetCurrentProduction();
                _upgradeSlots[i].gameObject.SetActive(value: true);
                _upgradeSlots[i].SetModel(GamePlaySytem.instance.marketSystem._view.checkOutSystem.checkOutList[i], UpgradeClicked, UpgradeReleased);
                if (_forceSlotsVisible)
                {
                    _upgradeSlots[i].GetComponent<CanvasGroup>().alpha = 1f;
                }
                _animatedSlots.Add(_upgradeSlots[i].GetComponent<CanvasGroup>());
                
                
            }
            for (int j = currentCountCashiers; j < MarketConfigurationStaticModel.maxCashiers; j++)
            {
                _upgradeSlots[j].gameObject.SetActive(value: false);
            }
            if (currentCountCashiers < currentMaxCashiers)
            {
                _hireSlotPrefab.gameObject.SetActive(value: true);
            //  Debug.Log("BUILD COST " + GamePlaySytem.instance.marketSystem._view.checkOutSystem.checkOutList[currentCountCashiers].id);
                GeneratorStaticModel _checkout = new GeneratorStaticModel();
                _checkout.id = GamePlaySytem.instance._generatorStaticModels["CHECKOUT" + (currentCountCashiers + 1).ToString()].id;
                _checkout.build_cost = GamePlaySytem.instance._generatorStaticModels["CHECKOUT" + (currentCountCashiers + 1).ToString()].build_cost;
                _checkout.requirement = GamePlaySytem.instance._generatorStaticModels["CHECKOUT" + (currentCountCashiers + 1).ToString()].requirement;
                _hireSlotPrefab.SetModel(_checkout, HireClicked);
                _animatedSlots.Add(_hireSlotPrefab.GetComponent<CanvasGroup>());
            }
            else
            {
               
               _hireSlotPrefab.gameObject.SetActive(value: false);
               GeneratorStaticModel firstRequerimentForNextCheckout = new GeneratorStaticModel();
               firstRequerimentForNextCheckout.id = _checkoutSystem.GetFirstRequerimentForNextCheckout().id;
               firstRequerimentForNextCheckout.build_cost = _checkoutSystem.GetFirstRequerimentForNextCheckout().build_cost;
               firstRequerimentForNextCheckout.requirement = _checkoutSystem.GetFirstRequerimentForNextCheckout().requirement;
               

               if (firstRequerimentForNextCheckout.id == null)
                {
                    _hireSlotPrefab.gameObject.SetActive(value: false);
                }
                else
                {
                    bool _reach = false;
                    if (GamePlaySytem.instance.marketSystem._view.generatorControllers[firstRequerimentForNextCheckout.requirement].level > 0)
                    _reach = true;
                     _hireSlotPrefab.gameObject.SetActive(value: true);
                    _hireSlotPrefab.SetRequirementNotMet(firstRequerimentForNextCheckout, _reach);
                    _animatedSlots.Add(_hireSlotPrefab.GetComponent<CanvasGroup>());
                }
                
        }
            _labelClientsPerMinute.text = string.Format(Language.Get("CM_PER_MIN", "COMMON"), Mathf.FloorToInt(num));
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(UIAnimations.HideMenu(_title, new CanvasGroup[1]
            {
                 _header
            }, duration));
            float num = 0.1f;
            foreach (CanvasGroup animatedSlot in _animatedSlots)
            {
                if (animatedSlot.gameObject.activeSelf)
                {
                    sequence.Insert(num, UIAnimations.HideVerticalOut(animatedSlot, duration));
                    num += 0.05f;
                }
            }
            return sequence;
        }
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(UIAnimations.ShowMenu(_title, new CanvasGroup[1]
            {
                 _header
            }, duration));
            _marketSystem.StartCoroutine(AnimateShowSlots(duration));
            return sequence;
        }
        private IEnumerator AnimateShowSlots(float duration)
        {
            while (!base.gameObject.activeInHierarchy)
            {
                yield return null;
            }
            _verticalLayout.gameObject.SetActive(value: false);
            yield return new WaitForSeconds(0.05f);
            _verticalLayout.gameObject.SetActive(value: true);
            yield return null;
            LayoutRebuilder.ForceRebuildLayoutImmediate(_verticalLayout.transform.parent.GetComponent<RectTransform>());
            yield return null;
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            float num = 0.05f;
            foreach (CanvasGroup animatedSlot in _animatedSlots)
            {
                sequence.Insert(num, UIAnimations.ShowGroupFadeIn(animatedSlot, duration));
                num += 0.05f;
            }
        }
        private void UpgradeClicked(CheckoutModel model)
        {
            _marketSystem._view.LevelUpGenerator(model.id, updateVisuals: false);
            RefreshData();
        }
        private void UpgradeReleased(CheckoutModel model)
        {
          //  _marketSystem.RefresGeneratorVisual(model.id);
        }
        private void HireClicked(GeneratorStaticModel model)
        {
               _marketSystem._view.UpgradeCheckout(model.id);
          //  _marketSystem.LevelUpGenerator(model.id);
             RefreshData(_forceSlotsVisible: true);
        }
        protected override void SubscribeToEventsInternal()
        {
           // _messageBus.Subscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsedEvent);
        }
        protected override void UnSubscribeToEventsInternal()
        {
            //_messageBus.UnSubscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsedEvent);
        }
   

    private void Update()
    {
        CheckoutUpgradeSlot[] upgradeSlots = _upgradeSlots;
        foreach (CheckoutUpgradeSlot checkoutUpgradeSlot in upgradeSlots)
        {
            if (checkoutUpgradeSlot.gameObject.activeInHierarchy)
            {
                checkoutUpgradeSlot.Refresh();
            }
        }
        if (_hireSlotPrefab.gameObject.activeInHierarchy)
        {
            _hireSlotPrefab.Refresh();
        }
    }

}






