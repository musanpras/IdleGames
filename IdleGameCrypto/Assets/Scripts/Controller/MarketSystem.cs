using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketSystem : MonoBehaviour
{
    protected bool IsCarSpawnEnabled;

    float timeToEndProduction;

    public static MarketSystem _instance;

    public MarketView _view;

    public GameObject _carPrefab;

    public List<CarController> _carSystemList = new List<CarController>();

    public List<PeopleController> _peopleSystemList = new List<PeopleController>();

    public MarketVisualFeedbackSystem VisualFeedback;

    private void Awake()
    {
        _instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        _view.Init();
        IsCarSpawnEnabled = true;
    }


    public void Tick(float deltaTime)
    {

        CalculateProduct(deltaTime);
    }


    private void CalculateProduct(float _tick)
    {
        if (timeToEndProduction > 0)
            timeToEndProduction -= _tick;
        else
        {
            if (IsCarSpawnEnabled && !_view._parkingVisualModel.QueueFull())
            {
                ProduceCar();
            }
        }

    }

    private void ProduceCar()
    {
        CreateNewCarWithPeopleParked();
        timeToEndProduction = _view._parkingVisualModel._timeToProduce;
        //Debug.Log("END CAR " + _view._parkingVisualModel._timeToProduce);
    }

    void CreateNewCarWithPeopleParked()
    {
        
        CarController carController = GameObject.Instantiate(_carPrefab).GetComponent<CarController>();
        carController.CreateNormalCarView();
        _carSystemList.Add(carController);
    }


    public void CheckOut(PeopleController people)
    {
        people.PayFX();
        AddMoney(people.accumulatedCash * (1 + GamePlaySytem.instance._increaseCityEarning) * (1 + GamePlaySytem.instance._doubleMoney), people.gameObject.transform.position );
    }

    public void AddMoney(double amount, Vector3 _pos)
    {
        _view.money += amount;
        VisualFeedback.ShowMoneyFeedback(amount, _pos);
    }



    public void AddGems(int amount)
    {
        _view.gems += amount;
    }

    public double GetMoney()
    {
        return _view.money;
    }



}
