using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkingUndergroundLane : MonoBehaviour
{
    public int Capacity;

    public Transform[] EntryPath;

    public Transform[] ExitPath;

    public Transform[] Walkway;

    private bool[] _isCarParked;

    private bool[] _isReserved;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
