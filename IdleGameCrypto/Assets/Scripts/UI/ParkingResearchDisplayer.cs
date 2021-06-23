using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ParkingResearchDisplayer : MonoBehaviour
{
    [SerializeField]
    protected GameObject _levelArea;
    [SerializeField]
    protected TextMeshProUGUI _levelLabel;
    [SerializeField]
    protected TextLocalize _titleLabel;
    [SerializeField]
    protected TextLocalize _descriptionLabel;
    [SerializeField]
    protected Image _icon;
    [SerializeField]
    protected MarketButton _upgradeButton;
    [SerializeField]
    protected ButtonPriceReactive _priceLabel;
    [SerializeField]
    protected MarketSlider _progressSlider;
    [SerializeField]
    protected Image _bonusIcon;
    [SerializeField]
    protected TextMeshProUGUI _bonusValue;
    [Header("Parameters")]
    [SerializeField]
    protected string _bonusFormatKey = "GENERIC_BONUS_FORMAT";
    [SerializeField]
    protected float _unlockedlSize;
    [SerializeField]
    protected float _lockedSize;
    [SerializeField]
    protected Color _titleUnlockedColor;
    [SerializeField]
    protected Color _titleLockedColor;
    [SerializeField]
    protected Color _descriptionUnlockedColor;
    [SerializeField]
    protected Color _descriptionLockedColor;
    public UnityEvent OnPurchased;

    public MarketSystem MarketSystem;

    public ParkingVisualModel _model;



    public void ShowInternal(ParkingVisualModel element)
    {
        _model = element;
        if (element == null)
        {
            base.gameObject.SetActive(value: false);
            return;
        }
        base.gameObject.SetActive(value: true);
        LayoutElement component = GetComponent<LayoutElement>();
        if (element.level > 0)
        {
            component.preferredHeight = _unlockedlSize;
            _levelArea.SetActive(value: true);
            _levelLabel.text = element.researchLevel.ToString();
            _titleLabel.key = element.ResearchNameKey;
            _descriptionLabel.key = element.ResearchDescriptionKey;
            _descriptionLabel.sheet = "RESEARCHES";
            _titleLabel.sheet = "RESEARCHES";
            _titleLabel.Localize();
            _descriptionLabel.Localize();
            _icon.sprite = GamePlaySytem.instance._atlas.GetSprite(element.ResearchIconName);
            _progressSlider.gameObject.SetActive(value: true);
            _progressSlider.value = element.ResearchLevelsProgress;
            _bonusIcon.gameObject.SetActive(value: true);
            Sprite sprite = GamePlaySytem.instance._atlas.GetSprite(element.ResearchBonusIconName);
            if (sprite != null)
            {
                _bonusIcon.gameObject.SetActive(value: true);
                _bonusIcon.sprite = sprite;
            }
            else
            {
                _bonusIcon.gameObject.SetActive(value: false);
            }
            _bonusValue.gameObject.SetActive(value: true);
            string format = Language.Get(_bonusFormatKey, "RESEARCHES");
            _bonusValue.text = string.Format(format, element.CurrentResearchLevelBonus);
            _upgradeButton.interactable = true;
            if (element.CanLevelUpResearch)
            {
                ShowUpgradeButton();
                _priceLabel.ShowAsNormal();
                _priceLabel.requiredPrice = element.NextLevelResearchCost();
            }
            else
            {
                ShowAsMaxReached();
            }
            TextMeshProUGUI component2 = _titleLabel.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI component3 = _descriptionLabel.GetComponent<TextMeshProUGUI>();
            component2.color = _titleUnlockedColor;
            component3.color = _descriptionUnlockedColor;
        }
        else
        {
            ShowUpgradeButton();
            _levelArea.SetActive(value: false);
            component.preferredHeight = _lockedSize;
            _progressSlider.gameObject.SetActive(value: false);
            _bonusIcon.gameObject.SetActive(value: false);
            _bonusValue.gameObject.SetActive(value: false);
            _titleLabel.key = element.ResearchNameKey;
            _titleLabel.Localize();
            _descriptionLabel.key = element.ResearchMissingBuildingKey;
            _descriptionLabel.Localize();
            _icon.sprite = GamePlaySytem.instance._atlas.GetSprite(element.ResearchIconName);
            _priceLabel.requiredPrice = element.NextLevelResearchCost();
            _upgradeButton.interactable = false;
            TextMeshProUGUI component4 = _titleLabel.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI component5 = _descriptionLabel.GetComponent<TextMeshProUGUI>();
            component4.color = _titleLockedColor;
            component5.color = _descriptionLockedColor;
        }
        Refresh();
    }
    protected void ShowUpgradeButton()
    {
        _priceLabel.ShowAsNormal();
    }
    protected void ShowAsMaxReached()
    {
        _priceLabel.ShowAsMaxReached();
    }
    public void Refresh()
    {
        if (_model != null && _model.level > 0)
        {
            _priceLabel.Refresh();
        }
    }
    protected void SubscribeToEventsInternal()
    {
        _upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);

    }
    public void UnSubscribeToEventsInternal()
    {
        _upgradeButton.onClick.RemoveListener(OnUpgradeButtonClicked);
    }
    protected void OnUpgradeButtonClicked()
    {
       // Debug.Log("PARKING RESEARCH LEVEL " + _upgradeButton.name);
        MarketSystem._view.LevelUpResearch(_model.id);
        _model = MarketSystem._view._parkingVisualModel;
        ShowInternal(_model);
        OnPurchased.Invoke();
    }

    public void Initialize()
    {
        //_model = element;
        SubscribeToEventsInternal();

    }
}






