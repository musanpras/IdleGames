using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

    public class CheckboxButton : MonoBehaviour
    {
        public Image icon;
        protected Tween _selectedTween;
        protected Tween _deselectedTween;
      
        private bool _isInitialized;
        public bool isSelected
        {
            get;
            protected set;
        }
        public void SetSelectedTween(Tween selectedTween)
        {
            if (_selectedTween != null)
            {
                _selectedTween.Kill();
            }
            _selectedTween = selectedTween;
        }
        public void SetDeselectedTween(Tween deselectedTween)
        {
            if (_deselectedTween != null)
            {
                _deselectedTween.Kill();
            }
            _deselectedTween = deselectedTween;
        }
        public void Show()
        {
            base.gameObject.SetActive(value: true);
            if (!_isInitialized)
            {
                _isInitialized = true;
                Sequence sequence = DOTween.Sequence();
                sequence.SetId("UI");
                sequence.AppendCallback(delegate
                {
                    icon.sprite = GamePlaySytem.instance._atlas.GetSprite("icon_control_on");
                });
                sequence.Pause();
                sequence.SetAutoKill(autoKillOnCompletion: false);
                SetSelectedTween(sequence);
                Sequence sequence2 = DOTween.Sequence();
                sequence2.SetId("UI");
                sequence2.AppendCallback(delegate
                {
                    icon.sprite = GamePlaySytem.instance._atlas.GetSprite("icon_control_off");
                });
                sequence2.Pause();
                sequence2.SetAutoKill(autoKillOnCompletion: false);
                SetDeselectedTween(sequence2);
            }
        }
        public void Hide()
        {
            base.gameObject.SetActive(value: false);
        }
        public void Select()
        {
            isSelected = true;
            if (_selectedTween != null)
            {
                _selectedTween.Restart();
                _selectedTween.PlayForward();
            }
        }
        public void Deselect()
        {
            isSelected = false;
            if (_deselectedTween != null)
            {
                _deselectedTween.Restart();
                _deselectedTween.PlayForward();
            }
            else if (_selectedTween != null)
            {
                _selectedTween.Goto(float.PositiveInfinity);
                _selectedTween.PlayBackwards();
            }
        }
    }
