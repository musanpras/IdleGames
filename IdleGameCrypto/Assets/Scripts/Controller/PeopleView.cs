using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;
using Pathfinding;
using UnityEngine.Events;
using System.Linq;

public class PeopleView : MonoBehaviour
{
    public PeopleController peopleController;

    public CarMovementView _car;

    public PathManager _pathManager;

    public Transform[] pathToCar;

    public List<Transform> pathToMarket = new List<Transform>();

    public List<Transform> staticPath = new List<Transform>();

    public splineMove _moveController;

    public Seeker _seeker;

    public int queueIndex;

    public int checkoutIndex;

    public int _offInQueue = 0;

    public int orderFromPos;

    public int orderFromQueue;

    public int shopStep;

    // Start is called before the first frame update
    void Start()
    {
        _moveController.speed = _moveController.speed * (1.0f + GamePlaySytem.instance._multiSpeedClient);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(CarMovementView para)
    {
        _car = para;
        shopStep = 0;
        LoadPathToMarket();
    }


    void LoadPathToMarket()
    {
        ClearPath();
        staticPath.Add(transform);

        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex].GetSlot(_car.slotParkingIndex).WalkPoint.position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex]._walkway._walkwayPath[2].position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex]._walkway._walkwayPath[1].position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex]._walkway._walkwayPath[0].position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._currentViewModel.exit.position));
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(SearchPathToQueueShop);
        _moveController.AddEvent(_pathManager.waypoints.Length - 1, m_MyEvent);
        peopleController.status = PeopleController.eStatus.WAY_TO_MARKET;
    }

    public void SearchPathToQueueShop()
    {
        if (MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.GetQueueIndexAvailable() < 0)
        {
            peopleController.AngryFX();
            GoToExit();
            return;
        }
           
        _seeker.StartPath(transform.position, MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.GetCustomerPos(), new OnPathDelegate(OnPathToQueueShopComplete));
    }


    public void SearchPathToQueueCheckout()
    {
        checkoutIndex = MarketSystem._instance._view.checkOutSystem.GetCheckoutIndex();
       // Debug.Log("CHECK OUT INDEX " + checkoutIndex);
        if (checkoutIndex == -1)
            return;
        if (MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].GetQueueIndex() < 0)
            return;
        Debug.Log("POS " + MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].GetCustomerPos());
        _seeker.StartPath(transform.position, MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].GetCustomerPos(), new OnPathDelegate(OnPathToQueueCheckoutComplete));
    }


    void OnPathToQueueCheckoutComplete(Path _p)
    {
        
        GetQueueSlot();
        ClearPath();
        staticPath.Add(transform);

        //staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].GetCustomerPos()));

        for (int i = 0; i < _p.vectorPath.Count; i++)
        {
            staticPath.Add(CreatTransfromFrom(_p.vectorPath[i]));
        }
        
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(WaitInCheckOutQueue);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
        peopleController.status = PeopleController.eStatus.WAY_TO_CHECKOUT_QUEUE;


    }

    void WaitInCheckOutQueue()
    {
        _offInQueue = MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].OffsetPositionForCustomer(peopleController, queueIndex);
        orderFromPos = MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].GetOrderInQueue(transform.position);
        orderFromQueue = MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex]._queue.IndexOf(peopleController);
        if (_offInQueue > 0)
        {
            EditPosInCheckoutQueue();
            return;
        }
        peopleController.status = PeopleController.eStatus.WAITING_IN_CHECKOUT_QUEUE;

        if (MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].GetFirstCustomer() == peopleController)
        {
            //Debug.Log("PRODUCING");
            MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].SupportCustomer(peopleController);
        }

    }


    void GetQueueSlot()
    {
        MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].AddPeopleToQueue(peopleController);
    }

    public void EditPosInCheckoutQueue()
    {

        ClearPath();
        staticPath.Add(transform);
        staticPath.Add(CreatTransfromFrom((MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].offsetQueuePos * _offInQueue + transform.position)));
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(WaitInCheckOutQueue);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
        peopleController.status = PeopleController.eStatus.EDITTING_POS_IN_CHECKOUT_QUEUE;


    }

    void WaitInShopQueue()
    {
        _offInQueue = MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.OffsetPositionForCustomer(peopleController, queueIndex);
        orderFromPos = MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.GetOrderInQueue(transform.position, queueIndex);
      
        if (_offInQueue > 0)
        {
            EditPosInQueue();
            return;
        }
        peopleController.status = PeopleController.eStatus.WAITING_IN_QUEUE_SHOP;
        if (MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.GetFirstCustomer(queueIndex) == peopleController)
        {
          //  Debug.Log("PRODUCING");
            MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.GetEmployeeByQueueIndex(queueIndex).SupportCustomer(peopleController);
        }
        
        

    }

    public void StandInShopQueue()
    {
        _moveController.Stop();
        _moveController.pathContainer.ClearWaypoint();
        MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.SetPeoplePos(this.peopleController, queueIndex);
        WaitInShopQueue();
    }

    public void StandInCheckoutQueue()
    {
        _moveController.Stop();
        _moveController.pathContainer.ClearWaypoint();
        MarketSystem._instance._view.checkOutSystem.checkOutList[checkoutIndex].SetPeoplePos(this.peopleController);                  
        WaitInCheckOutQueue();


    }

    void GetSlot()
    {
        queueIndex = MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.GetQueueIndexAvailable();
        orderFromQueue = MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value._queueList[queueIndex].peopleInQueue.IndexOf(peopleController);
        MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.AddPeopleToQueue(peopleController, queueIndex);
       
    }


    public void GoToNextInQueue()
    {
        ClearPath();
        staticPath.Add(transform);
        staticPath.Add(CreatTransfromFrom((- MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.offetQueuePos + transform.position) ));
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(WaitInShopQueue);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
        peopleController.status = PeopleController.eStatus.EDITTING_POS_IN_QUEUE;
    }


    public void GoToNextInQueueCheckout()
    {
        ClearPath();
        staticPath.Add(transform);
        staticPath.Add(CreatTransfromFrom((MarketSystem._instance._view.checkOutSystem.checkOutList[0].offsetQueuePos + transform.position)));
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(WaitInCheckOutQueue);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
        peopleController.status = PeopleController.eStatus.EDITTING_POS_IN_CHECKOUT_QUEUE;
    }

    public void EditPosInQueue()
    {
        
            ClearPath();
            staticPath.Add(transform);
            staticPath.Add(CreatTransfromFrom((-MarketSystem._instance._view.generatorControllers.ElementAt(shopStep).Value.offetQueuePos * _offInQueue + transform.position)));
            _pathManager.Create(staticPath.ToArray());
            _moveController.SetPath(_pathManager);
            UnityEvent m_MyEvent = new UnityEvent();
            m_MyEvent.AddListener(WaitInShopQueue);
            _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
            peopleController.status = PeopleController.eStatus.EDITTING_POS_IN_QUEUE;
        
        
    }

    void OnPathToQueueShopComplete(Path _p)
    {
        GetSlot();
        ClearPath();
        staticPath.Add(transform);
        for (int i = 0; i < _p.vectorPath.Count; i ++)
        {
            staticPath.Add(CreatTransfromFrom(_p.vectorPath[i]));
        }
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(WaitInShopQueue);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
        peopleController.status = PeopleController.eStatus.WAY_TO_SHOP_QUEUE;
       
    }

    Transform CreatTransfromFrom(Vector3 pos)
    {
        Transform _t = new GameObject("point").transform;
        //transform.position = new Vector3(transform.position.x, 0.0f, transform.position.z);
        _t.position = pos;
        //_t.rotation = Quaternion.Euler(0, 0, 0);
        return _t;
    }

    void ClearPath()
    {
        foreach (Transform _t in staticPath)
        {
            if (_t != transform)
                Destroy(_t.gameObject);
        }
           
        staticPath.Clear();
    }

    void ResetRotationPath()
    {
        foreach (Transform _t in staticPath)
        {
            if (_t != transform)
                _t.rotation = Quaternion.Euler(0, 0, 0);
        }

       
    }

    public void GoToExit()
    {
        SearchPathToExit();
    }


    public void SearchPathToExit()
    {
        _seeker.StartPath(transform.position, MarketSystem._instance._view._currentViewModel.exit.position, new OnPathDelegate(OnPathToExitComplete));
    }

    void OnPathToExitComplete(Path _p)
    {
        ClearPath();
        staticPath.Add(transform);
        for (int i = 0; i < _p.vectorPath.Count; i++)
        {
            staticPath.Add(CreatTransfromFrom(_p.vectorPath[i]));
        }

        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex]._walkway._walkwayPath[0].position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex]._walkway._walkwayPath[1].position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex]._walkway._walkwayPath[2].position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex].GetSlot(_car.slotParkingIndex).WalkPoint.position));
        ResetRotationPath();
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(GoInCar);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
        peopleController.status = PeopleController.eStatus.WAY_TO_PARKING;

    }

    void GoInCar()
    {
        _car.ExitTheParking();
        MarketSystem._instance._peopleSystemList.Remove(peopleController);
        Destroy(gameObject);
        //GameObjectPool._instance.unloadObject(this.gameObject);
    }    

    public void ForceToReturnCar()
    {
        _moveController.Stop();
        transform.position = MarketSystem._instance._view._parkingVisualModel.Lanes[_car.laneParkingIndex].GetSlot(_car.slotParkingIndex).WalkPoint.position;
        if (peopleController.status == PeopleController.eStatus.WAY_TO_MARKET)
            LoadPathToMarket();
        else if (peopleController.status == PeopleController.eStatus.WAY_TO_PARKING)
            GoInCar();
    }
}
