
using System;
using UnityEngine;


	public class BasicMenu : MonoBehaviour
	{
		//public UIElement uiElement;

		public Canvas canvas;

	
		///protected MenuController _menuController;

		private bool _isInitialized;

		[Obsolete("Use IsVisible instead")]
		public bool isVisible
		{
			get;
			private set;
		}

		public void Show()
		{
		}

		public void Hide()
		{
		}

		public void ShowNow()
		{
		}

		public void HideNow()
		{
		}

		private void OnApplicationQuit()
		{
			isVisible = false;
			HandleApplicationQuit();
		}

		protected virtual void HandleApplicationQuit()
		{
		}

		private void OnDisable()
		{
			HideNow();
		}

		public virtual void Initialize()
		{
			if (!_isInitialized)
			{
				_isInitialized = true;
				DoInitialize();
				InitializeInternal();
			}
		}

		protected virtual void InitializeInternal()
		{
		}

		private void DoInitialize()
		{
			
		}

		public virtual void SetModel(object model)
		{
		}

		public virtual object GetModel()
		{
			return null;
		}

		public void GoToLastMainMenu()
		{
			//_menuController.GoToLastMainMenu();
		}

		public virtual void GetFocus()
		{
		}

		public virtual void LoseFocus()
		{
		}

		public void OnCloseButtonClick()
		{
			//if (_menuController.currentMenu == this && _menuController.PeekLastMainMenu() != null)
			{
				//GoToLastMainMenu();
			}
		}

		public virtual void OnNativeBackButtonClick()
		{
			OnCloseButtonClick();
		}

		private void OnQuitAlertClick(string text)
		{
		}
	}

