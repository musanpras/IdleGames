using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarVipController : MonoBehaviour
{

    public enum eStatus
    {
        GOING_TO_PARKING,
        PARKING,
        LEAVE_TO_PARKING
    }

    public eStatus _Status;

    public bool HasGemAsReward;

    [SerializeField]
    private ParticleSystem _glowParticle;

    [SerializeField]
    private CarVipAnimator _carAnimator;

    private bool _hasBeenClick;

    public bool HasBeenClicked
    {
        get;

        private set;
    }


    // Start is called before the first frame update
    void Start()
    {
        _carAnimator.OnClicked = OnClickEvent;
        _hasBeenClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickEvent()
    {
        if(_Status == eStatus.PARKING && !HasBeenClicked)
        {
           // Debug.Log("CLICK TO VIP CAR");
            HasBeenClicked = true;
            WatchVipVideoPopup _popup = UISystem.instance._popupSystem.ShowWatchVipVideoPopup();
            _popup.Init(HasGemAsReward);
            _popup._vipCarPos = transform.position;
            HideGlow();
            double moneyValue = UISystem.instance.CalculateProfits() *(1.0f + Random.Range(-0.5f, 0.5f));
            int gemValue = Mathf.FloorToInt(GamePlaySytem.instance.marketSystem._view.evolution * Random.Range(10, 20) * (1.0f + Random.Range(-0.5f, 0.5f)));

            int _random = Random.Range(0, 10);

            HasGemAsReward = (_random == 5) ? true : false;
            if (HasGemAsReward)
            {
                _popup.RewardGems(gemValue);
            }
            else
            {
                _popup.RewardMoney(moneyValue * (1 + GamePlaySytem.instance._increaseRewardVip));
            }
        }

    }

    public void ShowGlow()
    {
        _glowParticle.Play();
    }

    public void HideGlow()
    {
        _glowParticle.Stop();
    }


}
