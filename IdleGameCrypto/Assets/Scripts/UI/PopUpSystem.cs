using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpSystem : MonoBehaviour
{
    [SerializeField]
    private WatchVipVideoPopup _watchVipVideoPopup;
    [SerializeField]
    private GameObject _background;

    private GameObject _currentPopup;


    
    public WatchVipVideoPopup ShowWatchVipVideoPopup()
    {
        ShowPopup(_watchVipVideoPopup.gameObject);
        return _watchVipVideoPopup;
    }


    public void ShowPopup(GameObject _Popup)
    {
        WatchVipVideoPopup _popup = _Popup.GetComponent<WatchVipVideoPopup>();
        _popup._closeButton.onClick.AddListener(HidePopup);
        _popup.Init(false);
        _currentPopup = _Popup;
        _background.SetActive(true);
        _Popup.SetActive(true);
    }

    public void HidePopup()
    {
        _currentPopup.GetComponent<WatchVipVideoPopup>()._closeButton.onClick.RemoveListener(HidePopup);
        _currentPopup.SetActive(false);
        _background.SetActive(false);
    }
}
