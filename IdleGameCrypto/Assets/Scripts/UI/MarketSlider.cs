using UnityEngine;
using UnityEngine.UI;


	[RequireComponent(typeof(Slider))]
	public class MarketSlider : MonoBehaviour
	{
		[SerializeField]
		private Slider _slider;

		[SerializeField]
		private ParticleSystem _onValueChangedFx;

		[SerializeField]
		private Image _fill;

		public bool interacable => _slider.interactable;

		public float value
		{
			get
			{
				return _slider.value;
			}
			set
			{
				if (value > _slider.value)
				{
					OnValueChanged(value);
				}
				_slider.value = value;
			}
		}

		protected virtual void OnValueChanged(float value)
		{
			if (_onValueChangedFx != null)
			{
				_onValueChangedFx.Play();
			}
		}

		public void SetColor(Color colorFil, Color colorBG)
		{
			_slider.image.color = colorBG;
			if (_fill != null)
			{
				_fill.color = colorFil;
			}
		}
	}

