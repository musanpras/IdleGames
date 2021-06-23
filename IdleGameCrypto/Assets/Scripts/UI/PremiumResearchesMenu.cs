
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

    public class PremiumResearchesMenu : MarketMenu
    {
       
        [SerializeField]
        private RectTransform _scrollContentParent;
        [SerializeField]
        private CanvasGroup _headerArea;
        [SerializeField]
        private CanvasGroup _contentArea;

        private List<EpicResearchWidget> _widgets;

        public GameObject _researchPrefab;

        public PremiumResearchStaticModel[] researchesTemplateIds;

        public void InitializeInternal()
        {
            _widgets = new List<EpicResearchWidget>();
            CreateResearchWidgets();
        }
        private void CreateResearchWidgets()
        {
            /*
            List<string> researchesTemplateIds = _marketGameStaticModel.GetResearchesTemplateIds();
            for (int i = 0; i < researchesTemplateIds.Count; i++)
            {
                PremiumResearchStaticModel research = _marketGameStaticModel.GetResearch(researchesTemplateIds[i]);
                if (ShouldCreatePremiumResearch(research))
                {
                    EpicResearchWidget epicResearchWidget = _gameplayPool.Provide<EpicResearchWidget>(“EpicResearch”);
                    epicResearchWidget.transform.SetParent(_scrollContentParent);
                    epicResearchWidget.transform.localScale = Vector3.one;
                    epicResearchWidget.transform.localPosition = Vector3.zero;
                    epicResearchWidget.SetResearch(research.id);
                    epicResearchWidget.SetPurchaseCallback(RefreshResearches);
                    _widgets.Add(epicResearchWidget);
                }
            }
            */
                  

        for (int i = 0; i < researchesTemplateIds.Length; i++)
           {
            EpicResearchWidget epicResearchWidget = GameObject.Instantiate(_researchPrefab).GetComponent<EpicResearchWidget>();
            epicResearchWidget.transform.SetParent(_scrollContentParent);
            epicResearchWidget.transform.localScale = Vector3.one;
            epicResearchWidget.transform.localPosition = Vector3.zero;
            epicResearchWidget.LoadData(researchesTemplateIds[i]);
            _widgets.Add(epicResearchWidget);
            }
        }
        private bool ShouldCreatePremiumResearch(PremiumResearchStaticModel research)
        {
            return research.show_in_menu;
        }
        public override void GetFocus()
        {
            base.GetFocus();
            RefreshResearches();
        }
        private void RefreshResearches()
        {
            foreach (EpicResearchWidget widget in _widgets)
            {
               // widget.RefreshModels();
                widget.RefreshVisuals();
            }
        }
        protected override Sequence HideAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.HideMenu(_headerArea, new CanvasGroup[1]
            {
                 _contentArea
            }, duration);
        }
        protected override Sequence ShowAnimatedInternal(float duration = 0.2f)
        {
            return UIAnimations.ShowMenu(_headerArea, new CanvasGroup[1]
            {
                 _contentArea
            }, duration);
        }
        protected override void SubscribeToEventsInternal()
        {
        }
        protected override void UnSubscribeToEventsInternal()
        {
        }
    }
