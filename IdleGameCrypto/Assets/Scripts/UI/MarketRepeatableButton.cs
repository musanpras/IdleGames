using DG.Tweening;
using UnityEngine;


	public class MarketRepeatableButton : MarketButton
	{
		[SerializeField]
		private float _secondClickDelay = 0.5f;

		[SerializeField]
		private float _startClicksDelay = 0.2f;

		[SerializeField]
		private float _finalClicksDelay = 0.05f;

		[SerializeField]
		private int _clicksForFinalDelay = 25;

		[SerializeField]
		private float _clickFxMinDelay = 0.07f;

		private float _clickDelay;

		private float _timeLastClickFx;

		public int ClicksCount
		{
			get;
			private set;
		}

		protected override void ShowClickAnimation()
		{
			base.transform.DOKill();
			base.transform.DOScale(0.95f, _clickDelay).SetEase(Ease.Flash, 2f);
			if (Time.time - _timeLastClickFx >= _clickFxMinDelay)
			{
				ShowClickFxFeedback();
				_timeLastClickFx = Time.time;
			}
		}

		protected override void StartClick()
		{
			base.StartClick();
			if (base.interactable)
			{
				ClicksCount = 0;
				_clickDelay = _startClicksDelay;
				_timeLastClickFx = 0f;
			}
		}

		protected override void ReleaseClick()
		{
			base.transform.DOKill();
			ShowClickReleasedAnimation();
			EnableParentScroll();
			base.IsPressedDown = false;
		}

		private void Update()
		{
			if (!base.interactable)
			{
				if (base.IsPressedDown)
				{
					ReleaseClick();
				}
			}
			else
			{
				if (!base.IsPointerInside || !base.IsPressedDown)
				{
					return;
				}
				if (ClicksCount == 1)
				{
					if (Time.time >= base.TimeLastClick + _secondClickDelay)
					{
						ClicksCount++;
						OnClickDetected();
						_clickDelay = _startClicksDelay;
					}
				}
				else if (Time.time >= base.TimeLastClick + _clickDelay)
				{
					ClicksCount++;
					_clickDelay = Mathf.Lerp(_startClicksDelay, _finalClicksDelay, (float)ClicksCount / (float)_clicksForFinalDelay);
					OnClickDetected();
				}
			}
		}
	}

