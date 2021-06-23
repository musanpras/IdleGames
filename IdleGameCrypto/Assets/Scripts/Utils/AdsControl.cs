using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GoogleMobileAds.Api;
using System;
using UnityEngine.Advertisements;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class AdsControl : MonoBehaviour
{
    protected AdsControl()
    {
    }
    private static AdsControl _instance;
    InterstitialAd interstitial;
    RewardBasedVideoAd rewardBasedVideo;
    BannerView bannerView;
    ShowOptions options;
    public string AdmobID_Android, AdmobID_IOS, BannerID_Android, BannerID_IOS, AdmobRW_Android, AdmobRW_IOS;
    public string UnityID_Android, UnityID_IOS, UnityZoneID;
    public static AdsControl Instance { get { return _instance; } }

    public enum ADS_STATE
    {
        REWARD_VIP,
        MULTI_EARNING,
        DOUBLE_REWARD
    };

    public ADS_STATE currentState;

    void Awake()
    {
        if (FindObjectsOfType(typeof(AdsControl)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        MakeNewInterstial();
       // RequestBanner();
       // HideBanner();

        if (Advertisement.isSupported)
        { // If the platform is supported,
#if UNITY_IOS
			Advertisement.Initialize (UnityID_IOS); // initialize Unity Ads.
#endif
#if UNITY_ANDROID
            Advertisement.Initialize(UnityID_Android); // initialize Unity Ads.
#endif
        }
        options = new ShowOptions();
        options.resultCallback = HandleShowResult;

        // Get singleton reward based video ad reference.
        this.rewardBasedVideo = RewardBasedVideoAd.Instance;

        // Called when an ad request has successfully loaded.
        rewardBasedVideo.OnAdLoaded += HandleRewardBasedVideoLoaded;

        // Called when an ad request failed to load.
        rewardBasedVideo.OnAdFailedToLoad += HandleRewardBasedVideoFailedToLoad;

        // Called when an ad is shown.
        rewardBasedVideo.OnAdOpening += HandleRewardBasedVideoOpened;

        // Called when the ad starts to play.
        rewardBasedVideo.OnAdStarted += HandleRewardBasedVideoStarted;

        // Called when the user should be rewarded for watching a video.
        rewardBasedVideo.OnAdRewarded += HandleRewardBasedVideoRewarded;

        // Called when the ad is closed.
        rewardBasedVideo.OnAdClosed += HandleRewardBasedVideoClosed;

        // Called when the ad click caused the user to leave the application.
        rewardBasedVideo.OnAdLeavingApplication += HandleRewardBasedVideoLeftApplication;

        GetNewRWAd();

        DontDestroyOnLoad(gameObject); 
    }
    public void GetNewRWAd()
    {
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
#if UNITY_ANDROID
        rewardBasedVideo.LoadAd(request, AdmobRW_Android);
#endif
#if UNITY_IOS
        rewardBasedVideo.LoadAd(request, AdmobRW_IOS);
#endif
    }
    public void HandleRewardBasedVideoLoaded(object sender, EventArgs args)
    {
    }
    public void HandleRewardBasedVideoFailedToLoad(object sender, AdFailedToLoadEventArgs args)
    {
    }
    public void HandleRewardBasedVideoOpened(object sender, EventArgs args)
    {
    }
    public void HandleRewardBasedVideoStarted(object sender, EventArgs args)
    {
    }
    public void HandleRewardBasedVideoClosed(object sender, EventArgs args)
    {
        GetNewRWAd();
    }
    public void HandleRewardBasedVideoRewarded(object sender, Reward args)
    {
        //Get Rewarded video

        switch(currentState)
        {
            case ADS_STATE.REWARD_VIP:
                GameObject.FindGameObjectWithTag("VipVideo").GetComponent<WatchVipVideoPopup>().OnVideoFinished(true);
                break;
            case ADS_STATE.MULTI_EARNING:
                GameObject.FindGameObjectWithTag("MultiEarning").GetComponent<MultiplyEarningsMenu>().OnVideoShown(true);
                break;
            case ADS_STATE.DOUBLE_REWARD:
                GameObject.FindGameObjectWithTag("DoubleReward").GetComponent<WelcomeBackMenu>().OnVideoShown(true);
                break;

        }
    }
    public void HandleRewardBasedVideoLeftApplication(object sender, EventArgs args)
    {
    }
    public void HandleInterstialAdClosed(object sender, EventArgs args)
    {
        if (interstitial != null)
            interstitial.Destroy();
        MakeNewInterstial();
    }
    void MakeNewInterstial()
    {
#if UNITY_ANDROID
        interstitial = new InterstitialAd(AdmobID_Android);
#endif
#if UNITY_IPHONE
		interstitial = new InterstitialAd (AdmobID_IOS);
#endif
        interstitial.OnAdClosed += HandleInterstialAdClosed;
        AdRequest request = new AdRequest.Builder().Build();
        interstitial.LoadAd(request);
    }
    public void showAds()
    {
        if (PlayerPrefs.GetInt("RemoveAds") == 0)
        {
            int adsCounter = PlayerPrefs.GetInt("AdsCounter");

            if (adsCounter >= 2)
            {
                if (PlayerPrefs.GetInt("RemoveAds") == 0)
                {
                    if (interstitial.IsLoaded())
                        interstitial.Show();
                    else
                        if (Advertisement.IsReady())

                        Advertisement.Show();
                }
                adsCounter = 0;
            }
            else
            {
                adsCounter++;
            }

            PlayerPrefs.SetInt("AdsCounter", adsCounter);
        }
    }
    public bool GetRewardAvailable()
    {
        bool avaiable = false;
        if (RewardBasedVideoAd.Instance.IsLoaded())
            avaiable = true;
        else
            avaiable = Advertisement.IsReady();
        return avaiable;
    }
    public void ShowRewardVideo(ADS_STATE _state)
    {
        currentState = _state;

       // if (RewardBasedVideoAd.Instance.IsLoaded())
       //     RewardBasedVideoAd.Instance.Show();
       // else
            Advertisement.Show(UnityZoneID, options);
    }

    private void RequestBanner()
    {
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
		string adUnitId = BannerID_Android;
#elif UNITY_IPHONE
		string adUnitId = BannerID_IOS;
#else
		string adUnitId = "unexpected_platform";
#endif
        // Create a 320x50 banner at the top of the screen.
        bannerView = new BannerView(adUnitId, AdSize.SmartBanner, AdPosition.Bottom);
        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the banner with the request.
        bannerView.LoadAd(request);
    }
    public void ShowBanner()
    {
        bannerView.Show();
    }
    public void HideBanner()
    {
        bannerView.Hide();
    }
   
    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                switch (currentState)
                {
                    case ADS_STATE.REWARD_VIP:
                        GameObject.FindGameObjectWithTag("VipVideo").GetComponent<WatchVipVideoPopup>().OnVideoFinished(true);
                        break;
                    case ADS_STATE.MULTI_EARNING:
                        GameObject.FindGameObjectWithTag("MultiEarning").GetComponent<MultiplyEarningsMenu>().OnVideoShown(true);
                        break;
                    case ADS_STATE.DOUBLE_REWARD:
                        GameObject.FindGameObjectWithTag("DoubleReward").GetComponent<WelcomeBackMenu>().OnVideoShown(true);
                        break;

                }
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Failed:
                break;
        }
    }
    public void PlayCallbackRewardVideo(Action<ShowResult> _action)
    {
        ShowOptions _options = new ShowOptions();
        _options.resultCallback = _action;
        Advertisement.Show(UnityZoneID, _options);
    }
    public void PlayDelegateRewardVideo(Action<bool> onVideoPlayed)
    {
        if (Advertisement.IsReady(UnityZoneID))
        {
            Advertisement.Show(UnityZoneID, new ShowOptions
            {
                //pause = true,
                resultCallback = result => {
                    switch (result)
                    {
                        case (ShowResult.Finished):
                            onVideoPlayed(true);
                            break;
                        case (ShowResult.Failed):
                            onVideoPlayed(false);
                            break;
                        case (ShowResult.Skipped):
                            onVideoPlayed(false);
                            break;
                    }
                }
            });
        }
        onVideoPlayed(false);
    }
    public void TestDelegate()
    {
        AdsControl.Instance.PlayDelegateRewardVideo(delegate
        {
            //function
        });
    }
}