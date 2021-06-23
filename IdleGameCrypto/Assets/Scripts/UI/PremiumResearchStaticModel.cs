using System;

[Serializable]
public class PremiumResearchStaticModel
{
    public enum eResearchType
    {
        CLIENTS_SPEED,
        SHOP_SPEED_MULT,
        CASHIER_SPEED_MULT,
        FAMILY_MULT,
        SALARY_COST_MULT,
        CASHIER_COST_MULT,
        EVOLUTION_MULT,
        MAX_VIDEO_POWER_UP,
        SHOP_COST_MULT,
        VIP_REWARD_BONUS_MULT,
        GENERAL_MANAGER,
        QUALITY_SEAL,
        MONEY_VAN_MULT
    }
  
    public string id;

    public eResearchType type;
 
    public string shop;
 
    public int tier;
  
    public string icon;

    public int prices;

    public bool show_in_menu;

    public double _bonus;

}