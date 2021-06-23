using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveModel
{
    public int evolution;
    public int timestamp;
    public double money;
    public int gems;
    public float elapsedTime;
    public int finishVideoPowerUpTimestamp;
    public bool videoPrizeToCollect;
    public int videosWatchedForAdCampaign;
    public double moneyOnPockets;
    public Dictionary<string, GeneratorSaveModel> zones = new Dictionary<string, GeneratorSaveModel>();
    public double costMultiplier = 1.0;
    public double evolutionMultiplier = 1.0;
    public int videoPowerUpTimeIncrease;
    public int maxVideoPowerUpTimeIncrease;
    public int shopsBuilded;
    public int customersServed;
    public Dictionary<string, double> statistics = new Dictionary<string, double>();
   // public CurrentQuestModel currentQuest = new CurrentQuestModel();
    public static SaveModel CreateInitialModel()
    {
        SaveModel saveModel = new SaveModel();
        //saveModel.money = MarketConfigurationStaticModel.initialMoney;
        //new MarketModel();
        saveModel.money = 200;
        saveModel.gems = 20;
        foreach (KeyValuePair<string, GeneratorStaticModel> cenerator in GamePlaySytem.instance._generatorStaticModels)
        {
            GeneratorSaveModel generatorSaveModel = new GeneratorSaveModel();
            generatorSaveModel.id = cenerator.Key;
            generatorSaveModel.templateId = cenerator.Key;
            saveModel.zones.Add(cenerator.Key, generatorSaveModel);

        }
        saveModel.zones["FRUIT"].level = 1;
        saveModel.zones["CHECKOUT1"].level = 1;
        saveModel.zones["PARKING"].level = 1;
        saveModel.shopsBuilded = 1;
        return saveModel;
    }

}
