using TMPro;
using UnityEngine;
using UnityEngine.UI;


    public class OfficeManagerShopBuilder : MonoBehaviour
    {
        public string Shop;
        [Header("Parametrs")]
        [SerializeField]
        private Color _titleUnlockedColor;
        [SerializeField]
        private Color _titleLockedColor;
        [SerializeField]
        private Color _descriptionUnlockedColor;
        [SerializeField]
        private Color _descriptionLockedColor;
        [Header("References")]
        [SerializeField]
        private TextLocalize _titleLabel;
        [SerializeField]
        private TextLocalize _descriptionLabel;
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private MarketButton _buildButton;
        [SerializeField]
        private ButtonPriceReactive _priceButton;
        [SerializeField]
        private GameObject _builtCheck;
        [HideInInspector]
        public Button.ButtonClickedEvent OnBuildShopClicked;
        private bool _isSubscribedToEvents;

        public GamePlaySytem _gamePlaySystem;

        public MarketSystem _marketSystem;
        
        
        protected GeneratorModel shopModel
        {
            get;
            private set;
        }
        public bool CanPurchase => _priceButton.interactable;
        public void Show(string shopId)
        {
             shopModel = _marketSystem._view.GetGeneratorModel(shopId);
            // Debug.Log("ID " + shopModel.id);
            Refresh();
            if (!_isSubscribedToEvents)
            {
                _isSubscribedToEvents = true;
                _buildButton.name = "Build" +shopModel.id + "Button";
                _buildButton.onClick.AddListener(OnBuildButtonClicked);
            }
        }
        public void Refresh()
        {
            if (shopModel.id != null)
            {
                _titleLabel.key = shopModel.NameKey;
                _icon.sprite = _gamePlaySystem._atlas.GetSprite(shopModel.IconName);
                // Debug.Log("ID " + shopModel.id);
                _titleLabel.Localize();
                if (shopModel.level > 0)
                {
                    ShowBuilt(shopModel);
                    //Debug.Log("SHOW BUILD " + shopModel.id);
                }
                else if (_marketSystem._view.IsShopUnlocked(shopModel.id))
                {
                    ShowUnlocked(shopModel);
                    //Debug.Log("SHOW UNLOCK " + shopModel.id);
                }
                else
                {
                    ShowLocked(shopModel);
                    //Debug.Log("SHOW LOCK " + shopModel.id);
                }
            }
        }
        private void ShowBuilt(GeneratorModel shopModel)
        {
            _descriptionLabel.key = shopModel.DescriptionKey;
            _descriptionLabel.Localize();
            _priceButton.ShowAsMaxReached();
            GetComponent<CanvasGroup>().interactable = true;
            TextMeshProUGUI component = _titleLabel.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI component2 = _descriptionLabel.GetComponent<TextMeshProUGUI>();
            component.color = _titleUnlockedColor;
            component2.color = _descriptionUnlockedColor;
        }
        private void ShowLocked(GeneratorModel shop)
        {
            _descriptionLabel.key = shop.MissingPreviousShopKey;
            _descriptionLabel.Localize();
            _priceButton.requiredPrice = shop._dataModel.build_cost;
            _buildButton.interactable = false;
            _priceButton.ShowAsNormal();
            _priceButton.Refresh();
            GetComponent<CanvasGroup>().interactable = false;
            TextMeshProUGUI component = _titleLabel.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI component2 = _descriptionLabel.GetComponent<TextMeshProUGUI>();
            component.color = _titleLockedColor;
            component2.color = _descriptionLockedColor;
        }
        private void ShowUnlocked(GeneratorModel shop)
        {
            _descriptionLabel.key = shop.DescriptionKey;
            _descriptionLabel.Localize();
            _priceButton.requiredPrice = shop._dataModel.build_cost;
           _buildButton.interactable = true;
            _priceButton.ShowAsNormal();
            _priceButton.Refresh();
            GetComponent<CanvasGroup>().interactable = true;
            TextMeshProUGUI component = _titleLabel.GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI component2 = _descriptionLabel.GetComponent<TextMeshProUGUI>();
            component.color = _titleUnlockedColor;
            component2.color = _descriptionUnlockedColor;
        }
        private void OnBuildButtonClicked()
        {
            _marketSystem._view.BuildNextShop();
            Show(Shop);
            OnBuildShopClicked.Invoke();
        }
    }

