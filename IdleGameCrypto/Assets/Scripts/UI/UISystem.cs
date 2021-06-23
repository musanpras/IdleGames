using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using ToonyColorsPro;
using UnityEngine;

public class UISystem : MonoBehaviour
{
    public static UISystem instance;

    public EmployeesProgressSystem _employeesProgressSystem;

    [SerializeField]
    private SellingPointMenu _sellingPointMenu;

    [SerializeField]
    private ParkingMenu _parkingMenu;

    [SerializeField]
    private MainMenu _mainMenu;

    [SerializeField]
    private ShopMenu _shopMenu;

    [SerializeField]
    private SettingsMenu _settingMenu;

    [SerializeField]
    private PremiumResearchesMenu _preminumResearchMenu;

    [SerializeField]
    private MultiplyEarningsMenu _multiplyEarningsMenu;

    [SerializeField]
    private WelcomeBackMenu welcomeBackMenu;

    [SerializeField]
    private CheckoutsMenu _checkoutsMenu;

    [SerializeField]
    private OfficeManagerMenu _officeManagerMenu;

    [SerializeField]
    private MenusBackgroundController _backgroundController;

    public TextMeshProUGUI _moneyLabel;

    public TextMeshProUGUI _moneyUnitLabel;

    public TextMeshProUGUI _profitsPerMinuteLabel;

    public TextMeshProUGUI _preminumLabel;

    [SerializeField]
    public PopUpSystem _popupSystem;

    public bool IsMenuTransitioning
    {
        get;
        private set;
    }

    public MarketMenu CurrentMenu
    {
        get;
        private set;
    }

    public bool HasMenuVisible
    {
        get
        {
            if( CurrentMenu == null || !CurrentMenu.IsVisible)
            {
                return IsMenuTransitioning;
            }
            return true;
        }
    }

    private void Awake()
    {
        instance = this;
       
    }

    public void Initialize()
    {
        _backgroundController.Initialize();
        _sellingPointMenu.InitializeInternal();
        _mainMenu.Initialize();
        _shopMenu.InitializeInternal();
        _preminumResearchMenu.InitializeInternal();
        _mainMenu.InitializeInternal();
        _multiplyEarningsMenu.InitializeInternal();
        _settingMenu.InitializeInternal();
        welcomeBackMenu.InitializeInternal();
        //HideMainMenu(0f);
        _backgroundController.Hide(0f);
        _employeesProgressSystem.Init();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpgradeFruit()
    {
        //MarketSystem._instance._view.UpgradeFruit();
    }

    public void UpgradeCheckout()
    {
       // MarketSystem._instance._view.UpgradeCheckout();
    }

    public void BuildBakery()
    {
        //MarketSystem._instance._view.BuildBakery();
    }

    public void UpgradeParking()
    {
        MarketSystem._instance._view.UpgradeParking();
    }

    public void ShowSellingPointMenu(GeneratorModel component)
    {
        _sellingPointMenu.AsssociateWith(component);
        ShowMenu(_sellingPointMenu);
        
    }

    private void ShowMenu(MarketMenu menu, MenusBackgroundController.Theme theme = MenusBackgroundController.Theme.Light)
    {
        AudioSystem._instance.Play("ui_open_popup");
        if(CurrentMenu != null)
        TransitionToMenu(menu);
        else if(!menu.isVisible)
        {
            HideMainMenu();
            Sequence sequence = DOTween.Sequence();
            IsMenuTransitioning = true;
            sequence.SetId("UI");
            IsMenuTransitioning = true;
            _backgroundController.Show(theme);
            sequence.AppendInterval(0.1f);

            sequence.AppendCallback
                (delegate
                {
                    menu.SubscribeToEvents();
                    menu.gameObject.SetActive(true);
                    _backgroundController.Transition(theme);
                }

                );
            sequence.Append(menu.ShowAnimated());
            sequence.AppendCallback
                (delegate
                {

                IsMenuTransitioning = false;
                }
                );

            CurrentMenu = menu;
            //AdsControl.Instance.ShowBanner();
        }

    }

    public double CalculateProfits()
    {
        double _num = 0.0f;
        for(int i = 0; i < GamePlaySytem.instance.marketSystem._view.generatorControllers.Count; i++)
        {
            if(GamePlaySytem.instance.marketSystem._view.generatorControllers.ElementAt(i).Value.level > 0)
            {

                _num += GamePlaySytem.instance.marketSystem._view.generatorControllers.ElementAt(i).Value.TotalProductionPerMinute;
            }
        }
        return _num;
    }

    public void HideCurrentMenu()
    {
        if (CurrentMenu != null)
            HideMenu(CurrentMenu);  
    }

    public void HideMenu(MarketMenu menu, MenusBackgroundController.Theme theme = MenusBackgroundController.Theme.Light)
    {
        if(!IsMenuTransitioning && menu.IsVisible)
        {
            AudioSystem._instance.Play("ui_close_popup");
            Sequence sequence = DOTween.Sequence();
            IsMenuTransitioning = true;
            sequence.SetId("UI");
            sequence.AppendCallback(delegate
            {
               // _worldCamera.enabled = true;
               // _worldCameraMovement.enabled = true;
                menu.UnSubscribeToEvents();
                menu.HideAnimated();
            });
            sequence.AppendInterval(0.1f);
            sequence.AppendCallback(delegate
            {
                _backgroundController.Hide(theme);
                ShowMainMenu();
            });
            sequence.AppendInterval(_backgroundController.TransitionDuration);
            sequence.AppendCallback(delegate
            {
                menu.gameObject.SetActive(value: false);
                menu.LoseFocus();
                CurrentMenu = null;
                IsMenuTransitioning = false;
                //SelectSystem.Filter(SelectController.eType.NONE);
                //_messageBus.Notify<MenuClosedEvent>(new MenuClosedEvent(menu));
            });
           // AdsControl.Instance.HideBanner();
        }
    }

    private void TransitionToMenu(MarketMenu menu)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetId("UI");
        IsMenuTransitioning = true;
        sequence.Append(CurrentMenu.HideAnimated());
        sequence.AppendCallback(delegate

            {
            menu.gameObject.SetActive(true);
            }
            );

        sequence.Append(menu.ShowAnimated());
        sequence.AppendCallback(delegate

        {
            IsMenuTransitioning = false;
        }
        );

        sequence.Play();
    }

    public void HideMainMenu(float duration = 0.2f)
    {
        Sequence sequence = DOTween.Sequence();
        sequence.SetId("UI");
        sequence.Append(_mainMenu.HideAnimated());
        sequence.AppendCallback
            (
            delegate
            {
                _mainMenu.gameObject.SetActive(false);
                //Debug.Log("MAIN MENU HIDE");
            }
            );
    }

    public void ShowMainMenu(float duration = 0.2f)
    {

        Sequence sequence = DOTween.Sequence();
        sequence.SetId("UI");
        sequence.AppendCallback(delegate
        {
            _mainMenu.gameObject.SetActive(true);
           // Debug.Log("MAIN MENU SHOW");
        });

        sequence.Append(_mainMenu.ShowAnimated(duration));
       
        sequence.Play();
    }

    public void ShowParkingMenu(ParkingVisualModel _parkingModel)
    {
        _parkingMenu.AssociateWith(_parkingModel);
        ShowMenu(_parkingMenu);
    }

    public void Tick(float deltaTime)
    {
        double money = GamePlaySytem.instance.marketSystem.GetMoney();
        _moneyLabel.text = money.ToLongUnit(out string unitStr);
        _moneyUnitLabel.text = unitStr;
        string format = Language.Get("CM_PER_MIN","COMMON");
        double value = CalculateProfits();
        _profitsPerMinuteLabel.text = string.Format(format, value.ToShortUnitWithUnits());
        double gemsValue = GamePlaySytem.instance.marketSystem._view.gems;
        _preminumLabel.text = gemsValue.ToShortNeverDecimalsInUnit(out string _txt);
        _mainMenu.Tick();
    }    

    public void ShowCashierMenu()
    {
        _checkoutsMenu.Populate();
        ShowMenu(_checkoutsMenu);
        
    }

    public void ShowOfficeManagerMenu(OfficeManagerMenu.Tab tabSelected = OfficeManagerMenu.Tab.Buildings)
    {
        //Debug.Log("SHOW OFFICE");
        _officeManagerMenu.InitializeInternal();
        _officeManagerMenu.SelectedTab = tabSelected;
        ShowMenu(_officeManagerMenu);

    }

    public void ShowShopMenu()
    {
        ShowMenu(_shopMenu);
        AdsControl.Instance.showAds();
    }

    public void ShowPreminumResearchMenu()
    {
        ShowMenu(_preminumResearchMenu);
        AdsControl.Instance.showAds();
    }

    public void ShowMultiplyEarningMenu()
    {
        ShowMenu(_multiplyEarningsMenu);
    }

    public void ShowSettingMenu()
    {
        ShowMenu(_settingMenu);
        AdsControl.Instance.showAds();
    }    

    public void ShowComeBackMenu(double oldMoney, double earnedMoney, int timeStamp)
    {
        welcomeBackMenu.SetOldMoney(oldMoney);
        welcomeBackMenu.SetMoneyEarned(earnedMoney);
        welcomeBackMenu.SetTimeLapsed(timeStamp);
        welcomeBackMenu.InitializeInternal();
        ShowMenu(welcomeBackMenu);
       
    }    
}
