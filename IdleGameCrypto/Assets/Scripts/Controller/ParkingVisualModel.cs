using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ParkingVisualModel : MonoBehaviour
{
    public string id;

    public int level;

    public int researchLevel;

    public Transform CarSpawnPoint;

    public Transform[] EntryPath;

    public Transform CarDespawnPoint;

    public Transform[] ExitPath;

    public Transform BusPoint;

    public Transform OnFootEntryPathContainer;

    public Transform[] OnFootExitPathContainers;

    public Transform VipPoint;

    public Transform VipClientPoint;

    public GeneratorStaticModel _dataModel;

    public int MaxResearchLevel => _dataModel.salaryPrice.Count;
    // public double NextLevelResearchCost => _dataModel.salaryPrice[researchLevel];
    public bool CanLevelUpResearch => researchLevel < MaxResearchLevel;
    public float ResearchLevelsProgress => (float)researchLevel / (float)MaxResearchLevel;
    public string ResearchNameKey => id.ToUpper() + "_RESEARCH";
    public string ResearchDescriptionKey => id.ToUpper() + "_RESEARCH_DESCRIPTION";
    public string ResearchIconName => ResearchNameKey.ToLower();
    public string ResearchBonusIconName => ResearchNameKey.ToLower() + "_bonus";
    public string ResearchMissingBuildingKey => ResearchNameKey + "_MISSING_BUILDING";

    public TextMeshPro UsageDisplayer;

    //public static ParkingVisualModel _instance;

    private MarketSystem _marketSystem;

    public float _timeToProduce => (float)_dataModel.start_speed / (1f + CurrentResearchLevelBonus);



    private bool _isSubscribedToEvents;

   
    public string IconName => "icon_parking";

    public int AvailableSlotsCount => GetAvailableSlots();

    public int TotalSlotsCount => GetTotalParkingSlots();
    public bool HasAvailableSlot => AvailableSlotsCount > 0;



    public float CurrentResearchLevelBonus
    {
        get
        {
            if (researchLevel <= 0)
                return 0f;
            return _dataModel.salaryBonus[0] * researchLevel;
        }
    }


    public double NextLevelResearchCost()
    {
        return _dataModel.salaryPrice[researchLevel];
    }

    //public ParkingUsageDisplayer UsageDisplayer => _parkingView.UsageDisplayer;
    public float OccupancyRate
    {
        get
        {
            float num = (float)GetAvailableSlots()* (1f / (float)TotalSlotsCount);
            return 1f - num;
        }
    }
   // public bool HasCarsInQueue => AvailableQueueSlotsCount < QueueSlotsCount;
  //  public bool HasAvailableQueueSlot => GetFirstAvailableQueueSlotIndex() > -1;
    public void Init(GeneratorModel model)
    {
       
    }


    public ParkingLane[] Lanes
    {
        get;
        private set;
    }

    public ParkingQueueSlot[] QueueSlots
    {
        get;
        private set;
    }

    public ParkingUndergroundLane UndergroundParking
    {
        get;
        private set;
    }

    private void Awake()
    {
        //_instance = this;
        //Initialize();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    

    public void Init(GeneratorSaveModel _saveModel, GeneratorStaticModel _data)
    {
        _dataModel = _data;
        level = _saveModel.level;
        id = _data.id;

        researchLevel = _saveModel.researchLevel;

        Lanes = GetComponentsInChildren<ParkingLane>();
        QueueSlots = GetComponentsInChildren<ParkingQueueSlot>();
        UndergroundParking = GetComponentInChildren<ParkingUndergroundLane>();

        if(UndergroundParking != null)
        {
            Debug.Log("Parking Underground parking found");
            
        }

        else
        {
            Debug.Log("Parking Underground parking not found");
        }

        for (int i = 0; i < QueueSlots.Length; i++)
            QueueSlots[i].Index = i;
         UsageDisplayRefresh();
    }

    public bool QueueFull()
    {
      
        if (MarketSystem._instance._view._parkingVisualModel.QueueSlots[MarketSystem._instance._view._parkingVisualModel.QueueSlots.Length - 1].IsUsed)
            return true;
        bool _check = true;
        for (int i = 0; i < QueueSlots.Length; i++)
            if (!QueueSlots[i].IsUsed)
            {
                _check = false;
                break;
            }
               
        return _check;
    }

    public int FindQueueSlotIndex()
    {
       
        for(int i = 0; i < MarketSystem._instance._view._parkingVisualModel.QueueSlots.Length; i ++ )
        {
            if (!MarketSystem._instance._view._parkingVisualModel.QueueSlots[i].IsUsed)
                return i;
        }
        return -1;
       
    }

    public int GetParkingVisualLevel()
    {
        return level;
    }

    public int GetTotalParkingSlots()
    {
        int count = 0;
        for(int i = 0; i < Lanes.Length; i++)
        {
            count += Lanes[i].SlotsCount;
        }

        return count;
    }

    public int GetAvailableSlots()
    {
        int count = 0;
        for (int i = 0; i < Lanes.Length; i++)
        {
            count += Lanes[i].AvailableSlotsCount;
        }

        return count;
    }

    public double GetCostForUpgrade()
    {
        return _dataModel.employeePrice[level];
    }

    public void UsageDisplayRefresh()
    {
        UsageDisplayer.text = (GamePlaySytem.instance.marketSystem._view.GetTotalCarParked()).ToString();
    }
}
