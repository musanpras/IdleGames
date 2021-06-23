using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


	public abstract class MarketMenu : BasicMenu
	{
		[Header("Market Menu")]
		[SerializeField]
		protected bool RefreshEachSecond;

		[SerializeField]
		protected Button BackButton;

		[SerializeField]
		protected bool StartsHidden = true;


		public bool IsInitialized
		{
			get;
			set;
		}

		public bool IsSubscribedToEvents
		{
			get;
			private set;
		}

		public bool IsVisible
		{
			get;
			private set;
		}

		public bool IsTransitioning
		{
			get;
			private set;
		}

		

		public override void Initialize()
		{
			if (!IsInitialized)
			{
				base.Initialize();
				//OnOpened.RemoveAllListeners();
				//OnClosed.RemoveAllListeners();
				if (StartsHidden)
				{
					HideAnimated(0f);
				}
				IsInitialized = true;
			}
		}

		public Sequence ShowAnimated(float duration = 0.2f)
		{
			Sequence sequence = ShowAnimatedInternal(duration);
			sequence.PrependCallback(delegate
			{
				//UnityEngine.Debug.Log("#UI# Shown menu " + base.name);
				IsVisible = true;
				IsTransitioning = true;
			});
			sequence.AppendCallback(delegate
			{
				IsTransitioning = false;
				//OnOpened.Invoke(this);
			});
			return sequence;
		}

		public Sequence HideAnimated(float duration = 0.2f)
		{
			Sequence sequence = HideAnimatedInternal(duration);
			sequence.PrependCallback(delegate
			{
				IsTransitioning = true;
			});
			sequence.AppendCallback(delegate
			{
				UnityEngine.Debug.Log("#UI# Hide menu " + base.name);
				IsVisible = false;
				IsTransitioning = false;
				//OnClosed.Invoke(this);
			});
			return sequence;
		}

		protected abstract Sequence ShowAnimatedInternal(float duration = 0.2f);

		protected abstract Sequence HideAnimatedInternal(float duration = 0.2f);

		public void SubscribeToEvents()
		{

		   
	     	if (IsSubscribedToEvents)
			{
				UnSubscribeToEvents();
			}
			if (BackButton != null)
			{
			   BackButton.onClick.AddListener(OnBackButtonClicked);
			   
	     	}
			if (RefreshEachSecond)
			{
				//MessageBus.Subscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsed);
			}
			SubscribeToEventsInternal();
			IsSubscribedToEvents = true;
		}

		public void UnSubscribeToEvents()
		{
			if (IsSubscribedToEvents)
			{
				if (BackButton != null)
				{
					BackButton.onClick.RemoveListener(OnBackButtonClicked);
				}
				if (RefreshEachSecond)
				{
					//MessageBus.UnSubscribe<ServerTimeController.SecondElapsedEvent>(OnSecondElapsed);
				}
				UnSubscribeToEventsInternal();
				IsSubscribedToEvents = false;
			}
		}

		protected abstract void SubscribeToEventsInternal();

		protected abstract void UnSubscribeToEventsInternal();

		public virtual void OnBackButtonClicked()
		{
		   UISystem.instance.HideCurrentMenu();
	    }

		public override void OnNativeBackButtonClick()
		{
			OnBackButtonClicked();
		}

	
	}

