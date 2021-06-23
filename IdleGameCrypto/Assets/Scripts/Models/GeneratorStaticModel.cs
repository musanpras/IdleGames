using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorStaticModel : MonoBehaviour
{
    public enum eBonusType
    {
        UNKNOWN,
        BONUS_PRODUCTION,
        BONUS_SPEED,
        EMPLOYEE
    }
    public string id;
    public double build_cost;
    public double base_cost;
    public double grow_cost;
    public double grow_production;
    public double start_speed;
    public double start_sell_price;
    public int max_level = 1;
    public int max_queue;
    public int maxEmployees = 5;
    public string requirement;
    public int id_zone;
    public bool is_shop;
    public bool is_checkout;
   
    public List<int> bonusAtLevel;
    
    public List<string> bonusType;

    public List<float> bonusParameter;
 
    public int bonusStellar;
   
    public List<double> employeePrice;
   
    public List<int> _visualLevelChange;
   
    public string _visualLevelPrefab;
    
    public string controller;
   
    public List<double> salaryPrice;
  
    public List<float> salaryBonus;
    public int researchMaxLevel => salaryPrice.Count;

}
