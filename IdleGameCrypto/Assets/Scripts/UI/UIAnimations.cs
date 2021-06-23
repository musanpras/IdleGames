using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

	public class UIAnimations
	{
		public static readonly float DelayBetweenGroups = 0.05f;

		public static Sequence ShowHorizontalIn(CanvasGroup[] groups, float duration, float xOffset = -100f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			for (int i = 0; i < groups.Length; i++)
			{
				float atPosition = DelayBetweenGroups * (float)i;
				sequence.Insert(atPosition, ShowHorizontalIn(groups[i], duration, xOffset));
			}
			return sequence;
		}

		public static Sequence ShowHorizontalIn(CanvasGroup group, float duration, float xOffset = -100f)
		{
			RectTransform component = group.GetComponent<RectTransform>();
			DOTween.Complete(group);
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.AppendCallback(delegate
			{
				group.gameObject.SetActive(value: true);
				group.interactable = false;
			});
			sequence.Append(group.DOFade(1f, duration));
			sequence.Insert(0f, component.DOAnchorPosX(component.anchoredPosition.x + xOffset, duration).SetEase(Ease.OutBack));
			LayoutElement component2 = group.GetComponent<LayoutElement>();
			if (component2 != null)
			{
				sequence.Insert(0f, GrowLayoutElement(component2, duration));
			}
			sequence.AppendCallback(delegate
			{
				group.interactable = true;
			});
			return sequence;
		}

		private static Sequence GrowLayoutElement(LayoutElement element, float duration = 0.2f)
		{
			Sequence sequence = DOTween.Sequence();
			LayoutGroup layoutGroup = element.GetComponentInParent<LayoutGroup>();
			HorizontalLayoutGroup x = layoutGroup as HorizontalLayoutGroup;
			VerticalLayoutGroup x2 = layoutGroup as VerticalLayoutGroup;
			float preferredWidth = element.preferredWidth;
			float preferredHeight = element.preferredHeight;
			sequence.Append(SetLayoutSize(element, (x2 != null) ? preferredWidth : 0f, (x != null) ? preferredHeight : 0f, 0f));
			if (layoutGroup != null)
			{
				sequence.AppendInterval(0.01f);
				sequence.AppendCallback(delegate
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
				});
				sequence.AppendInterval(0.01f);
			}
			sequence.Append(SetLayoutSize(element, preferredWidth, preferredHeight, duration));
			return sequence;
		}

		public static Sequence HideHorizontalOut(CanvasGroup[] groups, float duration, float xOffset = -100f, bool reposition = true)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			for (int i = 0; i < groups.Length; i++)
			{
				sequence.Insert(0f, HideHorizontalOut(groups[i], duration, xOffset));
			}
			return sequence;
		}

		public static Sequence HideHorizontalOut(CanvasGroup group, float duration, float xOffset = -100f, bool reposition = true)
		{
			RectTransform component = group.GetComponent<RectTransform>();
			DOTween.Complete(group);
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.AppendCallback(delegate
			{
				group.interactable = false;
			});
			sequence.Append(group.DOFade(0f, duration));
			sequence.Insert(0f, component.DOAnchorPosX(component.anchoredPosition.x + xOffset, duration).SetEase(Ease.OutQuad));
			if (reposition)
			{
				sequence.Append(component.DOAnchorPosX(component.anchoredPosition.x - xOffset, 0f));
			}
			LayoutElement component2 = group.GetComponent<LayoutElement>();
			if (component2 != null)
			{
				sequence.Insert(0f, ShrinkLayoutElement(component2));
			}
			sequence.AppendCallback(delegate
			{
				group.gameObject.SetActive(value: false);
				group.interactable = true;
			});
			return sequence;
		}

		private static Sequence ShrinkLayoutElement(LayoutElement element, float duration = 0.2f)
		{
			Sequence sequence = DOTween.Sequence();
			LayoutGroup layoutGroup = element.GetComponentInParent<LayoutGroup>();
			float preferredWidth = element.preferredWidth;
			float preferredHeight = element.preferredHeight;
			sequence.Append(SetLayoutSize(element, 0f, 0f, duration));
			sequence.AppendCallback(delegate
			{
				element.gameObject.SetActive(value: false);
			});
			sequence.AppendInterval(0.01f);
			if (layoutGroup != null)
			{
				sequence.AppendCallback(delegate
				{
					LayoutRebuilder.ForceRebuildLayoutImmediate(layoutGroup.GetComponent<RectTransform>());
				});
			}
			sequence.Append(SetLayoutSize(element, preferredWidth, preferredHeight, 0f));
			return sequence;
		}

		private static Sequence SetLayoutSize(LayoutElement element, float width, float height, float duration = 0.2f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.Append(element.DOPreferredSize(new Vector2(width, height), duration));
			return sequence;
		}

		public static Sequence ShowVerticalIn(CanvasGroup[] groups, float duration, float yOffset = -100f)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			for (int i = 0; i < groups.Length; i++)
			{
				float atPosition = DelayBetweenGroups * (float)i;
				sequence.Insert(atPosition, ShowVerticalIn(groups[i], duration, yOffset));
			}
			return sequence;
		}

		public static Sequence ShowVerticalIn(CanvasGroup group, float duration, float yOffset = -100f)
		{
			RectTransform component = group.GetComponent<RectTransform>();
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.AppendCallback(delegate
			{
				group.gameObject.SetActive(value: true);
				group.interactable = false;
			});
			LayoutElement layoutElement = group.GetComponent<LayoutElement>();
			if (layoutElement != null)
			{
				layoutElement.DOComplete(withCallbacks: true);
				float preferredWidth = layoutElement.preferredWidth;
				float preferredHeight = layoutElement.preferredHeight;
				sequence.AppendCallback(delegate
				{
					layoutElement.preferredWidth = 0f;
					layoutElement.preferredHeight = 0f;
				});
				sequence.Append(layoutElement.DOPreferredSize(new Vector2(preferredWidth, preferredHeight), duration));
			}
			sequence.Append(group.DOFade(0f, 0f));
			sequence.Append(group.DOFade(1f, duration));
			sequence.Insert(0f, component.DOAnchorPosY(component.anchoredPosition.y + yOffset, duration).SetEase(Ease.OutBack));
			sequence.AppendCallback(delegate
			{
				group.interactable = true;
			});
			return sequence;
		}

		public static Sequence HideVerticalOut(CanvasGroup[] groups, float duration, float yOffset = -100f, bool isSameInDirection = true)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			for (int i = 0; i < groups.Length; i++)
			{
				sequence.Insert(0f, HideVerticalOut(groups[i], duration, yOffset));
			}
			return sequence;
		}

		public static Sequence HideVerticalOut(CanvasGroup group, float duration, float yOffset = -100f, bool isSameInDirection = true)
		{
			RectTransform component = group.GetComponent<RectTransform>();
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.AppendCallback(delegate
			{
				group.interactable = false;
			});
			sequence.Append(group.DOFade(0f, duration));
			sequence.Insert(0f, component.DOAnchorPosY(component.anchoredPosition.y + yOffset, duration).SetEase(Ease.OutQuad));
			if (isSameInDirection)
			{
				sequence.Append(component.DOAnchorPosY(component.anchoredPosition.y - yOffset, 0f));
			}
			LayoutElement layoutElement = group.GetComponent<LayoutElement>();
			if (layoutElement != null)
			{
				float originalWidth = layoutElement.preferredWidth;
				float originalHeight = layoutElement.preferredHeight;
				sequence.Append(layoutElement.DOPreferredSize(new Vector2(0f, 0f), duration));
				sequence.AppendCallback(delegate
				{
					layoutElement.preferredWidth = originalWidth;
					layoutElement.preferredHeight = originalHeight;
				});
			}
			sequence.AppendCallback(delegate
			{
				group.gameObject.SetActive(value: false);
				group.interactable = true;
			});
			return sequence;
		}

		public static Sequence MoveHorizontal(CanvasGroup group, float duration, float xOffset)
		{
			RectTransform component = group.GetComponent<RectTransform>();
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.Append(component.DOAnchorPosX(component.anchoredPosition.x + xOffset, duration)).SetEase(Ease.OutQuad);
			return sequence;
		}

		public static Sequence MoveVertical(CanvasGroup group, float duration, float yOffset)
		{
			RectTransform component = group.GetComponent<RectTransform>();
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.Append(component.DOAnchorPosY(component.anchoredPosition.x + yOffset, duration)).SetEase(Ease.OutQuad);
			return sequence;
		}

		public static Sequence ShowMenu(CanvasGroup header, CanvasGroup[] groups, float duration)
		{
			float num = duration / (float)(groups.Length + 1);
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.Append(header.GetComponent<RectTransform>().DOAnchorPosX(-100f, 0f));
			sequence.Append(ShowHorizontalIn(header, num));
			sequence.Insert(DelayBetweenGroups, ShowVerticalIn(groups, duration - num));
			return sequence;
		}

		public static Sequence HideMenu(CanvasGroup header, CanvasGroup[] groups, float duration)
		{
			float num = duration / (float)(groups.Length + 1);
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.Append(header.GetComponent<RectTransform>().DOAnchorPosX(0f, 0f));
			sequence.Append(HideHorizontalOut(header, num));
			sequence.Insert(DelayBetweenGroups, HideVerticalOut(groups, duration - num));
			return sequence;
		}

		public static Sequence PaginateToLeft(CanvasGroup group, float duration, Action refreshAction)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.Append(HideHorizontalOut(group, duration * 0.5f, -500f, reposition: false));
			if (refreshAction != null)
			{
				sequence.AppendCallback(delegate
				{
					refreshAction();
				});
			}
			sequence.Append(MoveHorizontal(group, 0f, 1000f));
			sequence.Append(ShowHorizontalIn(group, duration * 0.5f, 0f));
			return sequence;
		}

		public static Sequence PaginateToRight(CanvasGroup group, float duration, Action refreshAction)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.Append(HideHorizontalOut(group, duration * 0.5f, 500f, reposition: false));
			if (refreshAction != null)
			{
				sequence.AppendCallback(delegate
				{
					refreshAction();
				});
			}
			sequence.Append(MoveHorizontal(group, 0f, -1000f));
			sequence.Append(ShowHorizontalIn(group, duration * 0.5f, 0f));
			return sequence;
		}

		public static Sequence ShowGroupFadeIn(CanvasGroup group, float duration)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.AppendCallback(delegate
			{
				group.blocksRaycasts = false;
			});
			sequence.Append(group.DOFade(1f, duration));
			sequence.AppendCallback(delegate
			{
				group.blocksRaycasts = true;
			});
			return sequence;
		}

		public static Sequence HideGroupFadeOut(CanvasGroup group, float duration)
		{
			Sequence sequence = DOTween.Sequence();
			sequence.SetId("UI");
			sequence.Append(group.DOFade(0f, duration));
			sequence.AppendCallback(delegate
			{
				group.blocksRaycasts = false;
			});
			return sequence;
		}
	}

