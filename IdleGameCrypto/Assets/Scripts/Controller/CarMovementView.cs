
using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarMovementView : MonoBehaviour
{

    public enum Phase
    {
        None,
        TravelingToParking,
        ParkingIn,
        Parked,
        ParkingOut,
        TravelingToQueue,
        WaitingOnParkingQueue,
        EditPositionInQueue,
        TravelingToVipSlot,
        ParkedInVipSlot,
        ExitParking,
        Exit

    }


    // protected UnitWalkSystem _unitWalkSystem;

    public readonly int NO_QUEUE_SLOT_INDEX = -1;

    public int laneParkingIndex, slotParkingIndex, waitingInQueueIndex;

    public splineMove _moveController;

    public PathManager _pathManager;

    public Phase currentPhase;

    public List<Transform> staticPath = new List<Transform>();

    public Transform[] exitPath;

    public GameObject peoplePrefab;

    public CarController _carController;

    private void Awake()
    {
        currentPhase = Phase.None;
    }

    private void Start()
    {
        CheckPath();

    }

    private void Update()
    {
        if(waitingInQueueIndex == 0 && IsParkingSlotAvailable() && currentPhase == Phase.WaitingOnParkingQueue)
        {
            currentPhase = Phase.TravelingToParking;
            MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].Free();
            SetParkingSlotIndex();
            LoadPathToParking();
        }
        else if(waitingInQueueIndex > 0)
        {
            if (currentPhase == Phase.WaitingOnParkingQueue && !MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex - 1].IsUsed)
            {
                NextQueueSlot();
            }
        }
       
    }

    void CheckPath()
    {

        //  bool queueWaiting = true, parkingSlotAvailable = false;


        int queueUseCount = 0;
        for (int i = 0; i < MarketSystem._instance._view._parkingVisualModel.QueueSlots.Length; i++)
        {
            if (MarketSystem._instance._view._parkingVisualModel.QueueSlots[i].IsUsed)
            {
                queueUseCount++;
                // break;
            }
        }

        if (queueUseCount > 0)
        {
            waitingInQueueIndex = queueUseCount;
            MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].Use(GetComponent<CarMovementView>()); ;
            LoadPathToQueue();
            Debug.Log("Waiting...In Queue " + waitingInQueueIndex + " total" + MarketSystem._instance._view._parkingVisualModel.QueueSlots.Length);
            return;
        }



        for (int i = 0; i < MarketSystem._instance._view._parkingVisualModel.Lanes.Length; i++)
        {

            if (MarketSystem._instance._view._parkingVisualModel.Lanes[i].HasSlotAvailable)
            {
                laneParkingIndex = i;
                slotParkingIndex = MarketSystem._instance._view._parkingVisualModel.Lanes[i].GetSlotAvailableIndex;
                LoadPathToParking();
              //  Debug.Log("Going...to park");
                return;
            }
        }
        waitingInQueueIndex = 0;
        MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].Use(GetComponent<CarMovementView>());
        LoadPathToQueue();
     //   Debug.Log("Waiting...In Queue 2");

    }

    public bool IsParkingSlotAvailable()
    {

        for (int i = 0; i < MarketSystem._instance._view._parkingVisualModel.Lanes.Length; i++)
        {

            if (MarketSystem._instance._view._parkingVisualModel.Lanes[i].HasSlotAvailable)
            {
                return true;
            }
        }
        return false;
    }

    public void SetParkingSlotIndex()
    {


        for (int i = 0; i < MarketSystem._instance._view._parkingVisualModel.Lanes.Length; i++)
        {

            if (MarketSystem._instance._view._parkingVisualModel.Lanes[i].HasSlotAvailable)
            {
                laneParkingIndex = i;
                slotParkingIndex = MarketSystem._instance._view._parkingVisualModel.Lanes[i].GetSlotAvailableIndex;
                MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].UseSlotByIndex(slotParkingIndex);
                return;
               
            }
        }

    }

    public void LoadPathToQueue()
    {
        currentPhase = Phase.TravelingToQueue;
        ClearPath();
        staticPath.Add(transform);
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.CarSpawnPoint.position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].waitPos.position));
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);

        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(WaitingInTheQueue);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
    }

    public void NextQueueSlot()
    {
        if (waitingInQueueIndex > 0)
        {
            MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].Free();
            waitingInQueueIndex--;
            MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].Use(GetComponent<CarMovementView>());
            currentPhase = Phase.EditPositionInQueue;
            ClearPath();
            staticPath.Add(transform);
            staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].waitPos.position));
            _pathManager.Create(staticPath.ToArray());
            _moveController.SetPath(_pathManager);

            UnityEvent m_MyEvent = new UnityEvent();
            m_MyEvent.AddListener(WaitingInTheQueue);
            _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);

        }
        else
            WaitingInTheQueue();
       
    }

    void WaitingInTheQueue()
    {
        currentPhase = Phase.WaitingOnParkingQueue;
    }

    public void LoadPathToParking()
    {
        currentPhase = Phase.TravelingToParking;
        SetParkingSlotIndex();
        ClearPath();
        staticPath.Add(transform);
        CurvePos(staticPath, transform.position, MarketSystem._instance._view._parkingVisualModel.EntryPath[1].position, MarketSystem._instance._view._parkingVisualModel.EntryPath[0].position);
;
        CurvePos(staticPath, MarketSystem._instance._view._parkingVisualModel.EntryPath[0].position, MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].InPoint.position
            , MarketSystem._instance._view._parkingVisualModel.EntryPath[1].position);
        CurvePos(staticPath, MarketSystem._instance._view._parkingVisualModel.EntryPath[1].position,
             MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).InPoint.position,
             MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].InPoint.position
            );
        CurvePos(staticPath,
            MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].InPoint.position,
            MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).ParkedPoint.position,
            new Vector3(MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).InPoint.position.x,
            MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).InPoint.position.y,
            MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).ParkedPoint.position.z)
           
            );
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).ParkedPoint.position));
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(SpawnPeople);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
    }

    public void ForceToPark()
    {
      
        transform.position = MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).ParkedPoint.position;
        if(MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).ParkedPoint.position.x 
            > MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).InPoint.position.x)
        transform.rotation = Quaternion.Euler(0, 90, 0);
        else
            transform.rotation = Quaternion.Euler(0, -90, 0);
        currentPhase = Phase.Parked;
         MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].UseSlotByIndex(slotParkingIndex);
        MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex)._associateWithCar = true;
        GamePlaySytem.instance.marketSystem._view._parkingVisualModel.UsageDisplayRefresh();

    }

    public void FreeParkingSlot()
    {
        MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].FreeSlot(MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex));
    }    

  

    public void ForceToQueuePos()
    {
        /*
        if(currentPhase == Phase.EditPositionInQueue || currentPhase == Phase.TravelingToQueue)
        {
            _moveController.Stop();
            transform.position = MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].waitPos.position;

        }
        else if(currentPhase == Phase.WaitingOnParkingQueue)
        {
            transform.position = MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].waitPos.position;
        }
        currentPhase = Phase.WaitingOnParkingQueue;
        MarketSystem._instance._view._parkingVisualModel.QueueSlots[waitingInQueueIndex].Use(GetComponent<CarMovementView>());
        */
    }

    public void CurvePos(List<Transform> _path, Vector3 BNode,Vector3 ANode,Vector3 pos)
    {
        Vector3 CurveStartPoint = Vector3.MoveTowards(pos, BNode, 1f);
        Vector3 CurveEndPoint = Vector3.MoveTowards(pos, ANode, 1f);

        float t = 0f;
        Vector3 position = Vector3.zero;
        for (int i = 0; i < 20; i++)
        {
            t = i / (20 - 1.0f);
            position = (1.0f - t) * (1.0f - t) * CurveStartPoint + 2.0f * (1.0f - t) * t * pos + t * t * CurveEndPoint;

           
            _path.Add(CreatTransfromFrom(position));
        }
        
    }    

    public void GoToExit()
    {
        currentPhase = Phase.Exit;
     
        ClearPath();
        staticPath.Add(transform);

        CurvePos(staticPath, MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).InPoint.position,
            MarketSystem._instance._view._parkingVisualModel.ExitPath[0].position,
            MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].OutPoint.position);
       

        CurvePos(staticPath, MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].OutPoint.position,
            MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position,
            MarketSystem._instance._view._parkingVisualModel.ExitPath[0].position);

        CurvePos(staticPath, MarketSystem._instance._view._parkingVisualModel.ExitPath[0].position,
            new Vector3(MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.x + 100, MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.y,
           MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.z),MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position);
        staticPath.Add(CreatTransfromFrom(
           new Vector3(MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.x + 100, MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.y,
           MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.z)
           ));

      
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);
       
       
        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(DestroyCar);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
    }

    public void ExitTheParking()
    {
        MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex)._associateWithCar = false;
        GamePlaySytem.instance.marketSystem._view._parkingVisualModel.UsageDisplayRefresh();
        currentPhase = Phase.ExitParking;

        ClearPath();
        staticPath.Add(transform);

        CurvePos(staticPath,
             MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).ParkedPoint.position,
           MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].InPoint.position, 
           new Vector3(MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).InPoint.position.x,
           MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).InPoint.position.y,
           MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).ParkedPoint.position.z)

           );

        _pathManager.Create(staticPath.ToArray());
        LockRotate();
        _moveController.SetPath(_pathManager);


        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(UnlockRotate);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);

        MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].FreeSlot(MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex));

    }    

    public void DestroyCar()
    {
        MarketSystem._instance._carSystemList.Remove(_carController);
        Destroy(gameObject);
    }

    public void LockRotate()
    {
        // _moveController.lockRotation = DG.Tweening.AxisConstraint.Y;
        transform.localScale = new Vector3(1, 1, -1);
    }

    public void UnlockRotate()
    {
        //_moveController.lockRotation = DG.Tweening.AxisConstraint.None;
        transform.localScale = new Vector3(1, 1, 1);
        GoToExit();
    }

    public void LoadPathFromParkingToQueue()
    {

    }

    void StartRun()
    {
        //_moveController.StartMove();
       // _moveController.moveToPath = true;
    }

    void SpawnPeople()
    {
        currentPhase = Phase.Parked;
        MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex)._associateWithCar = true;
        GamePlaySytem.instance.marketSystem._view._parkingVisualModel.UsageDisplayRefresh();
        StartCoroutine(SpawnPeopleIE());
    }

    IEnumerator SpawnPeopleIE()
    {
        yield return new WaitForSeconds(1.0f);
        // GameObject people = GameObjectPool._instance.getObject("PeopleView");
        GameObject people = GameObject.Instantiate(peoplePrefab);
        people.transform.position = MarketSystem._instance._view._parkingVisualModel.Lanes[laneParkingIndex].GetSlot(slotParkingIndex).WalkPoint.position;
        PeopleView peopleView = people.GetComponent<PeopleView>();
        peopleView.Initialize(this);
        peopleView._car = this;
        MarketSystem._instance._peopleSystemList.Add(peopleView.peopleController);
    }


    Transform CreatTransfromFrom(Vector3 pos)
    {
        Transform _t = new GameObject("point").transform;
        _t.position = pos;
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
}