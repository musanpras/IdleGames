using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmployeeController : MonoBehaviour
{
    public enum ePhase
    {
        NONE,
        WAITING_PEOPLE,
        PRODUCING
    }
    [HideInInspector]
    public ePhase phase;
    [HideInInspector]
    public double timeToEndProduction;
    [HideInInspector]
    public double timeToProducting;
    [HideInInspector]
    public PeopleController peopleDispatching;
    [HideInInspector]
    public int Index;
    [HideInInspector]
    public GeneratorModel _model;
    [HideInInspector]
    public int _queueIndex;
    [HideInInspector]
    public float _nextUpdateWidget;
   // [HideInInspector]
    public bool isCheckoutEmployee = false;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isCheckoutEmployee)
         Tick(Time.deltaTime);
    }

    void Tick(float deltaTime)
    {
        if (isCheckoutEmployee)
            return;

        switch(phase)
        {
            case ePhase.NONE:
                break;
            case ePhase.WAITING_PEOPLE:
                WaitingCustomer();
                break;
            case ePhase.PRODUCING:
                //StartProducting(Time.deltaTime);
                StateProducing(Time.deltaTime);
                break;
        }
    }

    private void StateProducing(float deltaTime)
    {
        timeToEndProduction -= deltaTime;
        if (timeToEndProduction > 0f)
        {
            UpdateProduction((float)timeToEndProduction/(float)timeToProducting);
        }
        else
            EndProducing();
    }

    private void UpdateProduction(float progress)
    {
       _model.UpdateproductionStatus(this, progress);
    }

    public void Init(int queue)
    {
        _queueIndex = queue;
        _model = transform.parent.GetComponent<GeneratorModel>();
        timeToProducting = _model.TimeToProduce;
        timeToEndProduction = timeToProducting;
        WaitingCustomer();
    }

    void WaitingCustomer()
    {
        phase = ePhase.WAITING_PEOPLE;
    }

    

    void EndProducing()
    {
        timeToEndProduction = timeToProducting;
        WaitingCustomer();
        peopleDispatching.AddValue(_model.GetCurrentProduction());
        _model.ReleasePeopleInQueue(_queueIndex);
        _model.EndProduction(this);
      
    }

    public void CustomerArrived()
    {
        phase = ePhase.PRODUCING;
        _model.StartProduction(this);
        _nextUpdateWidget = 0.1f;
    }

    public void SupportCustomer(PeopleController people)
    {
        CustomerArrived();
        peopleDispatching = people;
    }

    public void Reset(EmployeeController _para)
    {
        timeToEndProduction = _para.timeToEndProduction;
        peopleDispatching = _para.peopleDispatching;
        phase = _para.phase;
        if(phase == ePhase.PRODUCING)
          _model.StartProduction(this);
    }


}
