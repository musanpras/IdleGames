using SWS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CarVipMovement : MonoBehaviour
{
    public splineMove _moveController;

    public PathManager _pathManager;

    public List<Transform> staticPath = new List<Transform>();

    public CarVipController _carController;

    // Start is called before the first frame update
    void Start()
    {
        _carController.HideGlow();
        LoadPathToParking();
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void LoadPathToParking()
    {
        _carController._Status = CarVipController.eStatus.GOING_TO_PARKING;
        ClearPath();
        staticPath.Add(transform);
        staticPath.Add(CreatTransfromFrom(
            new Vector3(MarketSystem._instance._view._parkingVisualModel.CarSpawnPoint.position.x,
             MarketSystem._instance._view._parkingVisualModel.CarSpawnPoint.position.y,
              MarketSystem._instance._view._parkingVisualModel.CarSpawnPoint.position.z + 2.5f)));
        staticPath.Add(CreatTransfromFrom(
           new Vector3(MarketSystem._instance._view._parkingVisualModel.EntryPath[0].position.x,
            MarketSystem._instance._view._parkingVisualModel.EntryPath[0].position.y,
             MarketSystem._instance._view._parkingVisualModel.EntryPath[0].position.z + 2.5f)));

        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.EntryPath[1].position));

        staticPath.Add(CreatTransfromFrom(
            new Vector3(MarketSystem._instance._view._parkingVisualModel.VipPoint.position.x - 3.8f,
             MarketSystem._instance._view._parkingVisualModel.VipPoint.position.y,
              MarketSystem._instance._view._parkingVisualModel.VipPoint.position.z - 2.2f)
           ));

        staticPath.Add(CreatTransfromFrom(
           new Vector3(MarketSystem._instance._view._parkingVisualModel.VipPoint.position.x - 3.6f,
            MarketSystem._instance._view._parkingVisualModel.VipPoint.position.y,
             MarketSystem._instance._view._parkingVisualModel.VipPoint.position.z - 2.2f)
          ));

        // staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.VipPoint.position));

        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);

        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(Park);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);
    }

    public void Park()
    {
        _carController._Status = CarVipController.eStatus.PARKING;
        _carController.ShowGlow();
    }

    public void LeaveToParking()
    {
        _carController._Status = CarVipController.eStatus.LEAVE_TO_PARKING;
        ClearPath();
        staticPath.Add(transform);
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.VipPoint.position));
        staticPath.Add(CreatTransfromFrom(
            new Vector3(MarketSystem._instance._view._parkingVisualModel.VipPoint.position.x + 2.0f,
            MarketSystem._instance._view._parkingVisualModel.VipPoint.position.y,
            MarketSystem._instance._view._parkingVisualModel.VipPoint.position.z)));
        staticPath.Add(CreatTransfromFrom(
           new Vector3(MarketSystem._instance._view._parkingVisualModel.VipPoint.position.x + 2.0f,
           MarketSystem._instance._view._parkingVisualModel.VipPoint.position.y,
           MarketSystem._instance._view._parkingVisualModel.ExitPath[0].position.z)));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.ExitPath[0].position));
        staticPath.Add(CreatTransfromFrom(MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position));
        staticPath.Add(CreatTransfromFrom(
          new Vector3(MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.x + 100, MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.y,
          MarketSystem._instance._view._parkingVisualModel.ExitPath[1].position.z)
          ));
        _pathManager.Create(staticPath.ToArray());
        _moveController.SetPath(_pathManager);

        UnityEvent m_MyEvent = new UnityEvent();
        m_MyEvent.AddListener(RemoveCar);
        _moveController.AddEvent(_pathManager.GetPathPoints().Length - 1, m_MyEvent);

    }

    public void RemoveCar()
    {
        GamePlaySytem.instance.carVipSystem.ReleaseVipCar();
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
