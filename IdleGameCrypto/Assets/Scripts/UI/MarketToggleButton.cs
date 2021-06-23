using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

    [RequireComponent(typeof(Toggle))]
    public class MarketToggleButton : MonoBehaviour
    {
        [Serializable]
        public class ToggleEvent : UnityEvent<MarketToggleButton>
        {
        }
        [SerializeField]
        private LayoutElement _element;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private Image _background;
        [SerializeField]
        private CanvasGroup _nameGroup;
        [Header("Parameters")]
        [SerializeField]
        private Vector2 _normalSize;
        [SerializeField]
        private Sprite _normalBackground;
        [SerializeField]
        private Color _normalBackgroundColor;
        [SerializeField]
        private Color _normalIconColor;
        [SerializeField]
        private Vector2 _expandedSize;
        [SerializeField]
        private Sprite _expandedBackground;
        [SerializeField]
        private Color _expandedBackgroundColor;
        [SerializeField]
        private Color _expandedIconColor;
        [HideInInspector]
        public ToggleEvent onSelected;
        [HideInInspector]
        public ToggleEvent onDeselected;
        private Toggle _toggle;
        public Toggle Toggle
        {
            get
            {
                if (_toggle == null)
                {
                    _toggle = GetComponent<Toggle>();
                }
                return _toggle;
            }
        }
        public bool interactable
        {
            get
            {
                return Toggle.interactable;
            }
            set
            {
                Toggle.interactable = value;
            }
        }
        public bool IsSelected => Toggle.isOn;
        public bool IsSubscribedToEvents
        {
            get;
            private set;
        }
        public void SubscribeToEvents()
        {
            if (IsSubscribedToEvents)
            {
                UnSubscribeToEvents();
            }
            Toggle.onValueChanged.AddListener(OnValueChanged);
            IsSubscribedToEvents = true;
        }
        public void UnSubscribeToEvents()
        {
            if (IsSubscribedToEvents)
            {
                Toggle.onValueChanged.RemoveListener(OnValueChanged);
                IsSubscribedToEvents = false;
            }
        }
        public void Initialize()
        {
            SubscribeToEvents();
        }
        protected Sequence ShowPressedAnimation(float duration = 0.2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            return sequence;
        }
        protected Sequence ShowSelectedAnimation(float duration = 0.2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(_element.DOPreferredSize(_expandedSize, duration).SetEase(Ease.OutQuad));
            sequence.InsertCallback(0f, delegate
            {
                _background.sprite = _expandedBackground;
            });
            sequence.Insert(0f, _background.DOColor(_expandedBackgroundColor, duration));
            sequence.Insert(0f, _icon.DOColor(_expandedIconColor, duration));
            sequence.Insert(duration * 0.75f, _nameGroup.DOFade(1f, duration * 0.25f));
            return sequence;
        }
        protected Sequence ShowDeselectedAnimation(float duration = 0.2f)
        {
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("UI");
            sequence.Append(_nameGroup.DOFade(0f, duration * 0.25f));
            sequence.Insert(0f, _background.DOColor(_normalBackgroundColor, duration));
            sequence.Insert(0f, _icon.DOColor(_normalIconColor, duration));
            sequence.Insert(duration * 0.1f, _element.DOPreferredSize(_normalSize, duration * 0.9f).SetEase(Ease.OutQuad));
            sequence.InsertCallback(duration * 0.9f, delegate
            {
                _background.sprite = _normalBackground;
            });
            return sequence;
        }
        public void Select()
        {
            Toggle.isOn = true;
        }
        public void Deselect()
        {
            Toggle.isOn = false;
        }
        protected virtual void OnSelected()
        {
            ShowSelectedAnimation();
            onSelected.Invoke(this);
        }
        protected virtual void OnDeselected()
        {
            ShowDeselectedAnimation();
            onDeselected.Invoke(this);
        }
        private void OnValueChanged(bool selected)
        {
            if (selected)
            {
                OnSelected();
            }
            else
            {
                OnDeselected();
            }
        }
    }

