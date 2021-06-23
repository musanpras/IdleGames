using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    public SaveModel lastSaveModel;

    private JsonSystem _jsonSystem;

    private void Awake()
    {
       
    }

    public void Init()
    {
        _jsonSystem = new JsonSystem();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Save()
    {
        if(GamePlaySytem.instance != null)
        {
            SaveModel saveModel = GetSaveModel();
            if(saveModel != null)
            {
                Save(saveModel);
            }
        }
    }

    public void Save(SaveModel saveModel)
    {
        //lastSaveModel = GetSaveModel();
        if(_jsonSystem == null)
        {
            Debug.Log("CAN NOT SAVE");
            return;
        }

        string serialzedSaveModel = _jsonSystem.SerializeObject(saveModel);
        SaveInDevice(serialzedSaveModel);
    }

    private SaveModel GetSaveModel()
    {
        SaveModel saveMode = new SaveModel();
        MarketView _market = GamePlaySytem.instance.marketSystem._view;
        saveMode.evolution = _market.evolution;
        saveMode.gems = _market.gems;
        saveMode.money = _market.money + _market.moneyPending;
        saveMode.finishVideoPowerUpTimestamp = _market.finishVideoPowerUpTimer;
        saveMode.timestamp = GamePlaySytem.instance._serverTimer.GetServerTimestamp();
        //save shop visual
        foreach(KeyValuePair<string, GeneratorModel> zone in _market.generatorControllers)
        {
            GeneratorModel _value = zone.Value;
            GeneratorSaveModel _generatorSaveModel = new GeneratorSaveModel();
            _generatorSaveModel.id = _value.id;
            _generatorSaveModel.level = _value.level;
            _generatorSaveModel.productIndex = _value.ProductIndex;
            _generatorSaveModel.employeesBought = _value.EmployeesBought;
            _generatorSaveModel.researchLevel = _value.researchLevel;
            saveMode.zones.Add(zone.Key, _generatorSaveModel);
        }

        //save parking visual

        GeneratorSaveModel _parkingSaveModel = new GeneratorSaveModel();
        _parkingSaveModel.id = _market._parkingVisualModel .id;
        _parkingSaveModel.level = _market._parkingVisualModel.level;
        //_parkingSaveModel.productIndex = _market._parkingVisualModel.ProductIndex;
       // _parkingSaveModel.employeesBought = _market._parkingVisualModel.EmployeesBought;
        _parkingSaveModel.researchLevel = _market._parkingVisualModel.researchLevel;
        saveMode.zones.Add("PARKING", _parkingSaveModel);

        //save check out

        for(int i = 0; i < _market.checkOutSystem.checkOutList.Count; i++)
        {
            GeneratorSaveModel _checkoutSaveModel = new GeneratorSaveModel();
            _checkoutSaveModel.id = _market.checkOutSystem.checkOutList[i].id;
            _checkoutSaveModel.level = _market.checkOutSystem.checkOutList[i].level;
            _checkoutSaveModel.productIndex = _market.checkOutSystem.checkOutList[i].ProductIndex;
            //_checkoutSaveModel.employeesBought = _market.checkOutSystem.checkOutList[i].EmployeesBought;
           // _checkoutSaveModel.researchLevel = _market.checkOutSystem.checkOutList[i].researchLevel;
            saveMode.zones.Add("CHECKOUT" + (i + 1).ToString(), _checkoutSaveModel);
        }


        return saveMode;
    }

    private void SaveInDevice(string serializedSaveModel)
    {
        IOUtil.SaveToFile(GetPath("Save"),serializedSaveModel);
        Debug.Log("SAVE OK");
    }

    public static string GetPath(string fileName)
    {
        return Application.persistentDataPath + "/Save" + fileName + ".bytes";
    }

    public bool IsLocalSaveAvailable()
    {
        return IOUtil.ExistsFile(GetPath("Save"));
    }

    public SaveModel LoadLocalSave()
    {
        string text = IOUtil.LoadStringFromFile(GetPath("Save"));
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }
        return _jsonSystem.DeserializeObject<SaveModel>(text);
    }
    /*
   private MarketView LoadSaveModel(SaveModel saveModel)
    {


        MarketView _market = new MarketView();
        _market.evolution = saveModel.evolution;
        _market.money = saveModel.money;
        _market.moneyPending = saveModel.moneyOnPockets;
       
       foreach(KeyValuePair<string, GeneratorSaveModel> _generator in saveModel.zones)
        {
            GeneratorModel _generatorModel = new GeneratorModel();
            _generatorModel.id = _generator.Value.id;
            _generatorModel.level = _generator.Value.level;
            _generatorModel.ProductIndex = _generator.Value.productIndex;
            _generatorModel.EmployeesBought = _generator.Value.employeesBought;
            _market.generatorControllers.Add(_generator.Key, _generatorModel);
        }
        return _market;

    }
    */
    public SaveModel LoadMarketView()
    {
        SaveModel saveModel = LoadLocalSave();
        if(saveModel == null)
        {
            saveModel = SaveModel.CreateInitialModel();
        }
       
        return saveModel;
        
    }

    public void DeleteData()
    {
        if(GetPath("Save") != null)
         IOUtil.RemoveFile(GetPath("Save"));
    }
}
