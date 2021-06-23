using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

    public class ShopMenu : MarketMenu
    {
     
        [SerializeField]
        private RectTransform _widgetsParent;
        [SerializeField]
        private CanvasGroup _headerArea;
        [SerializeField]
        private CanvasGroup _contentArea;
        [SerializeField]
        private GameObject innAppPrefab;

        private List<GameObject> _shopWidgets;

        public InAppStaticModel[] innAppsList;
        public void InitializeInternal()
        {
            CreateWidgets();
        }
        public void CreateWidgets()
        {
            Debug.Log("SHOP INIT");
            _shopWidgets = new List<GameObject>();
            for(int i = 0; i < innAppsList.Length; i++)
            {

            GameObject innAppObj = GameObject.Instantiate(innAppPrefab);
            innAppObj.transform.SetParent(_widgetsParent);
            innAppObj.transform.localScale = Vector3.one;
            innAppObj.transform.localPosition = Vector3.zero;
            _shopWidgets.Add(innAppObj);
            innAppObj.GetComponent<ShopInAppWidget>().LoadData(innAppsList[i]);
            }
            /*
                foreach (KeyValuePair<string, InAppStaticModel> inApp in _gameStaticModel.inApps)
                {
                    InAppStaticModel value = inApp.Value;
                    if (value.category_id == 0)
                    {
                        ShopInAppWidget shopInAppWidget = _gameplayPool.Provide<ShopInAppWidget>(“ShopInApp”);
                        shopInAppWidget.transform.SetParent(_widgetsParent);
                        shopInAppWidget.transform.localScale = Vector3.one;
                        shopInAppWidget.transform.localPosition = Vector3.zero;
                        shopInAppWidget.SetInApp(value);
                        shopInAppWidget.Refresh();
                        _shopWidgets.Add(shopInAppWidget);
                    }
                }
            */
        }
        private void RefreshElements()
        {
        /*
            foreach (ShopInAppWidget shopWidget in _shopWidgets)
            {
                shopWidget.Refresh();
            }
        */
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            CanvasGroup[] array = new CanvasGroup[_shopWidgets.Count];
            for (int i = 0; i < _shopWidgets.Count; i++)
            {
            // array[i] = _shopWidgets[i].Content;
            array[i] = _shopWidgets[i].transform.Find("Content").GetComponent<CanvasGroup>();
        }
            return UIAnimations.HideMenu(_headerArea, array, duration);
        }
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
        
            CanvasGroup[] array = new CanvasGroup[_shopWidgets.Count];
            for (int i = 0; i < _shopWidgets.Count; i++)
            {
                 array[i] = _shopWidgets[i].transform.Find("Content").GetComponent<CanvasGroup>();
            }
            return UIAnimations.ShowMenu(_headerArea, array, duration);
        
        }
        protected override void SubscribeToEventsInternal()
        {
           
        }
        protected override void UnSubscribeToEventsInternal()
        {

        }
    }
