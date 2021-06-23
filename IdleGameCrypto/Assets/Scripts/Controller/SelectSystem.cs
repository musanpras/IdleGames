using HedgehogTeam.EasyTouch;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class SelectSystem : MonoBehaviour
{
    private LayerMask _layerMask;
    private void Awake()
    {
        Init();
    }
    void Init()
    {
        _layerMask = 1 << LayerMask.NameToLayer("Gameplay");
        SubcribeToEvents();
    }

    private void SubcribeToEvents()
    {

        EasyTouch.On_SimpleTap += HandleOn_SimpleTap;
        
    }

    private void HandleOn_SimpleTap(Gesture gesture)
    {
        Vector3 vector = gesture.position;
        if(!IsPointerOverUiObject(vector))
        {
            DoClick(vector);
        }
    }

    private void DoClick(Vector3 clickPos)
    {
        RaycastHit hitInfo;

        if( !(EventSystem.current == null) 
            && !EventSystem.current.IsPointerOverGameObject()
            && Physics.Raycast(Camera.main.ScreenPointToRay(clickPos), out hitInfo, float.MaxValue, _layerMask)
            && hitInfo.transform != null)

        {
            SelectController component = hitInfo.transform.GetComponent<SelectController>();
            if(component != null)
            {
                ClickSelectController(component);
            }
        }
    }

    private void ClickSelectController(SelectController selectController)
    {
        if(selectController != null)
        {
            switch(selectController.type)
            {
                case SelectController.eType.ZONE:
                    OnZoneClick(selectController, false);
                    break;
                case SelectController.eType.CHECKOUT:
                    OnCheckOutClick(selectController, false);
                    break;
                case SelectController.eType.PARKING:
                    OnParkingClick(selectController);
                    break;
                case SelectController.eType.OFFICE:
                    OnOfficeClick(selectController,false);
                    break;
                default:
                    break;
            }
        }
    }

    private void OnZoneClick(SelectController selectController, bool isSingleTap)
    {
        GeneratorModel component = selectController.GetComponent<GeneratorModel>();

        UISystem.instance.ShowSellingPointMenu(component);
    }

    private void OnParkingClick(SelectController _select)
    {
        ParkingVisualModel component = _select.GetComponent<ParkingVisualModel>();
        UISystem.instance.ShowParkingMenu(component);
    }

    private void OnCheckOutClick(SelectController selectController, bool isSingleTap)
    {
        UISystem.instance.ShowCashierMenu();
    }

    private void OnOfficeClick(SelectController selectController, bool isSingleTap)
    {
        UISystem.instance.ShowOfficeManagerMenu();
    }

    private bool IsPointerOverUiObject(Vector2 tapPos)
    {
        new PointerEventData(EventSystem.current).position = new Vector2(tapPos.x, tapPos.y);
        return new List<RaycastResult>().Count > 0;
    }
}
