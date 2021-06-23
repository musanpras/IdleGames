using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLoad : MonoBehaviour
{
    public SaveSystem _saveSystem;
    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData()
    {
        _saveSystem.Init();
        SaveModel _saveModel = new SaveModel();
        _saveModel.evolution = 2;
        _saveModel.money = 100;
        _saveModel.moneyOnPockets = 50;
        GeneratorSaveModel _generatorSaveModel = new GeneratorSaveModel();
        _generatorSaveModel.id = "FRUIT";
        _generatorSaveModel.level = 3;
        _generatorSaveModel.productIndex = 8;
        _saveModel.zones.Add(_generatorSaveModel.id, _generatorSaveModel);
        _saveSystem.Save(_saveModel);
    }

    public void LoadData()
    {
        SaveModel _saveModel = _saveSystem.LoadLocalSave();
        if(_saveModel != null)
        {
            Debug.Log(_saveModel.evolution);
            Debug.Log(_saveModel.money);
            Debug.Log(_saveModel.moneyOnPockets);
            Debug.Log(_saveModel.zones.Count);
        }
        else
            Debug.Log("DATA NULL");

    }
}
