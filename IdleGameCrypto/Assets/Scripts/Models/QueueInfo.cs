using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueInfo : MonoBehaviour
{
    public bool[] _queueInfo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(MarketSystem._instance._view._parkingVisualModel != null)
        {
            for(int i = 0; i < _queueInfo.Length; i++)
            {
                //if (ParkingVisualModel._instance.QueueSlots[i].IsUsed)
                    _queueInfo[i] = MarketSystem._instance._view._parkingVisualModel.QueueSlots[i].IsUsed;
            }
        }
    }
}
