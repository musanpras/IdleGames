
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

    public class OfficeManagerBuildingsTab : OfficeManagerTab
    {
        [SerializeField]
        private ScrollRect _scroll;
        [SerializeField]
        private int _indexMinScrollOffset;
        [SerializeField]
        private int _indexMaxScrollOffset;
        private OfficeManagerShopBuilder[] _builders;

        public MarketSystem _marketSystem;
        
        protected override void InitializeInternal()
        {
            FindScrollElements(_scroll);
        }
        private void FindScrollElements(ScrollRect scroll)
        {
            _builders = scroll.gameObject.GetComponentsInChildren<OfficeManagerShopBuilder>();
        }
        protected override void SubscribeToEventsInternal()
        {
            OfficeManagerShopBuilder[] builders = _builders;
            for (int i = 0; i < builders.Length; i++)
            {
                builders[i].OnBuildShopClicked.AddListener(OnShopBuilt);
            }
        }
        protected override void UnSubscribeToEventsInternal()
        {
            OfficeManagerShopBuilder[] builders = _builders;
            for (int i = 0; i < builders.Length; i++)
            {
                builders[i].OnBuildShopClicked.RemoveListener(OnShopBuilt);
            }
        }
        public override Sequence ShowAnimated(float duration = 0.2f)
        {
            CanvasGroup canvas = GetComponent<CanvasGroup>();
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.AppendCallback(delegate
            {
                canvas.blocksRaycasts = false;
                OfficeManagerShopBuilder[] builders = _builders;
                foreach (OfficeManagerShopBuilder obj in builders)
                {
                    obj.Show(obj.Shop);
                   // Debug.Log("show " + obj.Shop);
                }
                CenterOnPurchasableShop();
            });
            sequence.Append(canvas.DOFade(1f, duration));
            sequence.AppendCallback(delegate
            {
                canvas.blocksRaycasts = true;
            });
            return sequence;
        }
        public override Sequence HideAnimated(float duration = 0.2f)
        {
            CanvasGroup canvas = GetComponent<CanvasGroup>();
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.AppendCallback(delegate
            {
                canvas.blocksRaycasts = true;
            });
            sequence.Append(canvas.DOFade(0f, duration));
            return sequence;
        }
        protected void CenterOnPurchasableShop()
        {
            int indexLastPurchasedShop = GetIndexLastPurchasedShop();
            int indexMinScrollOffset = _indexMinScrollOffset;
            int max = _builders.Length - _indexMaxScrollOffset;
            indexLastPurchasedShop = Mathf.Clamp(indexLastPurchasedShop, indexMinScrollOffset, max);
            float num = indexLastPurchasedShop.Normalize(indexMinScrollOffset, max);
            _scroll.verticalNormalizedPosition = 1f - num;
            //UnityEngine.Debug.Log("#UI# Centering in element " + (indexLastPurchasedShop + 1) + "/" + _builders.Length + " with normalized position: " + num);
         }
        protected int GetIndexLastPurchasedShop()
        {
            for (int num = _builders.Length - 1; num >= 0; num--)
            {
                if (_marketSystem._view.GetGeneratorModel(_builders[num].Shop).level > 0)
                {
                    return num;
                }
            }
            return -1;
        }
        protected void Refresh()
        {
            OfficeManagerShopBuilder[] builders = _builders;
            for (int i = 0; i < builders.Length; i++)
            {
                builders[i].Refresh();
            }
        }
        protected void Update()
        {
        Refresh();  
        }
      
        private void OnShopBuilt()
        {
            Refresh();
            OnOperationPerformed.Invoke();
        }
    }
