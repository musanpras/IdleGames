using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


	public class SellingPointCurrentLevelDisplayer : MonoBehaviour
	{
		[SerializeField]
		private TextLocalize _titleLabel;

		[SerializeField]
		private TextLocalize _descriptionLabel;

		[SerializeField]
		private TextMeshProUGUI _levelLabel;

		[SerializeField]
		private MarketSlider _targetLevelSlider;

		[SerializeField]
		private TextMeshProUGUI _targetLevelLabel;

		[SerializeField]
		private TextMeshProUGUI _employeesProgressLabel;

		[SerializeField]
		private TextMeshProUGUI _basePriceLabel;

		[SerializeField]
		private TextMeshProUGUI _multiplierLabel;

		[SerializeField]
		private TextMeshProUGUI _totalValueLabel;

		[SerializeField]
		private TextMeshProUGUI _clientsLabel;

		[SerializeField]
		private TextMeshProUGUI _processingValueLabel;

		[SerializeField]
		private Image _productIcon;

		[SerializeField]
		private ParticleSystem _productLevelUpFx;

		[SerializeField]
		private Color _normalProgressBarColor;

		[SerializeField]
		private Color _normalProgressBarBGColor;

		[SerializeField]
		private Color _stellarProgressBarColor;

		[SerializeField]
		private Color _stellarProgressBarBGColor;

		[SerializeField]
		private GameObject _qualityWidget;

		[SerializeField]
		private List<Image> _qualityStars = new List<Image>();

		[SerializeField]
		private GameObject _targetLevelWidget;

		[SerializeField]
		private TextMeshProUGUI _targetLevelStar;


		private GeneratorModel _previousModel;

		private int lastStellarLevel = -1;

	
		public  void ShowInternal(GeneratorModel element)
		{
			_titleLabel.key = element.ProductNameKey;
			_titleLabel.sheet = "PRODUCTS";
			_titleLabel.Localize();
			_descriptionLabel.key = element.ProductDescriptionKey;
			_descriptionLabel.sheet = "PRODUCTS";
			_descriptionLabel.Localize();
			_levelLabel.text = element.level.ToString();
			if (_previousModel == null || element != _previousModel)
			{
				lastStellarLevel = -1;
			}
			string format = Language.Get("CM_LEVEL_FORMAT", "COMMON");
			if (element.LevelForNextProduct < 0)
			{
				_targetLevelLabel.text = Language.Get("SHOP_MAX_LEVEL", "UI");
			}
			else if (!element.IsStellar)
			{
				_targetLevelLabel.text = string.Format(format, element.LevelForNextProduct);
			}
			else
			{
				_targetLevelLabel.text = "";
				if (element.StellarLevel > 0)
				{
					_targetLevelStar.text = element.StellarLevel + 1 + " " + Language.Get("UI_STARS", "UI");
				}
				else
				{
					_targetLevelStar.text = element.StellarLevel + 1 + " " + Language.Get("UI_STAR", "UI");
				}
			}
			_targetLevelSlider.value = element.ProgressToNextProduct;
			if (!element.IsStellar)
			{
				_targetLevelSlider.SetColor(_normalProgressBarColor, _normalProgressBarBGColor);
				_qualityWidget.SetActive(value: false);
				_targetLevelWidget.SetActive(value: false);
			}
			else
			{
				_targetLevelSlider.SetColor(_stellarProgressBarColor, _stellarProgressBarBGColor);
				_qualityWidget.SetActive(value: true);
				if (element.LevelForNextProduct < 0)
				{
					_targetLevelWidget.SetActive(value: false);
				}
				else
				{
					_targetLevelWidget.SetActive(value: true);
				}
				int stellarLevel = element.StellarLevel;
				for (int i = 0; i < 5; i++)
				{
					if (i < stellarLevel)
					{
						_qualityStars[i].sprite = GamePlaySytem.instance._atlas.GetSprite("icon_quality");
					}
					else
					{
						_qualityStars[i].sprite = GamePlaySytem.instance._atlas.GetSprite("icon_quality_base");
					}
				}
			}
			_basePriceLabel.text = element.BaseValue.ToShortUnitWithUnits();
			_multiplierLabel.text = $"x{element.GetMultiplierForProduct(element.ProductIndex + 1)}";
			_totalValueLabel.text = element.GetCurrentProduction().ToShortUnitWithUnits();
			string format2 = Language.Get("CM_PER_MIN_DECIMAL2", "COMMON");
			_clientsLabel.text = string.Format(format2, element.ClientsPerMinute);
			Sprite sprite = GamePlaySytem.instance._atlas.GetSprite(element.ProductIconName);
			bool flag = sprite == _productIcon.sprite;
			bool num = element == _previousModel && !flag;
			bool flag2 = lastStellarLevel != -1 && lastStellarLevel < element.StellarLevel;
			lastStellarLevel = element.StellarLevel;
			if (num | flag2)
			{
				_productIcon.sprite = sprite;
				_productLevelUpFx.Play();
				//_audioSystem.Play("ui_evolution_shop");
			}
			else if (!flag)
			{
				_productIcon.sprite = sprite;
			}
			_employeesProgressLabel.text = $"{element.TotalEmployees}/{element.MaxEmployees}";
			float clientProcessDuration = element.ClientProcessDuration;
			if (clientProcessDuration < 60f)
			{
				string timeUnitShortName = TimeExtensionsUtil.GetTimeUnitShortName(0, TIME_UNIT.SECOND);
				_processingValueLabel.text = $"{clientProcessDuration:0.##}{timeUnitShortName}";
			}
			else
			{
				_processingValueLabel.text = clientProcessDuration.ToShortReadableTime();
			}
			_previousModel = element;
		}

	}
