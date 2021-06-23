using System.Collections.Generic;
using UnityEngine;

    public class ParkingLane : MonoBehaviour
    {
        
        public Transform InPoint;
        public Transform OutPoint;
        public ParkingLaneWalkway _walkway;
        private ParkingSlot[] _slots;
        private List<int> _availableIndexes;

    private void Awake()
    {
        Initialize(0);
    }
    public bool HasSlotAvailable
        {
            get
            {
                ParkingSlot[] slots = _slots;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (!slots[i].IsUsed)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    public int GetSlotAvailableIndex
    {
        get
        {
            ParkingSlot[] slots = _slots;
            for (int i = 0; i < slots.Length; i++)
            {
                if (!slots[i].IsUsed)
                {
                    return i;
                }
            }
            return -1;
        }
    }
    public int SlotsCount
        {
            get
            {
                if (_slots == null)
                {
                    return 0;
                }
                return _slots.Length;
            }
        }
        public int AvailableSlotsCount
        {
            get
            {
                int num = 0;
                ParkingSlot[] slots = _slots;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (!slots[i].IsUsed)
                    {
                        num++;
                    }
                }
                return num;
            }
        }
        public int VisuallyAvailableSlotsCount
        {
            get
            {
                int num = 0;
                ParkingSlot[] slots = _slots;
                for (int i = 0; i < slots.Length; i++)
                {
                    if (!slots[i].IsCarParked)
                    {
                        num++;
                    }
                }
                return num;
            }
        }
        public void Initialize(int firstSlotGlobalIndex)
        {
            _slots = GetComponentsInChildren<ParkingSlot>();
            for (int i = 0; i < _slots.Length; i++)
            {
                ParkingSlot obj = _slots[i];
                obj.Initialize();
                obj.GlobalIndex = firstSlotGlobalIndex + i;
                obj.AssociateWith(this);
            }
            _availableIndexes = new List<int>(_slots.Length);
            ResetRandomGenerator();
            _walkway = GetComponentInChildren<ParkingLaneWalkway>();
            _walkway.Initialize(this);
            InPoint = base.transform.Find("IN");
            OutPoint = base.transform.Find("OUT");
        }
        public ParkingSlot GetSlot(int index)
        {
            if (index < 0)
            {
                return null;
            }
            if (index >= _slots.Length)
            {
                return null;
            }
            return _slots[index];
        }
        public int IndexOf(ParkingSlot slot)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == slot)
                {
                    return i;
                }
            }
            return -1;
        }
        public void ReserveSlot(ParkingSlot slot)
        {
            slot.IsReserved = true;
            int item = IndexOf(slot);
            _availableIndexes.Remove(item);
        }
        public void UseSlot(ParkingSlot slot)
        {
            slot.IsCarParked = true;
        }
        public void  UseSlotByIndex(int index)
        {

        ParkingSlot slot = GetSlot(index);
        slot.IsCarParked = true;

        }
        public void FreeSlot(ParkingSlot slot)
        {
            slot.IsReserved = false;
            slot.IsCarParked = false;
        }
        public bool HasSlot(ParkingSlot slot)
        {
            ParkingSlot[] slots = _slots;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == slot)
                {
                    return true;
                }
            }
            return false;
        }
        public ParkingSlot GetRandomAvailableSlot()
        {
            if (_availableIndexes.Count < 1)
            {
                ResetRandomGenerator();
            }
            if (_availableIndexes.Count < 1)
            {
                return null;
            }
            int index = UnityEngine.Random.Range(0, _availableIndexes.Count);
            ParkingSlot slot = GetSlot(_availableIndexes[index]);
            _availableIndexes.RemoveAt(index);
            return slot;
        }
        public void ResetRandomGenerator()
        {
            _availableIndexes.Clear();
            for (int i = 0; i < _slots.Length; i++)
            {
                if (!_slots[i].IsUsed)
                {
                    _availableIndexes.Add(i);
                }
            }
        }
        public Vector3[] CreatePathFromSlotToMarket(int slotIndex)
        {
            return _walkway.CreatePathToWalkFromSlotToMarket(slotIndex);
        }
        public Vector3[] CreatePathFromMarketToSlot(int slotIndex)
        {
            return _walkway.CreatePathToWalkFromMarketToSlot(slotIndex);
        }
    }
