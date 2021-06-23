using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pathfinding;
using System.Linq;
using System.Text.RegularExpressions;
using System;

public class MarketView : MonoBehaviour
{

    public GameObject _parkingModel, _marketModel,_fruitModel,_checkOutModel,_fruitModel_lv2, _marketModel_lv2,_bakeryModel;

    public GameObject[] _parkingModelList;

    public string[] _shopNameList;

    public Transform parkingPosition, marketPosition;

    public MarketVisualModel _currentViewModel;

    public ParkingVisualModel _parkingVisualModel;

    public Dictionary<string, GeneratorModel> generatorControllers = new Dictionary<string, GeneratorModel>();

    public CheckoutSystem checkOutSystem;

    public ABPath _pathFinding;

    public int evolution;

    public double money;

    public int gems;

    public double moneyPending;

    public SaveModel _saveModel;

    public int finishVideoPowerUpTimer;

    public int timestamp;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Init()
    {
        _saveModel = GamePlaySytem.instance._saveSystem.LoadMarketView();
        money = _saveModel.money;
        gems = _saveModel.gems;
        timestamp = _saveModel.timestamp;
        finishVideoPowerUpTimer = _saveModel.finishVideoPowerUpTimestamp;
        //evolution = _saveModel.evolution;

        BuildDecoration();
        BuildVisuals();
        BuildMarketVisual();
  
    }    

    public void BuildDecoration()
    {

        SceneManager.LoadScene("MarketView", LoadSceneMode.Additive);
        RenderSettings.ambientLight = new Color(0.85f, 0.85f, 0.85f);
        GameObject gameObject1 = GameObject.Find("cameraBound1");
        GameObject gameObject2 = GameObject.Find("cameraBound2");

    }

    public void BuildVisuals()
    {
        GeneratorSaveModel _parkingSaveModel = _saveModel.zones["PARKING"];

        GameObject parkingVisual = (GameObject)Instantiate(Resources.Load("supermarket/" +
                    GamePlaySytem.instance._generatorStaticModels["PARKING"]._visualLevelPrefab + "_" + _parkingSaveModel.level));
        _parkingVisualModel = parkingVisual.GetComponent<ParkingVisualModel>();
        _parkingVisualModel.Init(_saveModel.zones["PARKING"], GamePlaySytem.instance._generatorStaticModels["PARKING"]);


        evolution = _saveModel.evolution;
        GameObject marketVisual = (GameObject)Instantiate(Resources.Load("supermarket/market/market_lvl_" + (_saveModel.evolution+1)));
        _currentViewModel = marketVisual.GetComponent<MarketVisualModel>();
        _currentViewModel.Init();
        parkingVisual.transform.parent = marketPosition.parent;
        marketVisual.transform.parent = marketPosition.parent;

        parkingVisual.transform.position = parkingPosition.position;
        parkingVisual.transform.rotation = marketPosition.rotation;

        marketVisual.transform.position = marketPosition.position;
        marketVisual.transform.rotation = marketPosition.rotation;
       

    }

    public void BuildMarketVisual()
    {
        
        for(int i = 0; i < _saveModel.zones.Count; i++)
        {
            //Debug.Log("Instance " + _saveModel.zones.ElementAt(i).Value.id);
        }

        foreach (KeyValuePair<string, GeneratorSaveModel> _generator in _saveModel.zones)
        {
            if (_generator.Value.level > 0 && _generator.Key != "PARKING")
            {
               // Debug.Log("Instance " + _generator.Key);
                GameObject _visual = (GameObject)Instantiate(Resources.Load("supermarket/" +
                    GamePlaySytem.instance._generatorStaticModels[_generator.Key]._visualLevelPrefab + "_" + (_generator.Value.productIndex + 1)));
                _visual.transform.position = GetTransformForZone(_generator.Key).position;
                _visual.transform.parent = parkingPosition.parent;
                _visual.transform.rotation = marketPosition.rotation;

                if (_generator.Key == "CHECKOUT1" || _generator.Key == "CHECKOUT2" || _generator.Key == "CHECKOUT3" || _generator.Key == "CHECKOUT4")
                {
                    CheckoutModel _model = _visual.GetComponent<CheckoutModel>();
                    _model.Init(_saveModel.zones[_generator.Key], GamePlaySytem.instance._generatorStaticModels[_generator.Key]);
                    checkOutSystem.checkOutList.Add(_model);
                }
                else
                {
                    GeneratorModel _model = _visual.GetComponent<GeneratorModel>();
                    _model.Init(_saveModel.zones[_generator.Key], GamePlaySytem.instance._generatorStaticModels[_generator.Key]);
                    generatorControllers.Add(_generator.Key, _model);
                }

            }
        }
           
            

            foreach (KeyValuePair<string, GeneratorStaticModel> _staticModel in GamePlaySytem.instance._generatorStaticModels)
            {
               
                if (!generatorControllers.ContainsKey(_staticModel.Key))
                {
                   if (_staticModel.Key != "PARKING" && _staticModel.Key != "CHECKOUT1" && _staticModel.Key != "CHECKOUT2" && _staticModel.Key != "CHECKOUT3" && _staticModel.Key != "CHECKOUT4")
                    {
                    GeneratorModel _model = new GeneratorModel();
                    _model.Init(GamePlaySytem.instance._generatorStaticModels[_staticModel.Key]);
                    generatorControllers.Add(_staticModel.Key, _model);
                    }

                }
                
                if (_staticModel.Key == "CHECKOUT1" || _staticModel.Key == "CHECKOUT2" || _staticModel.Key == "CHECKOUT3" || _staticModel.Key == "CHECKOUT4")
                {
               
                   if (_saveModel.zones[_staticModel.Key].level == 0)
                   {
                    CheckoutModel _model1 = new CheckoutModel();
                    _model1.Init(GamePlaySytem.instance._generatorStaticModels[_staticModel.Key]);
                    checkOutSystem.checkOutList.Add(_model1);
                   }
                
                }
                
            }
            
           
  
    }

    public void UpgradeShop(string _zone)
    {
        _saveModel.zones[_zone].productIndex = generatorControllers[_zone].ProductIndex;
        _saveModel.zones[_zone].level = generatorControllers[_zone].level;
        _saveModel.zones[_zone].researchLevel = generatorControllers[_zone].researchLevel;

        GameObject _visual = (GameObject)Instantiate(Resources.Load("supermarket/" +
                   GamePlaySytem.instance._generatorStaticModels[_zone]._visualLevelPrefab + "_" + (_saveModel.zones[_zone].productIndex + 1)));
        
        _visual.transform.position = GetTransformForZone(_zone).position;
        _visual.transform.parent = parkingPosition.parent;
        _visual.transform.rotation = marketPosition.rotation;
        GeneratorModel _model = _visual.GetComponent<GeneratorModel>();
        _model.Init(_saveModel.zones[_zone], GamePlaySytem.instance._generatorStaticModels[_zone]);
        GameObject lastFruit = generatorControllers[_zone].gameObject;
        generatorControllers[_zone].HideAllProgress();
     
        _model.RefreshPeopleInQueue(lastFruit.GetComponent<GeneratorModel>()._queueList);
        _model.RefreshEmployees(lastFruit.GetComponent<GeneratorModel>()._employeeList);
        
        
        Destroy(lastFruit);
        AstarPath.active.Scan();
        generatorControllers.Remove(_zone);
        generatorControllers.Add(_zone, _model);
    }
    /*
    public void BuildBakery()
    {
        //new market visual
        GameObject lastMarket = _currentViewModel.gameObject;
        GameObject marketVisual = (GameObject)Instantiate(_marketModel_lv2);
        _currentViewModel = marketVisual.GetComponent<MarketVisualModel>();
        _currentViewModel.Init();
        marketVisual.transform.parent = marketPosition.parent;
        marketVisual.transform.position = marketPosition.position;
        marketVisual.transform.rotation = marketPosition.rotation;
        generatorControllers["FRUIT"].gameObject.transform.position = GetTransformForZone("FRUIT").position;
        generatorControllers["FRUIT"].SortAllPeopleInQueue();
       
        checkOutSystem.checkOutList[0].transform.position = GetTransformForZone("CHECKOUT1").position;
        checkOutSystem.checkOutList[0].SortAllPeopleInQueue();

        GameObject bakeryVisual = (GameObject)Instantiate(_bakeryModel);
        bakeryVisual.transform.position = GetTransformForZone("BAKERY").position;
        bakeryVisual.transform.parent = parkingPosition.parent;
        bakeryVisual.transform.rotation = marketPosition.rotation;
        GeneratorModel bakeryModel = bakeryVisual.GetComponent<GeneratorModel>();
        bakeryModel.Init(_saveModel.zones["BAKERY"], GamePlaySytem.instance._generatorStaticModels["BAKERY"]);
        generatorControllers.Add("BAKERY", bakeryModel);

        Destroy(lastMarket);
        AstarPath.active.Scan();
    }
    */
    public void UpgradeCheckout(string zone)
    {
        int num = int.Parse(Regex.Match(zone, "\\d+").Value);
        GeneratorStaticModel checkoutModel = GamePlaySytem.instance._generatorStaticModels[zone];
        int level = num;
        double levelUpPriceForLevel = checkoutModel.build_cost;
        if (money >= levelUpPriceForLevel)
        {
            money -= levelUpPriceForLevel * (1 - GamePlaySytem.instance._reduceCashierCost);

            GameObject checkOutVisual = (GameObject)Instantiate(Resources.Load("supermarket/" +
                   GamePlaySytem.instance._generatorStaticModels[zone]._visualLevelPrefab + "_" + "1"));
            checkOutVisual.transform.position = GetTransformForZone(zone).position;
            checkOutVisual.transform.parent = parkingPosition.parent;
            checkOutVisual.transform.rotation = marketPosition.rotation;
            CheckoutModel checkOutModel = checkOutVisual.GetComponent<CheckoutModel>();

           // GeneratorSaveModel saveModel = new GeneratorSaveModel();
           // _saveModel.zones.Add(zone, saveModel);
            _saveModel.zones[zone].level = 1;
            checkOutModel.Init(_saveModel.zones[zone], GamePlaySytem.instance._generatorStaticModels[zone]);
            checkOutSystem.checkOutList[num - 1] = checkOutModel;
            GamePlaySytem.instance._saveSystem.Save();
           // checkOutSystem.checkOutList.Add(checkOutModel);
            AstarPath.active.Scan();

        }
      
    }

    public void UpgradeParking()
    {

        GameObject lastParkingModel = _parkingVisualModel.gameObject;
        int lastParkingLevel = _parkingVisualModel.level;
        int lastParkingResearch = _parkingVisualModel.researchLevel;
        Destroy(lastParkingModel);

        lastParkingLevel++;
        GameObject parkingVisual = (GameObject)Instantiate(Resources.Load("supermarket/" +
                   GamePlaySytem.instance._generatorStaticModels["PARKING"]._visualLevelPrefab + "_" + lastParkingLevel));
        _parkingVisualModel = parkingVisual.GetComponent<ParkingVisualModel>();
        _saveModel.zones["PARKING"].level = lastParkingLevel;
        _saveModel.zones["PARKING"].researchLevel = lastParkingResearch;
        _parkingVisualModel.Init(_saveModel.zones["PARKING"], GamePlaySytem.instance._generatorStaticModels["PARKING"]);
       

        parkingVisual.transform.parent = marketPosition.parent;
        parkingVisual.transform.position = parkingPosition.position;
        parkingVisual.transform.rotation = marketPosition.rotation;
      
        ResetParking();
        _parkingVisualModel.UsageDisplayRefresh();
    }

    void ResetParking()
    {

                 for (int i = 0; i < MarketSystem._instance._view._parkingVisualModel.QueueSlots.Length; i++)
                 {
                     if (MarketSystem._instance._view._parkingVisualModel.QueueSlots[i].IsUsed)
                     {
                         MarketSystem._instance._view._parkingVisualModel.QueueSlots[i].Free();
                     }
                 }

            List<GameObject> carTrash = new List<GameObject>();
             for (int i = 0; i < MarketSystem._instance._carSystemList.Count; i++)
             {
                 if (MarketSystem._instance._carSystemList[i]._car.currentPhase == CarMovementView.Phase.Exit
                     || MarketSystem._instance._carSystemList[i]._car.currentPhase == CarMovementView.Phase.ExitParking)
                 {
                carTrash.Add(MarketSystem._instance._carSystemList[i].gameObject);
                MarketSystem._instance._carSystemList[i] = null;
                
                    //  MarketSystem._instance._carSystemList.RemoveAt(i);

                 }


                 else if (MarketSystem._instance._carSystemList[i]._car.currentPhase == CarMovementView.Phase.TravelingToParking)
                 {
                     MarketSystem._instance._carSystemList[i]._car.FreeParkingSlot();
                carTrash.Add(MarketSystem._instance._carSystemList[i].gameObject);
                MarketSystem._instance._carSystemList[i] = null;

                //  MarketSystem._instance._carSystemList.RemoveAt(i);
            }

                 else if (MarketSystem._instance._carSystemList[i]._car.currentPhase == CarMovementView.Phase.Parked)
                 {
                     MarketSystem._instance._carSystemList[i]._car.ForceToPark();

                 }
                 else if (MarketSystem._instance._carSystemList[i]._car.currentPhase == CarMovementView.Phase.TravelingToQueue
                     || MarketSystem._instance._carSystemList[i]._car.currentPhase == CarMovementView.Phase.WaitingOnParkingQueue
                     || MarketSystem._instance._carSystemList[i]._car.currentPhase == CarMovementView.Phase.EditPositionInQueue)
                 {
                carTrash.Add(MarketSystem._instance._carSystemList[i].gameObject);
                MarketSystem._instance._carSystemList[i] = null;
                

                //  MarketSystem._instance._carSystemList.RemoveAt(i);
            }

             }
             int _step = 0;
             while (_step < MarketSystem._instance._carSystemList.Count)
             {
            
               if (MarketSystem._instance._carSystemList[_step] == null)
               {
                 MarketSystem._instance._carSystemList.RemoveAt(_step);
                 //Destroy(MarketSystem._instance._carSystemList[_step]);
               }      
               else
               _step++;
             }

        for (int i = 0; i < carTrash.Count; i++)
            Destroy(carTrash[i]);
        carTrash.Clear();

        for (int i = 0; i < MarketSystem._instance._peopleSystemList.Count ; i++)
        {
            if(MarketSystem._instance._peopleSystemList[i].status == PeopleController.eStatus.WAY_TO_MARKET || MarketSystem._instance._peopleSystemList[i].status == PeopleController.eStatus.WAY_TO_PARKING)
            {
                MarketSystem._instance._peopleSystemList[i].peopleView.ForceToReturnCar();
            }
        }
    }


   

    public Transform GetTransformForZone(string zone)
    {
        if(_currentViewModel != null)
          return _currentViewModel.zone2Transform[zone];
        return null;
    }

    public void BuyEmployeeToGenerator(string zone)
    {
        double costNextEmployee = 0;

        if (zone != "PARKING")
        
            costNextEmployee = generatorControllers[zone].GetCostNextEmployee();
        
        else if (zone == "PARKING")

            costNextEmployee = _parkingVisualModel.GetCostForUpgrade();

        if(money > costNextEmployee)
        {
           
            money -= costNextEmployee * (1 - GamePlaySytem.instance._reduceSalaryCost);
            if (zone != "PARKING")
            {
                generatorControllers[zone].EmployeesBought++;
                generatorControllers[zone].NewEmployee();
            }

            else if (zone == "PARKING")
                UpgradeParking();
            GamePlaySytem.instance._saveSystem.Save();

        }
    }


    public bool LevelUpGenerator(string zone, bool updateVisuals = true)
    {
        if(zone.Contains("CHECKOUT"))
        {
            int num = int.Parse(Regex.Match(zone, "\\d+").Value);
            CheckoutModel checkoutModel = checkOutSystem.checkOutList[num -1];
            int level = checkoutModel.level;
            double levelUpPriceForLevel = checkoutModel.GetLevelUpPrice(level);
            if (money >= levelUpPriceForLevel)
            {
                money -= levelUpPriceForLevel;
                checkoutModel.level = level + 1;
                GamePlaySytem.instance._saveSystem.Save();
            }
        }    
        else
        {
            GeneratorModel generatorModel = generatorControllers[zone];
            int level = generatorModel.level;
            int lastProductIndex = generatorModel.FindIndexForLevel(level);
            double levelUpPriceForLevel = generatorModel.GetLevelUpPrice(level);
            if (money >= levelUpPriceForLevel)
            {
                money -= levelUpPriceForLevel;
                generatorModel.level = level + 1;
                bool flag = generatorModel.BuildOrLevelUp(updateVisuals);
                if (generatorModel.ProductIndex > lastProductIndex)
                {
                    //Debug.Log("Upgrade");
                    //UpgradeShop(zone);
                }

                int level2 = level + 1;
                int num = level;
                if (num >= 0)
                {
                    int num2 = generatorModel.FindIndexForLevel(level2);
                    if (generatorModel.FindIndexForLevel(num) != num2)
                    {

                    }
                }
                GamePlaySytem.instance._saveSystem.Save();
            }
        }    
            
            return false;
    }

    public bool LevelUpResearch(string zone)
    {
        if(zone != "PARKING")
        {
            GeneratorModel generatorModel = generatorControllers[zone];
            double nextLevelResearchCost = generatorModel.NextLevelResearchCost();
            if (money < nextLevelResearchCost)
            {
                return false;
            }
            if (generatorModel.researchLevel >= generatorModel.MaxResearchLevel)
            {
                return false;
            }
            generatorModel.researchLevel++;
            money -= nextLevelResearchCost;

            for (int i = 0; i < generatorModel.employees.Length; i++)
            {
                if (i < generatorModel.TotalEmployees)
                {

                    EmployeeController employeeController = generatorModel.employees[i].GetComponent<EmployeeController>();
                    employeeController.timeToProducting = generatorModel.GetTimeToProduce();

                }
            }
            GamePlaySytem.instance._saveSystem.Save();
        }
        else if(zone == "PARKING")
        {
            
            double nextLevelResearchCost = _parkingVisualModel.NextLevelResearchCost();
            if (money < nextLevelResearchCost)
            {
                return false;
            }
            if (_parkingVisualModel.researchLevel >= _parkingVisualModel.MaxResearchLevel)
            {
                return false;
            }
            _parkingVisualModel.researchLevel++;
            money -= nextLevelResearchCost;
            GamePlaySytem.instance._saveSystem.Save();
        }
        
        return true;
    }

    public GeneratorModel GetGeneratorModel(string zone)
    {
        if (generatorControllers.ContainsKey(zone))
            return generatorControllers[zone];
        else
            return null;
        
    }

    public GeneratorModel FindModel(string id)
    {
        generatorControllers.TryGetValue(id, out GeneratorModel value);
        return value;
    }

    public bool IsShopUnlocked(string shopId)
    {
        //GeneratorModel generatorModel = GetGeneratorModel(shopId);
        bool _check = false;
        /*
        if (money >= generatorModel._dataModel.build_cost)
            _check = true;
        */
        if (generatorControllers[FindPreviosShop(shopId)].level > 0)
            _check = true;
        return _check;
    }

    public bool CanBuildNewShop
    {
        get
        {
            foreach (GeneratorStaticModel value in GamePlaySytem.instance._generatorStaticModels.Values)
            {
                if (value.is_shop && IsShopUnlocked(value.id) && money >= value.build_cost)
                {
                    return true;
                }
            }
            return false;
        }
    }

    public bool HasGeneralManager { get;  set; }

    public int GetShopBuildCount()
    {
        int _count = 0;

        for(int i = 0; i < generatorControllers.Count; i++)
        {
            if (generatorControllers.ElementAt(i).Value.level > 0)
                _count++;
        }

        return _count;
    }

    public void BuildNextShop()
    {

        double levelUpPriceForLevel = GamePlaySytem.instance._generatorStaticModels[_shopNameList[evolution +1]].build_cost;
        if (money < levelUpPriceForLevel)
            return;
            money -= levelUpPriceForLevel;
            evolution++;
        GameObject lastMarket = _currentViewModel.gameObject;
        GameObject marketVisual = (GameObject)Instantiate(Resources.Load("supermarket/market/market_lvl_" + (evolution + 1)));
        _currentViewModel = marketVisual.GetComponent<MarketVisualModel>();
        _currentViewModel.Init();
        marketVisual.transform.parent = marketPosition.parent;
        marketVisual.transform.position = marketPosition.position;
        marketVisual.transform.rotation = marketPosition.rotation;
        

        for(int i = 0; i < generatorControllers.Count; i++)
        {
            if(generatorControllers.ElementAt(i).Value.level > 0)
            {
                generatorControllers.ElementAt(i).Value.gameObject.transform.position = GetTransformForZone(generatorControllers.ElementAt(i).Key).position;
                generatorControllers.ElementAt(i).Value.SortAllPeopleInQueue();
            }    
        }

        for (int j = 0; j < checkOutSystem.checkOutList.Count; j++)
        {
            if(checkOutSystem.checkOutList[j].level > 0)
            {
                checkOutSystem.checkOutList[j].transform.position = GetTransformForZone("CHECKOUT" + (j + 1).ToString()).position;
                checkOutSystem.checkOutList[j].SortAllPeopleInQueue();
            }
           
        }

        GameObject bakeryVisual = (GameObject)Instantiate(Resources.Load("supermarket/" +
                    GamePlaySytem.instance._generatorStaticModels[_shopNameList[evolution]]._visualLevelPrefab + "_" + 1));
        bakeryVisual.transform.position = GetTransformForZone(_shopNameList[evolution]).position;
        bakeryVisual.transform.parent = parkingPosition.parent;
        bakeryVisual.transform.rotation = marketPosition.rotation;
        GeneratorModel bakeryModel = bakeryVisual.GetComponent<GeneratorModel>();
        generatorControllers.Remove(_shopNameList[evolution]);
        _saveModel.zones[_shopNameList[evolution]].level = 1;
        bakeryModel.Init(_saveModel.zones[_shopNameList[evolution]], GamePlaySytem.instance._generatorStaticModels[_shopNameList[evolution]]);
        generatorControllers.Add(_shopNameList[evolution], bakeryModel);
        Destroy(lastMarket);
        GamePlaySytem.instance._saveSystem.Save();
        StartCoroutine(ScanScene());
    }

    IEnumerator ScanScene()
    {
        yield return new WaitForSeconds(0.1f);
        AstarPath.active.Scan();
    }
    
    public int GetTotalCarParked()
    {
        int _total = 0;
       
        for(int i = 0; i < _parkingVisualModel.Lanes.Length; i++)
        {
            for(int j = 0; j < _parkingVisualModel.Lanes[i].SlotsCount; j++)
                if (!_parkingVisualModel.Lanes[i].GetSlot(j)._associateWithCar)
                {
                    _total++;
                }
        }
        return _total;
    }

    public void ProcessResearchUpgrade(string _id)
    {

    }

    public double SimulateMarketProduction(int startTimestamp, int endTimestamp)
    {
        if (startTimestamp >= endTimestamp)
        {
            return 0.0;
        }
       
        int num = endTimestamp - startTimestamp;
        double _moneyPending = 0;

        _moneyPending = (num/60) * CalculateProfits();

        return _moneyPending;
    }
    public double CalculateProfits()
    {
        double _num = 0.0f;
        for (int i = 0; i < generatorControllers.Count; i++)
        {
            if (generatorControllers.ElementAt(i).Value.level > 0)
            {

                _num += generatorControllers.ElementAt(i).Value.TotalProductionPerMinute;
            }
        }
        return _num;
    }


    public string FindPreviosShop(string _id)
    {
        int _index = 0;

        for (int i = 0; i < _shopNameList.Length; i++)
        {
            if (_id == _shopNameList[i])
                _index = i - 1;
        }

        if (_index >= 0)

            return _shopNameList[_index];
        else
            return _shopNameList[0];

    }
}
