using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GeneratorModel : MonoBehaviour
{
    [HideInInspector]
    public string id;

    [HideInInspector]
    public int level;

    [HideInInspector]
    public int researchLevel;

    [HideInInspector]
    public List<PeopleQueue> _queueList = new List<PeopleQueue>();

    [HideInInspector]
    public int MaxLevel, MaxQueue;

    [HideInInspector]
    public bool CanLevelUp => level < MaxLevel;

    [HideInInspector]
    public int ProductIndex;

    [HideInInspector]
    public Transform[] employees;

    [HideInInspector]
    public List<EmployeeController> _employeeList = new List<EmployeeController>();

   //[HideInInspector]
    public Transform[] pointCustomer;

    [HideInInspector]
    [SerializeField]
    public Transform manager;

    [HideInInspector]
    [SerializeField]
    public TextMeshPro indexLabel;

    [HideInInspector]
    [SerializeField]
    public ParticleSystem boostParticle;

    [HideInInspector]
    public int TotalEmployees;

    [HideInInspector]
    public int EmployeesBought;

   // [HideInInspector]
    public int EmployeesGifted;

   // [HideInInspector]
    public Vector3 offetQueuePos;

    [HideInInspector]
    public GeneratorStaticModel _dataModel;

    public string NameKey => id.ToUpper();

    public string DescriptionKey => NameKey + "_DESCRIPTION";
    public string MissingPreviousShopKey => NameKey + "_MISSING_PREVIOUS_SHOP";
    public string IconName => "icon_shop_" + NameKey.ToLower();
    public string ProductNameKey => $"{id.ToUpper()}_NAME_{Mathf.Clamp(ProductIndex, 0, MaxNormalLevel)}";
    public string ProductDescriptionKey => $"{id.ToUpper()}_DESCRIPTION_{Mathf.Clamp(ProductIndex, 0, MaxNormalLevel)}";
    public string ProductIconName => $"{id.ToLower()}_{Mathf.Clamp(ProductIndex, 0, MaxNormalLevel)}";

    public int MaxResearchLevel => _dataModel.researchMaxLevel;
 
    public bool CanLevelUpResearch => researchLevel < MaxResearchLevel;
    public float ResearchLevelsProgress => (float)researchLevel / (float)MaxResearchLevel;
    public string ResearchNameKey => id.ToUpper() + "_RESEARCH";
     public string ResearchDescriptionKey => id.ToUpper() + "_RESEARCH_DESCRIPTION";
     public string ResearchIconName => ResearchNameKey.ToLower();
    public string ResearchBonusIconName => ResearchNameKey.ToLower() + "_bonus";
    public string ResearchMissingBuildingKey => ResearchNameKey + "_MISSING_BUILDING";


    private int _maxNormalLevel = -1;


    public int ProductsCount => _dataModel.bonusAtLevel.Count;

    public int MaxBuyableEmployees => _dataModel.maxEmployees - 1;
    public int MaxEmployees => MaxBuyableEmployees + NotBoughtEmployees;

    public int NotBoughtEmployees => EmployeesGifted;
  
    public bool CanBuyEmployees => EmployeesBought < MaxBuyableEmployees;

    public double TimeToProduce => _dataModel.start_speed / (double)(1f + CurrentResearchLevelBonus);

    public float ClientsPerMinute => 1f / (float)TimeToProduce * (float)TotalEmployees * 60f;
    public double TotalProductionPerMinute => (double)ClientsPerMinute * GetCurrentProduction();

    public float ClientProcessDuration => (float)TimeToProduce;

    public float CurrentResearchLevelBonus
    {
        get
        {
            if (researchLevel <= 0)
                return 0f;
            int num = Mathf.Clamp(researchLevel, 1, _dataModel.salaryBonus.Count);
            return _dataModel.salaryBonus[num - 1];
        }
    }

    public int MaxNormalLevel
    {
        get
        {
            if (_maxNormalLevel == -1)
            {
                for (int i = 0; i < ProductsCount; i++)
                {
                    if (_dataModel.bonusStellar <= _dataModel.bonusAtLevel[i])
                    {
                        _maxNormalLevel = i;
                        return _maxNormalLevel;
                    }
                }
                return ProductsCount;
            }
            return _maxNormalLevel;
        }
    }
    public bool IsStellar => level >= _dataModel.bonusStellar;
    public int StellarLevel
    {
        get
        {
            int num = 0;
            if (level >= _dataModel.bonusStellar)
            {
                for (int i = 0; i < ProductsCount; i++)
                {
                    if (level >= _dataModel.bonusAtLevel[i] && _dataModel.bonusAtLevel[i] > _dataModel.bonusStellar)
                    {
                        num++;
                    }
                }
            }
            return num;
        }
    }


    public int LevelForNextProduct
    {
        get
        {
            int num = ProductIndex + 1;
            if (num >= _dataModel.bonusAtLevel.Count)
            {
                return -1;
            }
            return _dataModel.bonusAtLevel[num];
        }
    }
    public float ProgressToNextProduct
    {
        get
        {
            int productIndex = ProductIndex;
            int num = productIndex + 1;
            if (num >= _dataModel.bonusAtLevel.Count)
            {
                return 1f;
            }
            int num2 = _dataModel.bonusAtLevel[productIndex];
            int num3 = _dataModel.bonusAtLevel[num];
            return (float)(level - num2) / (float)(num3 - num2);
        }
    }
    public float LevelProgress
    {
        get
        {
            int researchMaxLevel = _dataModel.researchMaxLevel;
            return (float)level / (float)MaxLevel;
        }
    }
    public double BaseValue => _dataModel.start_sell_price;





    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Init(GeneratorSaveModel _saveModel,  GeneratorStaticModel _data)
    {
        LoadAllTransform();
        EmployeesBought = _saveModel.employeesBought;
        level = _saveModel.level;
        researchLevel = _saveModel.researchLevel;
        _dataModel = _data;
        id = _data.id;
        MaxQueue = _data.max_queue;
        MaxLevel = _data.max_level;
        ProductIndex = _saveModel.productIndex;
        TotalEmployees = EmployeesGifted + EmployeesBought;
        InitEmployees();
        InitManager();
    }

    public void Init(GeneratorStaticModel _data)
    {
        level = 0;
        _dataModel = _data;
        id = _data.id;
        MaxQueue = _data.max_queue;
        MaxLevel = _data.max_level;
    }

    public Vector3 GetCustomerPos()
    {
        int _queueIndex = GetQueueIndexAvailable();
        if (_queueIndex < 0)
            return Vector3.zero;
        int _peopleInQueue = _queueList[_queueIndex].peopleInQueue.Count;
        Vector3 _pos = offetQueuePos * _peopleInQueue + pointCustomer[_queueIndex].position;
        return _pos;
    }

    public void SetPeoplePos(PeopleController people, int queueIndex)
    {
        int _peopleInQueue = _queueList[queueIndex].peopleInQueue.Count;
        int orderFromQueue = _queueList[queueIndex].peopleInQueue.IndexOf(people);
        Vector3 _pos = offetQueuePos * orderFromQueue + pointCustomer[queueIndex].position;
        people.transform.position = _pos;
    }

    public int GetOrderInQueue(Vector3 pos, int queueIndex)
    {
        return Mathf.RoundToInt((pos - pointCustomer[queueIndex].position).z / offetQueuePos.z); 
    }    

    public int OffsetPositionForCustomer(PeopleController people, int queueIndex)
    {
        int offset = 0;
        int orderFromPos = GetOrderInQueue(people.gameObject.transform.position, queueIndex);
        int orderFromQueue = _queueList[queueIndex].peopleInQueue.IndexOf(people);
        if(orderFromQueue < orderFromPos)
        {
            offset = orderFromPos - orderFromQueue;
        }
        return offset;
    }

    public Vector3 GetEmloyeePosition(int employeeIndex)
    {
        Transform transform = _employeeList[employeeIndex].gameObject.transform;
        Transform child = transform.GetChild(0);
        if(child != null)
        {
            return child.position;
        }
        return transform.position;
    }  

    public PeopleController GetFirstCustomer(int queueIndex)
    {
        return _queueList[queueIndex].peopleInQueue[0];
    }   
    
    public IEnumerator SortAllPeopleInQueueIE()
    {
        for(int i = 0; i < _queueList.Count; i++)
        {
            for(int j = 0; j < _queueList[i].peopleInQueue.Count; j ++)
            {
                if (_queueList[i].peopleInQueue[j].status == PeopleController.eStatus.WAY_TO_SHOP_QUEUE ||
                    _queueList[i].peopleInQueue[j].status == PeopleController.eStatus.WAITING_IN_QUEUE_SHOP ||
                    _queueList[i].peopleInQueue[j].status == PeopleController.eStatus.EDITTING_POS_IN_QUEUE)
                {
                    _queueList[i].peopleInQueue[j].peopleView.StandInShopQueue();
                    yield return new WaitForSeconds(0.03f);
                }
            }
        }    
    }    

    public void SortAllPeopleInQueue()
    {
        StartCoroutine(SortAllPeopleInQueueIE());
    }

    public int GetPeopleCountInQueue(int index)
    {
        return _queueList[index].peopleInQueue.Count;
    }

    public void SetPeopleOnEmployee(PeopleController people)
    {
        _employeeList[GetQueueIndexAvailable()].SupportCustomer(people);
        AddPeopleToQueue(people, GetQueueIndexAvailable());
    }

    public Vector3 GetEmployeePos(int employeeIndex)
    {
        Transform _trans = employees[employeeIndex];
        return _trans.position;

    }

    public EmployeeController GetEmployeeByQueueIndex(int queue)
    {
        return _employeeList[queue];

    }

    public void ReleasePeopleInQueue(int queueIndex)
    {
        PeopleController endPeople = _queueList[queueIndex].peopleInQueue[0];
        _queueList[queueIndex].peopleInQueue.RemoveAt(0);
       // Debug.Log("TOTAL SHOP " + MarketSystem._instance._view.GetShopBuildCount());
        if (endPeople.peopleView.shopStep < MarketSystem._instance._view.GetShopBuildCount() -1)
        {
            endPeople.peopleView.shopStep++;
            endPeople.peopleView.SearchPathToQueueShop();
        }
           
        else
          endPeople.peopleView.SearchPathToQueueCheckout();
        
        for (int i = 0; i < _queueList[queueIndex].peopleInQueue.Count; i++)
        {
            if (_queueList[queueIndex].peopleInQueue[i].peopleView.peopleController.status == PeopleController.eStatus.WAITING_IN_QUEUE_SHOP)
            _queueList[queueIndex].peopleInQueue[i].peopleView.GoToNextInQueue();
        }

           
    }

    private void InitEmployees()
    {
        for(int i =0; i < employees.Length; i++)
        {
            if (i < TotalEmployees)
            {
                employees[i].gameObject.SetActive(true);
                EmployeeController employeeController = employees[i].GetComponent<EmployeeController>();
                //employeeController.Index = i;
                PeopleQueue _peopleList = new PeopleQueue();
                _queueList.Add(_peopleList);
                employeeController.Init(i);
                employeeController.timeToProducting = GetTimeToProduce();
                _employeeList.Add(employeeController);
            }
               
            else
                employees[i].gameObject.SetActive(false);
        }
    }

    public void NewEmployee()
    {
        TotalEmployees = EmployeesBought + EmployeesGifted;
        int _newIndex = TotalEmployees -1;
        employees[_newIndex].gameObject.SetActive(true);
        EmployeeController employeeController = employees[_newIndex].GetComponent<EmployeeController>();
        PeopleQueue _peopleList = new PeopleQueue();
        _queueList.Add(_peopleList);
        employeeController.Init(_newIndex);
        employeeController.timeToProducting = GetTimeToProduce();
        _employeeList.Add(employeeController);
      
    }

    private void InitManager()
    {
        if(manager != null)
        {
            manager.gameObject.SetActive(value: false);
        }
    }

    public void RefreshPeopleInQueue(List<PeopleQueue> queueListPara)
    {
       // _queueList.Clear();
        for (int i = 0; i < queueListPara.Count; i ++)
        {
            _queueList[i] = queueListPara[i];
            for (int j = 0; j < queueListPara[i].peopleInQueue.Count; j++)
                _queueList[i].peopleInQueue[j] = queueListPara[i].peopleInQueue[j];
        }
    }

    public void RefreshEmployees(List<EmployeeController> employeeListPara)
    {
        for (int i = 0; i < _employeeList.Count; i++)
        {
            _employeeList[i].Reset(employeeListPara[i]);
        }
    }


    public void AddPeopleToQueue(PeopleController _people, int _queueIndex)
    {
        if(_queueList[_queueIndex].peopleInQueue.Count <= MaxQueue)
        {
            _queueList[_queueIndex].peopleInQueue.Add(_people);
           // Debug.Log("ADD PEOPLE " + _queueList[_queueIndex].peopleInQueue.Count);
        }
    }

    public void AddPeople(PeopleController _people)
    {
        AddPeopleToQueue(_people, GetQueueIndexAvailable());
    }    

    public int GetQueueIndexAvailable()
    {
        int _index = 0;

        int _totalPeopleInQueue = 0, _temp = _queueList[0].peopleInQueue.Count;
        //Debug.Log("PEOPLE " + _queueList.Count);
        for (int i = 1; i < _queueList.Count; i++)
        {
            //Debug.Log("PEOPLE " + i + " " + _queueList[i].peopleInQueue.Count);
            if (_queueList[i].peopleInQueue.Count < MaxQueue)
            {
                _totalPeopleInQueue = _queueList[i].peopleInQueue.Count;
                if (_temp > _totalPeopleInQueue)
                {
                    _temp = _totalPeopleInQueue;
                    _index = i;
                }
            }
            else
                _index = -1;
               
        }
        //Debug.Log("FIND INDEX " + _index);
        return _index;
    }

    public bool CheckSlotToShop()
    {
        for (int i = 0; i < _queueList.Count; i++)
        {
            if (_queueList[i].peopleInQueue.Count <= MaxQueue)

                return true;

        }
        return false;
    }
    public void StartProduction(EmployeeController employee)
    {
        UISystem.instance._employeesProgressSystem.Show(employee);
    }

    public void UpdateproductionStatus(EmployeeController employee, float progress)
    {
        UISystem.instance._employeesProgressSystem.UpdateProgress(employee, progress);
    }

    public void EndProduction(EmployeeController employee)
    {
        UISystem.instance._employeesProgressSystem.Hide(employee);
    }

    public void HideAllProgress()
    {
        for(int i = 0; i < _employeeList.Count; i ++)
            UISystem.instance._employeesProgressSystem.HideEmployeee(_employeeList[i]);
    }

    public double GetCurrentProductionForLevel(int level)
    {
        if (level == 0)
            return 0.0;
        return _dataModel.start_sell_price * (1.0 + _dataModel.grow_production * (double)(level - 1));
    }

    public double GetCurrentProduction()
    {
        return GetCurrentProductionForLevel(level) * GetMultiplierForProduct(FindProductIndexForLevel(level));
    }

    private int FindProductIndexForLevel(int level)
    {
        int result = 0;
        for(int i = 0; i < _dataModel.bonusAtLevel.Count; i ++)
        {
            if (level >= _dataModel.bonusAtLevel[i])
                result = i;
        }
        return result;
    }
    public int FindIndexForLevel(int level)
    {
        return FindProductIndexForLevel(level);
    }
    public float GetMultiplierForProduct(int productIndex)
    {
        productIndex = Mathf.Clamp(productIndex, 0, _dataModel.bonusParameter.Count - 1);
        return _dataModel.bonusParameter[productIndex];
    }

    public double GetLevelUpPrice(int level)
    {
        if (level == 0)
            return _dataModel.build_cost;
        double num = 1.0f;
        return _dataModel.build_cost * Math.Pow(1.0 + _dataModel.grow_cost, level - 1) * num;

    }

    public double GetCostNextEmployee()
    {
        int boughtEmployee = EmployeesBought;

        if (boughtEmployee >= _dataModel.employeePrice.Count)
            return double.MaxValue;
        return _dataModel.employeePrice[boughtEmployee];
    }

    public double GetTimeToProduce()
    {
        return _dataModel.start_speed / (double)(1f + GetCurrenResearchLevelBonus());
    }

    public float GetCurrenResearchLevelBonus()
    {
        if (researchLevel <= 0)
            return 0f;
        int num = Mathf.Clamp(researchLevel, 1, _dataModel.salaryBonus.Count);
        return _dataModel.salaryBonus[num - 1];
    }

    public double NextLevelResearchCost()
    {
        return _dataModel.salaryPrice[researchLevel];
    }

    public double GetLevelUpPrice()
    {
        return GetLevelUpPrice(level);
    }

    public bool BuildOrLevelUp(bool changeVisuals = true)
    {
        int productIndex = FindIndexForLevel(level);

        ProductIndex = productIndex;

        return false;
    }

    private void RefreshEmployees()
    {
        if (employees == null || employees.Length == 0)
            return;

        for(int i = 0; i < employees.Length; i ++)
        {

            if (i < TotalEmployees)
                employees[i].gameObject.SetActive(true);
            else
                employees[i].gameObject.SetActive(false);
        }
        InitEmployees();
    }

    public bool RefreshVisual(bool changeVisuals = true)
    {
        if (level == 0)
            return false;

        bool flag = false;

        RefreshEmployees();
        return flag;

    }

    public void LoadAllTransform()
    {
        employees = new Transform[5];
        for(int i = 1; i <= 5; i++)
        {
            employees[i - 1] = transform.Find("employee_" + i.ToString());
        }
        pointCustomer = new Transform[5];
        for (int i = 1; i <= 5; i++)
        {
            pointCustomer[i-1] = transform.Find("Customer_" + i.ToString());
        }

        manager = transform.Find("manager_pos");
        boostParticle = transform.Find("FX_BoosterCashier").GetComponent<ParticleSystem>();
    }

    /*
    protected void OnDrawGizmos()
    {
        Vector3 _queueSlot = GetCustomerPos();
       // Debug.Log("frist customer slot " + _queueSlot);
        Gizmos.color = Color.red;
        float num = 0.35f;
        float num2 = 0.35f;
        Vector3 vector = _queueSlot + new Vector3(num, 0f, num2);
        Vector3 vector2 = _queueSlot + new Vector3(num, 0f, 0f - num2);
        Vector3 vector3 = _queueSlot + new Vector3(0f - num, 0f, 0f - num2);
        Vector3 vector4 = _queueSlot + new Vector3(0f - num, 0f, num2);
        Gizmos.DrawLine(vector, vector2);
        Gizmos.DrawLine(vector2, vector3);
        Gizmos.DrawLine(vector3, vector4);
        Gizmos.DrawLine(vector4, vector);      
        //Gizmos.DrawLine(base.transform.position, Car.transform.position);
    }
    */
}
