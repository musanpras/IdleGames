using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    protected  enum eStatus
    {
        NONE,
        GOING_TO_PARKING,
        WAITING_TO_RETURN_PEOPLE,
        EXITTING
    }


    public struct VisualSetup
    {

        public Mesh Body;

       // public Mesh Weels;

    }

    private VisualSetup[] _visuals;

    protected eStatus Status;

    private bool hasParking;

    public CarMovementView _car;

    protected bool goingToParking;

    public VisualSetup[] Cars;

    public Mesh[] bodyCarList;

    //public Mesh[] wheelCarList;

    public MeshFilter _bodyMeshFilter;

  //  public MeshFilter _wheelsMeshFilter;


    public static CarController _instance;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        
    }


    public void StartParking()
    {
        Status = eStatus.GOING_TO_PARKING;
       
    }


    private void GoToParkingSlot()
    {

    }

    public virtual void SpawnPerson()
    {

    }

    public void SetVisuals(VisualSetup carVisuals)
    {
        _bodyMeshFilter.mesh = carVisuals.Body;
       // _wheelsMeshFilter.mesh = carVisuals.Weels;
    }

    public void SpawnCar()
    {
        _car.gameObject.SetActive(true);
        _car.transform.position = GamePlaySytem.instance.marketSystem._view._parkingVisualModel.CarSpawnPoint.position;

    }

    public virtual void Initialize()
    {
        goingToParking = false;
        //this._car.gameObject.SetActive(false);
        _car.transform.position = GamePlaySytem.instance.marketSystem._view._parkingVisualModel.CarSpawnPoint.position;

    }

    public void RandomizeVisuals()
     {
       int num = UnityEngine.Random.Range(0, Cars.Length);
       SetVisuals(Cars[num]);
     }
    private void LoadCarMesh()
    {
        Cars = new VisualSetup[bodyCarList.Length];
        for (int i = 0; i < bodyCarList.Length; i++)
        {
            Cars[i].Body = bodyCarList[i];
            //Cars[i].Weels = wheelCarList[i];
        }
    }



    public void CreateNormalCarView()
    {
        LoadCarMesh();
        RandomizeVisuals();
        SpawnCar();
    }
}
