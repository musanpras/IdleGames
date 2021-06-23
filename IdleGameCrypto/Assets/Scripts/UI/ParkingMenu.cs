using DG.Tweening;
using UnityEngine;

    public class ParkingMenu : MarketMenu
    {
        [Space]
        [SerializeField]
        private CanvasGroup _headerArea;
        [SerializeField]
        private CanvasGroup _currentLevelArea;
        [SerializeField]
        private CanvasGroup _upgradeArea;
        [SerializeField]
        private CanvasGroup _researchArea;
        [SerializeField]
        private TextLocalize _titleLabel;
        [Header("Displayers")]
        [SerializeField]
        private ParkingCurrentLevelDisplayer _currentLevelDisplayer;
        [SerializeField]
        private ParkingUpgradeDisplayer _upgradeDisplayer;
        [SerializeField]
        private ParkingResearchDisplayer _parkingResearch;

        private bool _isPaginating;

        public MarketSystem MarketSystem;

    

    public ParkingVisualModel Parking
        {
            get;
            private set;
        }
        
        public void AssociateWith(ParkingVisualModel parking)
        {

             InitializeInternal();

             Parking = parking;
            _titleLabel.key = parking.id;
            _titleLabel.sheet = "SHOPS";
            _titleLabel.Localize();
            _currentLevelDisplayer.ShowInternal(parking);
             _upgradeDisplayer.ShowInternal(parking);
            _parkingResearch.ShowInternal(parking);
            
        }
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.ShowMenu(_headerArea, new CanvasGroup[3]
            {
                 _currentLevelArea,
                 _upgradeArea,
                 _researchArea
            }, duration);
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.HideMenu(_headerArea, new CanvasGroup[3]
            {
                 _currentLevelArea,
                 _upgradeArea,
                 _researchArea
            }, duration);
        }
        protected override void SubscribeToEventsInternal()
        {
            
            _upgradeDisplayer.OnUpgradedAction = OnLevelUpgraded;
        }
        protected override void UnSubscribeToEventsInternal()
        {
            
            _upgradeDisplayer.OnUpgradedAction = null;
            _parkingResearch.UnSubscribeToEventsInternal();
            _upgradeDisplayer.UnSubscribeToEventsInternal();
        }

    private void Update()
    {
        _upgradeDisplayer.Refresh();
        _currentLevelDisplayer.Refresh();
        _parkingResearch.Refresh();
    }
    private void OnLevelUpgraded()
        {
        _currentLevelDisplayer.ShowInternal(Parking);
        }


    public override void Initialize()
    {
        
        if (!IsInitialized)
        {
            base.Initialize();

            if (StartsHidden)
                HideAnimated(0f);
            IsInitialized = true;
        }
    }
    
    protected void InitializeInternal()
    {
        _currentLevelDisplayer.Initialize();
        _upgradeDisplayer.Initialize();
        _parkingResearch.Initialize();
    }

    
}
