using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeesProgressSystem : MonoBehaviour
{
    private List<EmployeesProgressWidget> _widgets;

    private List<bool> _usage;

    private EmployeesProgressWidget _prefab;

    public Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        _widgets = new List<EmployeesProgressWidget>();
        _usage = new List<bool>();
        EmployeesProgressWidget[] components = GetComponentsInChildren<EmployeesProgressWidget>();

        foreach (EmployeesProgressWidget employeeProgressWidget in components)
        {
            if (_prefab == null)
                _prefab = employeeProgressWidget;
            RegisterWidget(employeeProgressWidget);
        }
    }

    private EmployeesProgressWidget CreateWidget()
    {
        //Debug.Log("NEW ONE");
        return UnityEngine.Object.Instantiate(_prefab, base.transform);
    }
    private EmployeesProgressWidget GetWidget(EmployeeController employee)
    {
        EmployeesProgressWidget employeeProgressWidget = FindWidget(employee, createNewOne: false);
        if (employeeProgressWidget == null)
        {
            for (int i = 0; i < _usage.Count; i++)
            {
                if (!_usage[i])
                {
                    employeeProgressWidget = _widgets[i];
                }
            }
        }
        if (employeeProgressWidget == null)
        {
            employeeProgressWidget = CreateWidget();
            RegisterWidget(employeeProgressWidget);
            employeeProgressWidget.AssociateWith(employee);
        }
        UseWidget(employeeProgressWidget);
        return employeeProgressWidget;
    }

    private void RegisterWidget(EmployeesProgressWidget widget)
    {
        widget.RaycastCamera = camera;
        _widgets.Add(widget);
        _usage.Add(item: false);
        widget.name = "EmployeeProgress_" +_widgets.IndexOf(widget);
        Hide(widget, 0f);
    }
    private EmployeesProgressWidget FindWidget(EmployeeController employee, bool createNewOne = true)
    {
        for (int i = 0; i < _widgets.Count; i++)
        {
           
            
                if (_widgets[i]._employee == employee)
                {
                    return _widgets[i];
                }
            
            
           
        }
        if (createNewOne)
        {
            return GetWidget(employee);
        }
        return null;
    }
    private void UseWidget(EmployeesProgressWidget widget)
    {
        int index = _widgets.IndexOf(widget);
        _usage[index] = true;
    }
    private void FreeWidget(EmployeesProgressWidget widget)
    {
        widget.Clean();
        int index = _widgets.IndexOf(widget);
        _usage[index] = false;
    }

    public EmployeesProgressWidget Show(EmployeeController employee)
    {
        EmployeesProgressWidget widget = GetWidget(employee);
        widget.AssociateWith(employee);
        Show(widget);
        return widget;
    }
    public void Show(EmployeesProgressWidget widget, float duration = 0.2f)
    {
        widget.gameObject.SetActive(value: true);
        widget.IsHiding = false;
        widget.Show(duration);
    }

    public void Hide(EmployeeController employee)
    {
        EmployeesProgressWidget employeeProgressWidget = FindWidget(employee, createNewOne: false);
        if (!(employeeProgressWidget == null))
        {
            Hide(employeeProgressWidget);
        }
    }

    public void HideEmployeee(EmployeeController employee)
    {
        EmployeesProgressWidget employeeProgressWidget = FindWidget(employee, false);
        if (!(employeeProgressWidget == null))
        {
            employeeProgressWidget.gameObject.SetActive(false);
            FreeWidget(employeeProgressWidget);
            employeeProgressWidget.IsHiding = false;
        }
    }
    public void Hide(EmployeesProgressWidget widget, float duration = 0.2f)
    {
        if (!widget.IsHiding)
        {
            widget.IsHiding = true;
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("Gameplay");
            if (duration > 0f)
            {
                sequence.Append(widget.Hide(duration));
                sequence.AppendCallback(delegate
                {
                    widget.gameObject.SetActive(value: false);
                    widget.IsHiding = false;
                    FreeWidget(widget);
                });
                return;
            }
            widget.Hide(duration);
            widget.gameObject.SetActive(value: false);
            FreeWidget(widget);
            widget.IsHiding = false;
        }
    }
    public void UpdateProgress(EmployeeController employee, float progress)
    {
        EmployeesProgressWidget employeeProgressWidget = FindWidget(employee);
        if (!(employeeProgressWidget == null))
        {
            employeeProgressWidget.SetProgress(progress);
        }
    }
}
