using System.Collections.Generic;
using UnityEngine;

    public class ParkingLaneWalkway : MonoBehaviour
    {
        public Transform[] _walkwayPath;
        protected ParkingLane Lane
        {
            get;
            private set;
        }
        public void Initialize(ParkingLane lane)
        {
            Lane = lane;
            int childCount = base.transform.childCount;
            _walkwayPath = new Transform[childCount];
            for (int i = 0; i < childCount; i++)
            {
                Transform child = base.transform.GetChild(i);
                _walkwayPath[i] = child;
            }
        }
        public Vector3 GetSlotWalkPoint(int slotIndex)
        {
            return Lane.GetSlot(slotIndex).WalkPoint.position;
        }
        public Vector3[] CreatePathToWalkFromMarketToSlot(int slotIndex)
        {
            List<Vector3> list = new List<Vector3>();
            for (int i = 0; i < _walkwayPath.Length; i++)
            {
                list.Add(_walkwayPath[i].position);
            }
            for (int j = 0; j <= slotIndex; j++)
            {
                Vector3 slotWalkPoint = GetSlotWalkPoint(j);
                list.Add(slotWalkPoint);
            }
            return list.ToArray();
        }
        public Vector3[] CreatePathToWalkFromSlotToMarket(int slotIndex)
        {
            List<Vector3> list = new List<Vector3>();
            for (int num = slotIndex; num >= 0; num--)
            {
                Vector3 slotWalkPoint = GetSlotWalkPoint(num);
                list.Add(slotWalkPoint);
            }
            for (int num2 = _walkwayPath.Length - 1; num2 >= 0; num2--)
            {
                list.Add(_walkwayPath[num2].position);
            }
            return list.ToArray();
        }

        public Transform[] GetWalkway()
       {
         return _walkwayPath;
       }
    }






