using UnityEngine;


	public class NativeBackButtonController : MonoBehaviour
	{
		public delegate void OnBackButtonClick();

		[SerializeField]
		private FilteredInputModule _filteredInputModule;

    private void Awake()
    {
		
    }

    private void Update()
		{
			if (!Input.GetKeyDown(KeyCode.Escape))
			{
				return;
			}
			if (_filteredInputModule.isFilterEnabled)
			{
				Quit();
				return;
			}		
			Quit();
			
		}

		public void Quit()
		{
		    GamePlaySytem.instance._saveSystem.Save();
			ShowQuitAlert();
		}

		public void ShowQuitAlert()
		{

		}

		private void OnQuitAlertClick(string text)
		{
		}
	}

