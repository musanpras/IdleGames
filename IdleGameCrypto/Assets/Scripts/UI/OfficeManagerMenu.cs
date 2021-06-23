using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


    public class OfficeManagerMenu : MarketMenu
    {
        public enum Tab
        {
            Buildings,
            Stats
        }
        [SerializeField]
        private CanvasGroup _headerArea;
        [SerializeField]
        private CanvasGroup _tabsArea;
        [SerializeField]
        private OfficeManagerTab[] _tabs;
        [SerializeField]
        private GameObject _buildingsAlert;
        private ToggleGroup _tabsGroup;
        private MarketToggleButton[] _tabButtons;

        public MarketSystem _marketSystem;
       
        public Tab SelectedTab
        {
            get;
            set;
        }
        public void InitializeInternal()
        {
        
             _tabsGroup = _tabsArea.GetComponent<ToggleGroup>();
            _tabButtons = _tabsGroup.GetComponentsInChildren<MarketToggleButton>();
            OfficeManagerTab[] tabs = _tabs;
          //_tabsGroup = _tabsArea.GetComponentInChildren<ToggleGroup>();
            foreach (OfficeManagerTab officeManagerTab in tabs)
            {
                officeManagerTab.Initialize();
                //HideTab(officeManagerTab, 0f);
            }
            MarketToggleButton[] tabButtons = _tabButtons;
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].Initialize();
            }
            SelectTab(Tab.Buildings, 0f);
            //OnTabButtonSelected(_tabButtons[1]);
            
        }
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
            Sequence sequence = DOTween.Sequence();
            HideTabsInmediatly();
            sequence.SetId("UI");
            sequence.AppendCallback(delegate
            {
                _buildingsAlert.SetActive(_marketSystem._view.CanBuildNewShop);
            });
            sequence.Append(UIAnimations.ShowMenu(_headerArea, new CanvasGroup[1]
            {
                 _tabsArea
            }, duration));
            sequence.Append(SelectTab(SelectedTab));
            return sequence;
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(HideTab(SelectedTab));
            sequence.Append(UIAnimations.HideMenu(_headerArea, new CanvasGroup[1]
            {
                 _tabsArea
            }, duration));
            sequence.AppendCallback(delegate
            {
                DeselectTab(SelectedTab);
            });
            sequence.Append(SelectTab(Tab.Buildings, 0f));
            return sequence;
        }
        protected override void SubscribeToEventsInternal()
        {
            MarketToggleButton[] tabButtons = _tabButtons;
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].onSelected.AddListener(OnTabButtonSelected);
            }
            OfficeManagerTab[] tabs = _tabs;
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].OnOperationPerformed.AddListener(OnOperationOnTabPerformed);
            }
        }
        protected override void UnSubscribeToEventsInternal()
        {
            MarketToggleButton[] tabButtons = _tabButtons;
            for (int i = 0; i < tabButtons.Length; i++)
            {
                tabButtons[i].onSelected.RemoveListener(OnTabButtonSelected);
            }
            OfficeManagerTab[] tabs = _tabs;
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].OnOperationPerformed.RemoveListener(OnOperationOnTabPerformed);
            }
        }
        public Sequence SelectTab(Tab tab, float duration = 0.2f)
        {
            _tabButtons[(int)tab].Select();
            return ShowTab(tab, duration);
        }
        public void DeselectTab(Tab tab, float duration = 0.2f)
        {
            _tabButtons[(int)tab].Deselect();
        }
        protected void OnTabButtonSelected(MarketToggleButton tabButton)
        {
            Tab tab = (Tab)IndexOf(tabButton);
            if (tab != SelectedTab)
            {
                ShowTab(tab);
            }
        }
        protected int IndexOf(MarketToggleButton tabButton)
        {
            for (int i = 0; i < _tabButtons.Length; i++)
            {
                if (_tabButtons[i] == tabButton)
                {
                    return i;
                }
            }
            return -1;
        }
        protected int IndexOf(OfficeManagerTab tab)
        {
            for (int i = 0; i < _tabs.Length; i++)
            {
                if (_tabs[i] == tab)
                {
                    return i;
                }
            }
            return -1;
        }
        protected Sequence ShowTab(Tab tab, float duration = 0.2f)
        {
            return ShowTab(_tabs[(int)tab], duration);
        }
        protected Sequence ShowTab(OfficeManagerTab tab, float duration = 0.2f)
        {
            
            Tab tab2 = (Tab)IndexOf(tab);
            if (tab2 != SelectedTab)
            {
                HideTab(SelectedTab);               
            }
            SelectedTab = tab2;
            tab.gameObject.SetActive(value: true);
       // Debug.Log("show TAB " + tab.name);
        return tab.ShowAnimated(duration);
        }
        protected Sequence HideTab(Tab tab, float duration = 0.2f)
        {
            return HideTab(_tabs[(int)tab], duration);
        }
        protected void HideTabsInmediatly()
        {
            OfficeManagerTab[] tabs = _tabs;
            for (int i = 0; i < tabs.Length; i++)
            {
                tabs[i].gameObject.SetActive(value: false);
           /// Debug.Log("hide TAB " + tabs[i].name);
            }
        }
        protected Sequence HideTab(OfficeManagerTab tab, float duration = 0.2f)
        {
            Sequence sequence = tab.HideAnimated(duration);
            sequence.AppendCallback(delegate
            {
                tab.gameObject.SetActive(value: false);
            });
            return sequence;
        }
        protected void OnOperationOnTabPerformed()
        {
            _buildingsAlert.SetActive(_marketSystem._view.CanBuildNewShop);
        }
    }






