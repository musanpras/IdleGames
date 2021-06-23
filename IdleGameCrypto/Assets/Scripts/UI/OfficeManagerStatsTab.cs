using DG.Tweening;
using UnityEngine;


    public class OfficeManagerStatsTab : OfficeManagerTab
    {
        [SerializeField]
        private CanvasGroup _summary;
        //private OfficeManagerStatsSummary _summary;
        [SerializeField]
        private CanvasGroup _productsList;
        //private OfficeManagerStatsProductsList _productsList;

        public MarketSystem _marketSystem;
        
        protected override void InitializeInternal()
        {
        }
        public override Sequence ShowAnimated(float duration = 0.2f)
        {
            CanvasGroup component = _summary.GetComponent<CanvasGroup>();
            CanvasGroup[] groups = new CanvasGroup[1]
            {
                 _productsList.GetComponent<CanvasGroup>()
            };
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.AppendCallback(delegate
            {
                //_productsList.Initialize();
                //_productsList.Show(MarketSystem.GetModel());
            });
            sequence.Append(UIAnimations.ShowMenu(component, groups, duration));
            return sequence;
        }
        public override Sequence HideAnimated(float duration = 0.2f)
        {
            CanvasGroup component = _summary.GetComponent<CanvasGroup>();
            CanvasGroup[] groups = new CanvasGroup[1]
            {
                 _productsList.GetComponent<CanvasGroup>()
            };
            return UIAnimations.HideMenu(component, groups, duration);
        }
        protected override void OnSecondElapsed()
        {
            if (base.gameObject.activeInHierarchy)
            {
                //_summary.Refresh();
                //_productsList.Refresh();
            }
        }
    }
