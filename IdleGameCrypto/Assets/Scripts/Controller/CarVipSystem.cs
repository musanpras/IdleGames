using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVipSystem : MonoBehaviour
{
    public float ElapsedTimeSinceLastSpawn;

    public float NextSpawnDelay;

    public bool _isVipCarLeft;

    public GameObject _carVipObj;

    public GameObject _currentVipCar;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Tick(float deltaTime)
    {
        if(_isVipCarLeft)
        {
            ElapsedTimeSinceLastSpawn += deltaTime;

            if (ElapsedTimeSinceLastSpawn >= NextSpawnDelay)
            {
                ElapsedTimeSinceLastSpawn = 0.0f;
                CreateVipCar();
            }

        }
       
    }

    public void Init()
    {
        ElapsedTimeSinceLastSpawn = 0.0f;
        _isVipCarLeft = true;
    }

    private void CreateVipCar()
    {
        CarVipMovement _vipCar = GameObject.Instantiate(_carVipObj).GetComponent<CarVipMovement>();
        _vipCar.transform.position = GamePlaySytem.instance.marketSystem._view._parkingVisualModel.CarSpawnPoint.position;
        _currentVipCar = _vipCar.gameObject;
        _isVipCarLeft = false;

    }

    public void ResetVipCarSystem()
    {
        Destroy(_currentVipCar);
        _currentVipCar = null;
        _isVipCarLeft = true;
        ElapsedTimeSinceLastSpawn = 0.0f;
    }

    public void LeaveCarParking()
    {
        if(_currentVipCar != null)
        {
            CarVipMovement _car = _currentVipCar.GetComponent<CarVipMovement>();
            _car.LeaveToParking();
        }

    }

    public void ReleaseVipCar()
    {
        Destroy(_currentVipCar);
        _currentVipCar = null;
        _isVipCarLeft = true;
        ElapsedTimeSinceLastSpawn = 0.0f;
    }
}
