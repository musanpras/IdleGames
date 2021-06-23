using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckoutModel : MonoBehaviour
{
    public enum ePhase
    {
        WAITING,
        PROCESSING
    }

    public ePhase _phase;

    private float processingTimeLeft;

    public float _processingDuration => 60.0f / ((float)GetCurrentProduction()*(1+ GamePlaySytem.instance._multiSpeedCheckout));


    public string id;

    public int level;

    public int MaxLevel, MaxQueue;

    [SerializeField]
    public Transform employees;

    [SerializeField]
    public EmployeeController _employee;

    [SerializeField]
    public Transform startQueue;

    public List<PeopleController> _queue = new List<PeopleController>();

    public ParticleSystem boostParticle;


    public Vector3 offsetQueuePos;

    private PeopleController _people;

    public GeneratorStaticModel _dataModel;

    public int ProductIndex;

   


    public void Init(GeneratorSaveModel _saveModel, GeneratorStaticModel _data)
    {
      
        level = _saveModel.level;
        _dataModel = _data;
        id = _data.id;
        ProductIndex = _saveModel.productIndex;
        MaxQueue = _data.max_queue;
        MaxLevel = _data.max_level;
        _phase = ePhase.WAITING;
       
       
    }


    public void Init(GeneratorStaticModel _data)
    {
        level = 0;
        _dataModel = _data;
        id = _data.id;
        MaxQueue = _data.max_queue;
        MaxLevel = _data.max_level;

    }

    // Update is called once per frame
    void Update()
    {
        Tick(Time.deltaTime);   
    }

    public PeopleController GetFirstCustomer()
    {
        if (_queue.Count > 0)
            return _queue[0];
        else
            return null;
    }


    public int GetOrderInQueue(Vector3 pos)
    {
        return Mathf.RoundToInt((startQueue.position - pos).z / offsetQueuePos.z);
    }

    public int OffsetPositionForCustomer(PeopleController people, int queueIndex)
    {
        int offset = 0;
        int orderFromPos = GetOrderInQueue(people.gameObject.transform.position);
        int orderFromQueue = _queue.IndexOf(people);
        if (orderFromQueue < orderFromPos)
        {
            offset = orderFromPos - orderFromQueue;
        }
        return offset;
    }

    public void AddPeopleToQueue(PeopleController _people)
    {
        if (_queue.Count < MaxQueue)
        {
            _queue.Add(_people);
        }
    }

    public void ReleasePeople(PeopleController _people)
    {
        _queue.Remove(_people);
        _people.peopleView.GoToExit();

        for (int i = 0; i < _queue.Count; i++)
        {
            if (_queue[i].peopleView.peopleController.status == PeopleController.eStatus.WAITING_IN_CHECKOUT_QUEUE)
                _queue[i].peopleView.GoToNextInQueueCheckout();
        }
    }

    public int GetQueueIndex()
    {
        if(_queue.Count < MaxQueue)
        return _queue.Count;
        else
        return -1;
    }


    public Vector3 GetCustomerPos()
    {
        int _peopleInQueue = _queue.Count;
        return -offsetQueuePos * _peopleInQueue + startQueue.position;
    }

    protected void OnDrawGizmos()
    {
        Vector3 _queueSlot = GetCustomerPos();
        Gizmos.color = Color.green;
        float num = 0.35f;
        float num2 = 0.35f;
        Vector3 vector = _queueSlot + new Vector3(num, 0f, num2);
        Vector3 vector2 = _queueSlot + new Vector3(num, 0f, 0f - num2);
        Vector3 vector3 = _queueSlot + new Vector3(0f - num, 0f, 0f - num2);
        Vector3 vector4 = _queueSlot + new Vector3(0f - num, 0f, num2);
        Gizmos.DrawLine(vector, vector2);
        Gizmos.DrawLine(vector2, vector3);
        Gizmos.DrawLine(vector3, vector4);
        Gizmos.DrawLine(vector4, vector);
        //Gizmos.DrawLine(base.transform.position, Car.transform.position);
    }

    private void UpdateProcessing()
    {
        float progress = processingTimeLeft / _processingDuration;
        UpdateproductionStatus( progress);
    }

    private void EndProcessing()
    {
        _phase = ePhase.WAITING;
        EndProduction();
        MarketSystem._instance.CheckOut(_people);
        ReleasePeople(_people);
    }

    private void StartProcessing()
    {
        _phase = ePhase.PROCESSING;
        processingTimeLeft = _processingDuration;
        StartProduction();
    }

    public void SupportCustomer(PeopleController people)
    {
        _people = people;
        StartProcessing();
    }

    public void Tick(float deltaTime)
    {

        switch(_phase)
        {
            case ePhase.WAITING:
                break;
            case ePhase.PROCESSING:

                processingTimeLeft -= deltaTime;

                if (processingTimeLeft > 0f)
                {
                    UpdateProcessing();
                }
                else
                    EndProcessing();

                break;
        }

    }

    public void StartProduction()
    {
        UISystem.instance._employeesProgressSystem.Show(_employee);
       // Debug.Log("SHOW WIDGET");
    }

    public void UpdateproductionStatus(float progress)
    {
        UISystem.instance._employeesProgressSystem.UpdateProgress(_employee,progress);
    }

    public void EndProduction()
    {
        UISystem.instance._employeesProgressSystem.Hide(_employee);
    }


    public IEnumerator SortAllPeopleInQueueIE()
    {
        for (int i = 0; i < _queue.Count; i++)
        {
            
                if (_queue[i].status == PeopleController.eStatus.WAY_TO_CHECKOUT_QUEUE ||
                    _queue[i].status == PeopleController.eStatus.EDITTING_POS_IN_CHECKOUT_QUEUE ||
                    _queue[i].status == PeopleController.eStatus.WAITING_IN_CHECKOUT_QUEUE)
                {
                    _queue[i].peopleView.StandInCheckoutQueue();
                    yield return new WaitForSeconds(0.03f);
                }
            
        }
    }

    public void SortAllPeopleInQueue()
    {
        StartCoroutine(SortAllPeopleInQueueIE());
    }

    public void SetPeoplePos(PeopleController people)
    {
        int _peopleInQueue = _queue.Count;
        int orderFromQueue = _queue.IndexOf(people);
        Vector3 _pos = - offsetQueuePos * orderFromQueue + startQueue.position;
        people.transform.position = _pos;
    }


    public double GetCurrentProductionForLevel(int level)
    {
        if (level == 0)
            return 0.0;
        return _dataModel.start_sell_price * (1.0 + _dataModel.grow_production * (double)(level - 1));
    }

    public double GetCurrentProduction()
    {
        return GetCurrentProductionForLevel(level) * GetMultiplierForProduct(FindProductIndexForLevel(level));
    }

    public float GetMultiplierForProduct(int productIndex)
    {
        productIndex = Mathf.Clamp(productIndex, 0, _dataModel.bonusParameter.Count - 1);
        return _dataModel.bonusParameter[productIndex];
    }

    private int FindProductIndexForLevel(int level)
    {
        int result = 0;
        for (int i = 0; i < _dataModel.bonusAtLevel.Count; i++)
        {
            if (level >= _dataModel.bonusAtLevel[i])
                result = i;
        }
        return result;
    }
    public int FindIndexForLevel(int level)
    {
        return FindProductIndexForLevel(level);
    }

    public double GetLevelUpPrice(int level)
    {
        if (level == 0)
            return _dataModel.build_cost;
        double num = 1.0f;
        return _dataModel.build_cost * Math.Pow(1.0 + _dataModel.grow_cost, level - 1) * num;

    }
    public bool BuildOrLevelUp(bool changeVisuals = true)
    {
        int productIndex = FindIndexForLevel(level);

        ProductIndex = productIndex;

        return false;
    }
}
