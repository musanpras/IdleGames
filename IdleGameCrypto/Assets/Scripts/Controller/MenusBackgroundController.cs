using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


    public class MenusBackgroundController : MonoBehaviour
    {
        public enum Theme
        {
            Light,
            Dark
        }
       
       // private IColorLibrary _colorLibrary;
       
      //  private IAssetProvider _assetProvider;
        [Header("Parameters")]
        [SerializeField]
        public float TransitionDuration = 0.2f;
        [Space]
        [SerializeField]
        private float _normalHeaderSize = 50f;
        [SerializeField]
        private float _expandedHeaderSize = 250f;
        [SerializeField]
        private float[] _expandedHeaderSizeByTheme;
        [SerializeField]
        private float _normalFooterSize;
        [SerializeField]
        private float _expandedFooterSize = 150f;
        [Header("References")]
        [SerializeField]
        private CanvasGroup _header;
        [SerializeField]
        private RectTransform _menusContainer;
        [SerializeField]
        private CanvasGroup _footer;
        [SerializeField]
        private CanvasGroup _menusBackground;
        [SerializeField]
        private CanvasGroup _popupsBackground;
        [SerializeField]
        private Image _headerBackground;
        [SerializeField]
        private Image _footerBackground;
        [SerializeField]
        private Image _background;
        private RectTransform _headerRect;
        private RectTransform _footerRect;
        private RectTransform _backgroundRect;
        private Vector2 _headerPosOffset;
        private Vector2 _footerSizeOffset;
        private Theme _themeSelected;
        public bool IsBackgroundShown
        {
            get;
            private set;
        }
        public static bool HasNotch
        {
            get
            {
                if (!IsIphoneXResolution())
                {
                    return HasLargeSafeAreaAtTop();
                }
                return true;
            }
        }
        public void Initialize()
        {
            _headerRect = _header.GetComponent<RectTransform>();
            _footerRect = _footer.GetComponent<RectTransform>();
            _backgroundRect = _menusBackground.GetComponent<RectTransform>();
            AdjustForNotch();
        }
        public void Show(Theme theme = Theme.Light)
        {
            Show(TransitionDuration, theme);
        }
        public void Show(float duration, Theme theme = Theme.Light)
        {
            IsBackgroundShown = true;
            SetTheme(theme);
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(ShowHeader(duration));
            sequence.Insert(0f, ShowFooter(duration));
            sequence.Insert(0f, ShowBackground(duration));
            sequence.Play();
        }
        public void Transition(Theme theme)
        {
            SetTheme(theme);
        }
        public void Hide(Theme theme = Theme.Light)
        {
            Hide(TransitionDuration, theme);
        }
        public void Hide(float duration, Theme theme = Theme.Light)
        {
            IsBackgroundShown = false;
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(HideHeader(duration));
            sequence.Insert(0f, HideFooter(duration));
            sequence.Insert(0f, HideBackground(duration));
            sequence.OnComplete(delegate
            {
                SetTheme(theme);
            });
            sequence.Play();
        }
        private void AdjustForNotch()
        {
            if (HasNotch)
            {
                _headerPosOffset = Vector2.zero;
                _menusContainer.anchoredPosition = Vector2.zero;
                _footerSizeOffset = Vector2.zero;
            }
            else
            {
                _headerPosOffset = new Vector2(0f, 100f);
                _menusContainer.anchoredPosition = new Vector2(0f, 100f);
                _footerSizeOffset = new Vector2(0f, 0f - _expandedFooterSize);
            }
            _headerRect.anchoredPosition = _headerPosOffset;
            _footerRect.sizeDelta = _footerSizeOffset;
        }
        private static bool HasLargeSafeAreaAtTop()
        {
            Rect rect = new Rect(0f, 0f, Screen.width, Screen.height);
            Rect safeArea = Screen.safeArea;
            UnityEngine.Debug.Log("#UI# Detected full rect as " + rect + ". Safe area is " + safeArea);
             if (CalculateSafeResolutionVerticalDelta() > (float)Screen.height * 0.05f)
            {
                return true;
            }
            return false;
        }
        private static float CalculateSafeResolutionVerticalDelta()
        {
            Rect safeArea = Screen.safeArea;
            return (float)Screen.height - safeArea.height;
        }
        private static float CalculateSafeResolutionHorizontalDelta()
        {
            Rect safeArea = Screen.safeArea;
            return (float)Screen.width - safeArea.width;
        }
        private static bool IsIphoneXResolution()
        {
            return IsEditorIphoneXResolution();
        }
        private static bool IsEditorIphoneXResolution()
        {
            if (Application.isEditor)
            {
                return IsScreenIphoneXResolution();
            }
            return false;
        }
        private static bool IsScreenIphoneXResolution()
        {
            return (double)((float)Screen.width / (float)Screen.height) <= 0.4621;
        }
        private Sequence ShowHeader(float duration)
        {
            return ResizeElement(_headerRect, _expandedHeaderSize, duration);
        }
        private Sequence ShowFooter(float duration)
        {
            return FadeInAndResizeElement(_footer, _expandedFooterSize + _footerSizeOffset.y, duration);
        }
        private Sequence ShowBackground(float duration)
        {
            return FadeInElement(_menusBackground, duration);
        }
        private Sequence HideHeader(float duration)
        {
            return ResizeElement(_headerRect, _normalHeaderSize, duration);
        }
        private Sequence HideFooter(float duration)
        {
            return FadeOutAndResizeElement(_footer, _normalFooterSize, duration);
        }
        private Sequence HideBackground(float duration)
        {
            return FadeOutElement(_menusBackground, duration);
        }
        private void SetTheme(Theme theme = Theme.Light)
        {
            if (_themeSelected != theme)
            {
                _themeSelected = theme;
                SetExpandedHeaderSize(theme);
                string name = "tab_main_menu_" + theme.ToString().ToLower();
                string name2 = "tab_main_menu_inverse_" +theme.ToString().ToLower();
                string key = "background_" + theme.ToString().ToLower();
                _headerBackground.sprite = GamePlaySytem.instance._atlas.GetSprite(name);
                _footerBackground.sprite = GamePlaySytem.instance._atlas.GetSprite(name2);
            // _background.color = _colorLibrary.GetAssociatedColor(key);
            if (theme == Theme.Light)
                _background.color = Color.white;
            else if (theme == Theme.Dark)
                _background.color = Color.gray;
            }
        }
        private void SetExpandedHeaderSize(Theme theme = Theme.Light)
        {
            if (_expandedHeaderSizeByTheme.Length > (int)theme)
            {
                _expandedHeaderSize = _expandedHeaderSizeByTheme[(int)theme];
            }
            else
            {
                _expandedHeaderSize = _expandedHeaderSizeByTheme[0];
            }
        }
        private static Sequence FadeInAndResizeElement(CanvasGroup element, float height, float duration)
        {
            RectTransform component = element.GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(FadeInElement(element, duration));
            sequence.Insert(0f, ResizeElement(component, height, duration));
            return sequence;
        }
        private static Sequence FadeOutAndResizeElement(CanvasGroup element, float height, float duration)
        {
            RectTransform component = element.GetComponent<RectTransform>();
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(FadeOutElement(element, duration));
            sequence.Insert(0f, ResizeElement(component, height, duration));
            return sequence;
        }
        private static Sequence FadeInElement(CanvasGroup element, float duration)
        {
            element.gameObject.SetActive(value: true);
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(element.DOFade(1f, duration));
            return sequence;
        }
        private static Sequence FadeOutElement(CanvasGroup element, float duration)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(element.DOFade(0f, duration));
            sequence.AppendCallback(delegate
            {
                element.gameObject.SetActive(value: false);
            });
            return sequence;
        }
        private static Sequence ResizeElement(RectTransform element, float height, float duration)
        {
            Vector2 endValue = new Vector2(element.sizeDelta.x, height);
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(element.DOSizeDelta(endValue, duration).SetEase(Ease.OutBack));
            return sequence;
        }
    }
