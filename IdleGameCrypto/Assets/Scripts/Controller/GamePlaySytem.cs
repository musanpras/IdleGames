using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GamePlaySytem : MonoBehaviour
{
    public Vector3 parkingPos, fruitPos, checkOutPos;

    public Transform stageRoot;

    public MarketSystem marketSystem;

    public CarVipSystem carVipSystem;

    public DataJsonSystem _dataObject;

    public Dictionary<string, GeneratorStaticModel> _generatorStaticModels = new Dictionary<string, GeneratorStaticModel>();

    public string[] modelIDList;

    public UISystem _UISytem;

    public static GamePlaySytem instance;

    public SaveSystem _saveSystem;

    public Atlas _atlas;

    public ServerTimeController _serverTimer;

    public AudioSystem _audioSystem;

    public float _multiSpeedClient;

    public float _multiSpeedCheckout;

    public float _reduceSalaryCost;

    public float _reduceCashierCost;

    public float _increaseRewardVip;

    public float _increaseCityEarning;

    public int _increaseMarketingCampain;

    public float _doubleMoney;

    private void Awake()
    {
        instance = this;
        Application.targetFrameRate = 60;
        Init();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _UISytem.Tick(Time.deltaTime);
        marketSystem.Tick(Time.deltaTime);
        carVipSystem.Tick(Time.deltaTime);
       // _audioSystem.UpdateSystem(Time.deltaTime);
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            _saveSystem.Save();
            Application.Quit();
        }
    }

    private void Init()
    {

        //BuildModel();
        _serverTimer = new ServerTimeController();
        _audioSystem.InitializeSounds();
        //AddVideoPowerUp();
        _atlas.SetUp();
        _dataObject.LoadData();
        RefreshResearchData();

        _saveSystem.Init();
        marketSystem.Init();
        carVipSystem.Init();
        _UISytem.Initialize();

        StartCoroutine(PlayEvolutionAnimation());
    }

    private IEnumerator PlayEvolutionAnimation()
    {
        yield return new WaitForSeconds(0.1f);
        _UISytem.ShowMainMenu();
        yield return new WaitForSeconds(0.5f);
        ShowWelcomeMenu();
    }

    public int GetVideoMultiplierTimeLeft()
    {

        int finishVideoPowerUpTimer = marketSystem._view.finishVideoPowerUpTimer;
        int serverTimer = _serverTimer.GetServerTimestamp();
        return finishVideoPowerUpTimer - serverTimer;

    }

    public void AddVideoPowerUp()
    {
        int serverTimestamp = _serverTimer.GetServerTimestamp();

        if (marketSystem._view.finishVideoPowerUpTimer < serverTimestamp)
            marketSystem._view.finishVideoPowerUpTimer += serverTimestamp + MarketConfigurationStaticModel.videoPowerUpDuration + _increaseMarketingCampain;
        else
            marketSystem._view.finishVideoPowerUpTimer += MarketConfigurationStaticModel.videoPowerUpDuration;
        if (marketSystem._view.finishVideoPowerUpTimer > (serverTimestamp + GetMaxVideoPowerUpDuration() + _increaseMarketingCampain))
            marketSystem._view.finishVideoPowerUpTimer = serverTimestamp + GetMaxVideoPowerUpDuration() + _increaseMarketingCampain;
    }
    public int GetVideoBonusDuration()
    {
        return MarketConfigurationStaticModel.videoPowerUpDuration;
    }
    public int GetMaxVideoPowerUpDuration()
    {
        return MarketConfigurationStaticModel.maxVideoPowerUpDuration;
    }
    void OnApplicationQuit()
    {

        _saveSystem.Save();

    }

    public void ShowWelcomeMenu()
    {
        //_UISytem.ShowComeBackMenu(100,100,100);
        
        int serverTimeStamp = _serverTimer.GetServerTimestamp();
        int timestamp = marketSystem._view.timestamp;
        if(timestamp > 0)
        {
            if (serverTimeStamp > timestamp)
            {
                Debug.Log("EARNING..." + marketSystem._view.SimulateMarketProduction(timestamp, serverTimeStamp) * (marketSystem._view.evolution + 1)/ marketSystem._view.generatorControllers.Count);
                double _earnedMoney = marketSystem._view.SimulateMarketProduction(timestamp, serverTimeStamp) * (marketSystem._view.evolution + 1) / marketSystem._view.generatorControllers.Count;
                if(_earnedMoney >= 1.0 &&  PlayerPrefs.GetInt("TutComplete") == 1)
                 _UISytem.ShowComeBackMenu(marketSystem._view.money, _earnedMoney, serverTimeStamp - timestamp);
            }
            else
                Debug.Log("ZERO");
        }
        
    }

    public void IncreaseClientSpeed()
    {
        PlayerPrefs.SetInt("ClientSpeed", 1);
        _multiSpeedClient = 0.2f;
    }

    public void IncreaseCheckoutSpeed()
    {
        PlayerPrefs.SetInt("CheckoutSpeed", 1);
        _multiSpeedCheckout = 0.2f;
    }

    public void ReduceSalaryCost()
    {
        PlayerPrefs.SetInt("SalaryCost", 1);
        _reduceSalaryCost = 0.2f;
    }

    public void ReduceCashierCost()
    {
        PlayerPrefs.SetInt("CashierCost", 1);
        _reduceCashierCost = 0.2f;
    }

    public void IncreaseRewardVip()
    {
        PlayerPrefs.SetInt("RewardVip", 1);
        _increaseRewardVip = 0.5f;
    }

    public void IncreaseCityEarning()
    {
        PlayerPrefs.SetInt("CityEarning", 1);
        _increaseCityEarning = 0.5f;
    }

    public void IncreaseMarketing()
    {
        PlayerPrefs.SetInt("Marketing", 1);
        _increaseMarketingCampain = 1200;
    }

    public void RefreshResearchData()
    {
        if (PlayerPrefs.GetInt("ClientSpeed") == 1)
            _multiSpeedClient = 0.2f;
        if (PlayerPrefs.GetInt("CheckoutSpeed") == 1)
            _multiSpeedCheckout = 0.2f;
        if (PlayerPrefs.GetInt("SalaryCost") == 1)
            _reduceSalaryCost = 0.2f;
        if (PlayerPrefs.GetInt("CashierCost") == 1)
            _reduceCashierCost = 0.2f;
        if (PlayerPrefs.GetInt("RewardVip") == 1)
            _increaseRewardVip = 0.5f;
        if (PlayerPrefs.GetInt("CityEarning") == 1)
            _increaseRewardVip = 0.2f;
        if (PlayerPrefs.GetInt("Marketing") == 1)
            _increaseMarketingCampain = 1200;
    }
   
}

