using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


    [RequireComponent(typeof(Button))]
    public class MarketButton : MonoBehaviour, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
    {
        
        [SerializeField]
        private Button _button;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private ParticleSystem _onClickedFx;
      
         [HideInInspector]
        public Button.ButtonClickedEvent onClick;
        [HideInInspector]
        public Button.ButtonClickedEvent onClickReleased;
        private Color _originalColor;
        private float _lastFxTime;
        protected CanvasGroup ParentCanvas
        {
            get;
            private set;
        }
        protected ScrollRect ParentScroll
        {
            get;
            private set;
        }
        protected bool IsHorizontalScroll
        {
            get;
            private set;
        }
        protected bool IsVerticalScroll
        {
            get;
            private set;
        }
        public bool interactable
        {
            get
            {
                if (_button.interactable)
                {
                    return IsParentCanvasInteractable;
                }
                return false;
            }
            set
            {
                _button.interactable = value;
            }
        }
        protected bool IsParentCanvasInteractable
        {
            get
            {
                if (ParentCanvas != null)
                {
                    return ParentCanvas.interactable;
                }
                return true;
            }
        }
        public float TimeLastClick
        {
            get;
            private set;
        }
        public bool IsPressedDown
        {
            get;
            protected set;
        }
        public bool IsPointerInside
        {
            get;
            protected set;
        }
        protected virtual void StartClick()
        {
            IsPressedDown = true;
            DisableParentScroll();
            ShowClickDownAnimation();
        }
        protected virtual void ReleaseClick()
        {
            ShowClickReleasedAnimation();
            OnClickDetected();
            EnableParentScroll();
            IsPressedDown = false;
        }
        protected void ShowClickDownAnimation(float duration = 0.2f)
        {
            base.transform.DOKill();
            base.transform.DOScale(0.9f, duration).SetEase(Ease.OutQuad);
            Color endValue = Color.Lerp(_originalColor, Color.black, 0.25f);
            _background.DOColor(endValue, duration).SetEase(Ease.OutQuad);
        }
        protected void ShowClickReleasedAnimation(float duration = 0.2f)
        {
            base.transform.DOKill();
            base.transform.DOScale(1f, duration).SetEase(Ease.OutBack);
            _background.DOColor(_originalColor, duration).SetEase(Ease.OutQuad).OnComplete(delegate
            {
                onClickReleased.Invoke();
            });
        }
        protected virtual void ShowClickAnimation()
        {
            ShowClickFxFeedback();
        }
        protected void ShowClickFxFeedback()
        {
            if (_onClickedFx != null)
            {
                _onClickedFx.Play();
            }
        }
        protected virtual void OnClickDetected()
        {
            if (interactable)
            {
                AudioSystem._instance.Play("ui_button_click");
                ShowClickAnimation();
                onClick.Invoke();
                TimeLastClick = Time.time;
            }
        }
        protected void DisableParentScroll()
        {
            if (!(ParentScroll == null))
            {
                UnityEngine.Debug.Log("Disabling parent scroll");
                ParentScroll.enabled = false;
            }
        }
        protected void EnableParentScroll()
        {
            if (!(ParentScroll == null))
            {
                UnityEngine.Debug.Log("Enabling parent scroll");
                ParentScroll.enabled = true;
            }
        }
        protected void Start()
        {
            _originalColor = _background.color;
            ParentCanvas = GetComponentInParent<CanvasGroup>();
            ParentScroll = GetComponentInParent<ScrollRect>();
            if (ParentScroll != null)
            {
                IsHorizontalScroll = ParentScroll.horizontal;
                IsVerticalScroll = ParentScroll.vertical;
            }
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            eventData.Use();
            IsPointerInside = true;
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            eventData.Use();
            IsPointerInside = false;
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            eventData.Use();
            if (interactable && !IsPressedDown)
            {
                StartClick();
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            eventData?.Use();
            ReleaseClick();
        }
        protected void OnDisable()
        {
            if (IsPressedDown)
            {
                OnPointerUp(null);
            }
        }
    }






