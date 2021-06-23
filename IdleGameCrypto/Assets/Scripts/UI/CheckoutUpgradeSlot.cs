using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

    public class CheckoutUpgradeSlot : MonoBehaviour
    {
        [SerializeField]
        private ButtonPriceReactive _button;
        [SerializeField]
        private MarketButton _upgradeButton;
        [SerializeField]
        private Transform _upgradeZone;
        [SerializeField]
        private Transform _maxZone;
        [SerializeField]
        private TextMeshProUGUI _textLevel;
        [SerializeField]
        private TextMeshProUGUI _textCustomersPerMinute;
        [SerializeField]
        private TextMeshProUGUI _indexLabel;
        [Header("Queue")]
        [SerializeField]
        private Image _queueArea;
        [SerializeField]
        private TextMeshProUGUI _queueLengthValueLabel;
        [SerializeField]
        private StatDynamicVisuals[] _statVisuals;
        private CheckoutModel _model;
        private Action<CheckoutModel> _onUpgradeCallback;
        public void SetModel(CheckoutModel model, Action<CheckoutModel> onClick = null, Action<CheckoutModel> onClickReleased = null)
        {
            _model = model;
            _onUpgradeCallback = onClick;
            _upgradeButton.onClick.RemoveAllListeners();
            _upgradeButton.onClick.AddListener(delegate
            {
                _onUpgradeCallback?.Invoke(_model);
            });
            _upgradeButton.onClickReleased.RemoveAllListeners();
            _upgradeButton.onClickReleased.AddListener(delegate
            {
                onClickReleased?.Invoke(_model);
            });
            int num = int.Parse(Regex.Match(model.id, "\\d+").Value);
            _indexLabel.text = num.ToString("N0");
            Refresh();
        }
        public void Refresh()
        {
            _textCustomersPerMinute.text = string.Format(Language.Get("CM_PER_MIN_DECIMAL1", "COMMON"), (float)_model.GetCurrentProduction());

            _textLevel.text = _model.level.ToString();
            _button.requiredPrice = _model.GetLevelUpPrice(_model.level);
            if (_model.level < _model.MaxLevel)
            {
                _button.ShowAsNormal();
                _button.Refresh();
            }
            else
            {
                _button.ShowAsMaxReached();
            }
            int count = _model._queue.Count;
            _queueLengthValueLabel.text = count.ToString("N0");

            StatDynamicVisuals visuals = FindVisuals(count);
            ApplyVisuals(visuals);
        }
        private StatDynamicVisuals FindVisuals(int queueLength)
        {
            for (int i = 0; i < _statVisuals.Length; i++)
            {
                if (queueLength >= _statVisuals[i].MinValue && queueLength < _statVisuals[i].MaxValue)
                {
                    return _statVisuals[i];
                }
            }
            return _statVisuals[_statVisuals.Length - 1];
        }
        private void ApplyVisuals(StatDynamicVisuals visuals)
        {
            _queueArea.sprite = visuals.Image;
        }

    }






