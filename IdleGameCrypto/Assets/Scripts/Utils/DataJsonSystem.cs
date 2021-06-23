using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SimpleJSON;

public class DataJsonSystem : MonoBehaviour
{
    private JSONNode N;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadFile()
    {
        var jsonString = Resources.Load<TextAsset>("GameData");       
        N = JSON.Parse(jsonString.text);
       
    }

    public void LoadData()
    {
        LoadFile();
        for (int i = 0; i < GamePlaySytem.instance.modelIDList.Length; i++)
        {
            
            GeneratorStaticModel _model = new GeneratorStaticModel();
            _model.id = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_id"];
            _model.build_cost = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_build_cost"].AsDouble;
            _model.base_cost = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_base_cost"].AsDouble;
            _model.grow_cost = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_grow_cost"].AsDouble;
            _model.grow_production = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_grow_production"].AsDouble;
            _model.start_speed = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_start_speed"].AsDouble;
             _model.start_sell_price = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_start_sell_price"].AsDouble;
             _model.max_level = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_max_level"].AsInt;
             _model.max_queue = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_max_queue"].AsInt;
             _model.requirement = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_requirement"];
            _model.id_zone = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_id_zone"].AsInt;
            _model.is_shop = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_is_shop"].AsBool;
            _model.is_checkout = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_is_checkout"].AsBool;

            _model.bonusAtLevel = new List<int>();
            for (int i1 = 0; i1 < N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_bonus_level"].AsArray.Count; i1 ++)
            {               
                _model.bonusAtLevel.Add(N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_bonus_level"][i1].AsInt);
            }


            _model.bonusType = new List<string>();
            for (int i1 = 0; i1 < N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_bonus_type"].AsArray.Count; i1++)
            {
                _model.bonusType.Add(N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_bonus_type"][i1]);
            }

            _model.bonusParameter = new List<float>();
            for (int i1 = 0; i1 < N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_bonus_parameter"].AsArray.Count; i1++)
            {
                _model.bonusParameter.Add(N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_bonus_parameter"][i1].AsFloat);
            }


             _model.bonusStellar = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_bonus_stellar"].AsInt;


            _model.employeePrice = new List<double>();
            for (int i1 = 0; i1 < N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_employee_price"].AsArray.Count; i1++)
            {
                _model.employeePrice.Add(N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_employee_price"][i1].AsDouble);
            }


            _model._visualLevelChange = new List<int>();
            for (int i1 = 0; i1 < N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_visual_level"].AsArray.Count; i1++)
            {
                _model._visualLevelChange.Add(N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_visual_level"][i1].AsInt);
            }



          
             _model._visualLevelPrefab = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_visual_prefab"];
             _model.controller = N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_controller"];

            

            _model.salaryPrice = new List<double>();
            for (int i1 = 0; i1 < N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_salary_price"].AsArray.Count; i1++)
            {
                _model.salaryPrice.Add(N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_salary_price"][i1].AsDouble);
            }


            _model.salaryBonus = new List<float>();
            for (int i1 = 0; i1 < N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_salary_bonus"].AsArray.Count; i1++)
            {
                _model.salaryBonus.Add(N[GamePlaySytem.instance.modelIDList[i]]["GeneratorModelType"]["_salary_bonus"][i1].AsFloat);
            }

           GamePlaySytem.instance._generatorStaticModels.Add(_model.id, _model);
           
        }

        
    }
}
