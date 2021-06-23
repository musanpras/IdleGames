using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SWS;

public class PeopleController : MonoBehaviour
{
    [SerializeField]
    protected ParticleSystem _payFx;

    [SerializeField]
    protected ParticleSystem _angryFx;

    public Animator characterAnimation;

    public MeshFilter peopleMesh;

    public GameObject[] peopleMeshAsset;

    public double accumulatedCash;

    public enum eStatus
    {
        STOP,
        WAY_TO_MARKET,
        WAY_TO_SHOP_QUEUE,
        EDITTING_POS_IN_QUEUE,
        WAITING_IN_QUEUE_SHOP,
        SHOPPING,
        WAY_TO_CHECKOUT_QUEUE,
        WAITING_IN_CHECKOUT_QUEUE,
        EDITTING_POS_IN_CHECKOUT_QUEUE,
        CHECKOUT,
        WAY_TO_PARKING
    }

    public eStatus status;

    public PeopleView peopleView;

    private void Awake()
    {
        AnimationIdle();
        InitMesh();
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        switch(status)
        {
            case eStatus.WAITING_IN_CHECKOUT_QUEUE:
                AnimationIdle();
                break;

            case eStatus.WAITING_IN_QUEUE_SHOP:
                AnimationIdle();
                break;
            default:
                AnimationWalk();
                break;
        }
    }

    public void PayFX()
    {
        _payFx.Play();
    }

    public void AngryFX()
    {
        _angryFx.Play();
    }

    public void AnimationIdle()
    {
        characterAnimation.SetBool("Walk", false);
    }

    public void AnimationWalk()
    {
        characterAnimation.SetBool("Walk", true);
    }

    void InitMesh()
    {
        int _random = Random.Range(0, peopleMeshAsset.Length);
        peopleMeshAsset[_random].SetActive(true);
    }

    public void AddValue(double v)
    {
        accumulatedCash += v;
    }

}
