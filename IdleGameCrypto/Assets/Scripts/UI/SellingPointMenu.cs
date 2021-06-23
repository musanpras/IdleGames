using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SellingPointMenu : MarketMenu
{

    [Space]
    [SerializeField]
    private CanvasGroup _headerArea;

    [SerializeField]
    private CanvasGroup _currentLevelArea;

    [SerializeField]
    private CanvasGroup _upgradeArea;

    [SerializeField]
    private CanvasGroup _hireEmployeeArea;

    [SerializeField]
    private CanvasGroup _productListArea;

    [SerializeField]
    private CanvasGroup _researchArea;

    [SerializeField]
    private CanvasGroup _footerArea;

    [SerializeField]
    private TextLocalize _titleLabel;

    [Header("Pagination")]
    [SerializeField]
    private Button _rightArrow;

    [SerializeField]
    private Button _leftArrow;

    private bool _isPaginating;

    [Header("Displayers")]
    [SerializeField]
    private SellingPointCurrentLevelDisplayer _currentLevelDisplayer;
    [SerializeField]
    private SellingPointUpgradeDisplayer _upgradeDisplayer;
    [SerializeField]
    private SellingPointHireEmployeesDisplayer _personalDisplayer;
    [SerializeField]
    private OfficeManagerResearchDisplayer _researchDisplayer;


    public GeneratorModel Model
    {
        get;
        private set;
    }

    protected override Sequence HideAnimatedInternal(float duration = 0.2F)
    {
        return UIAnimations.HideMenu(_headerArea, new CanvasGroup[6]
        {
            _currentLevelArea,
            _upgradeArea,
            _hireEmployeeArea,
            _productListArea,
            _researchArea,
            _footerArea
        }, duration);
    }

    protected override Sequence ShowAnimatedInternal(float duration = 0.2F)
    {
        return UIAnimations.ShowMenu(_headerArea, new CanvasGroup[6]
        {
            _currentLevelArea,
            _upgradeArea,
            _hireEmployeeArea,
            _productListArea,
            _researchArea,
            _footerArea

        }, duration);
    }

    protected override void SubscribeToEventsInternal()
    {
        /*
        MessageBus.Subscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsedEvent);
        _upgradeDisplayer.OnUpgradedAction = OnLevelUpgraded;
        _personalDisplayer.OnPersonalHiredAction = OnPersonalHired;
        _leftArrow.onClick.AddListener(OnPaginateToLeft);
        _rightArrow.onClick.AddListener(OnPaginateToRight);
        _infoButton.onClick.AddListener(OnInfoButtonClicked);
        _overlayBackground.onClick.AddListener(OnOverlayClicked);
        _researchDisplayer.OnPurchased.AddListener(OnResearchPurchased);
        */
        _upgradeDisplayer.OnUpgradedAction = OnLevelUpgraded;
        _personalDisplayer.OnPersonalHiredAction = OnPersonalHired;
        _researchDisplayer.OnPurchased.AddListener(OnResearchPurchased);
    }
    protected override void UnSubscribeToEventsInternal()
    {
        /*
        MessageBus.UnSubscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsedEvent);
        _upgradeDisplayer.OnUpgradedAction = null;
        _personalDisplayer.OnPersonalHiredAction = null;
        _leftArrow.onClick.RemoveListener(OnPaginateToLeft);
        _rightArrow.onClick.RemoveListener(OnPaginateToRight);
        _infoButton.onClick.RemoveListener(OnInfoButtonClicked);
        _overlayBackground.onClick.RemoveListener(OnOverlayClicked);
        _researchDisplayer.OnPurchased.RemoveListener(OnResearchPurchased);
        */
        _upgradeDisplayer.OnUpgradedAction = null;
        _personalDisplayer.OnPersonalHiredAction = null;
        _researchDisplayer.OnPurchased.RemoveListener(OnResearchPurchased);
    }

    public void InitializeInternal()
    {
        /*
        _currentLevelDisplayer.Initialize();
        _upgradeDisplayer.Initialize();
        _personalDisplayer.Initialize();
        _listDisplayer.Initialize();
        _researchDisplayer.Initialize();
        HideOverlay(0f);
        */
        _upgradeDisplayer.Initialize();
        _personalDisplayer.Initialize();
        _researchDisplayer.Initialize();
        
        //_researchDisplayer.Initialize(Model);
    }

    private void OnEnable()
    {
        SubscribeToEventsInternal();
    }

    private void OnDisable()
    {
        UnSubscribeToEventsInternal();
    }

    private void OnLevelUpgraded()
    {
        _currentLevelDisplayer.ShowInternal(Model);
    }

    private void OnPersonalHired()
    {
        _currentLevelDisplayer.ShowInternal(Model);
    }

    private void OnResearchPurchased()
    {
        _currentLevelDisplayer.ShowInternal(Model);
    }

    public void Initialize()
    {
        /*
        if(!IsInitialized)
        {
            base.Initialize();

            if (StartsHidden)
                HideAnimated(0f);
            IsInitialized = true;
        }
        */
       
    }

    public void AsssociateWith(GeneratorModel generator)
    {
        Model = generator;
        _titleLabel.key = generator.id;
        _titleLabel.sheet = "SHOPS";
        _titleLabel.Localize();
        _currentLevelDisplayer.ShowInternal(generator);
        _upgradeDisplayer.ShowInternal(generator);
        _personalDisplayer.ShowInternal(generator);
        _researchDisplayer.ShowInternal(generator);
    }

    private void Update()
    {
        _upgradeDisplayer.Refresh();
        _personalDisplayer.Refresh();
    }
}
