using DG.Tweening;
using TMPro;
using UnityEngine;


	public class MoneyFeedback : MonoBehaviour
	{
		[SerializeField]
		private TextMeshPro _label;

		[SerializeField]
		private Vector3 _offset;

		public Sequence Show(double amount, Vector3 point, float duration = 0.2f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("Gameplay");
			sequence.AppendCallback(delegate
			{
				_label.text = amount.ToShortUnitWithUnits();
				_label.transform.position = point + _offset;
				Camera main = Camera.main;
				Vector3 normalized = (_label.transform.position - main.transform.position).normalized;
				_label.transform.rotation = Quaternion.LookRotation(normalized);
			});
			sequence.Append(_label.transform.DOMoveY(3f, duration).SetEase(Ease.OutBack));
			sequence.Insert(0f, _label.DOFade(1f, duration));
			return sequence;
		}

		public Sequence Stay(float duration = 0.2f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("Gameplay");
			sequence.Append(_label.transform.DOMoveY(3f, duration).SetEase(Ease.Linear));
			return sequence;
		}

		public Sequence Hide(float duration = 0.2f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("Gameplay");
			sequence.Append(_label.transform.DOMoveY(3f, duration));
			sequence.Insert(0f, _label.DOFade(0f, duration));
			return sequence;
		}

		public Sequence ShowAndHide(double amount, Vector3 point, float duration = 1f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("Gameplay");
			sequence.AppendCallback(delegate
			{
				_label.text = amount.ToShortUnitWithUnits();
				_label.transform.position = point + _offset;
				Camera main = Camera.main;
				if (main != null)
				{
					Vector3 normalized = (_label.transform.position - main.transform.position).normalized;
					_label.transform.rotation = Quaternion.LookRotation(normalized);
				}
			});
			sequence.Append(_label.transform.DOMoveY(3f, duration));
			sequence.Insert(0f, _label.DOFade(1f, duration * 0.5f));
			sequence.Insert(duration * 0.5f, _label.DOFade(0f, duration * 0.2f));
			return sequence;
		}
	}

