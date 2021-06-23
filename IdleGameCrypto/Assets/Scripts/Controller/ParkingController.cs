using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingController : GeneratorControllerBase
{

    private ParkingView _parkingView;

    public Vector3 CarSpawnPoint => MarketSystem._instance._view._parkingVisualModel.CarSpawnPoint.position;

}
