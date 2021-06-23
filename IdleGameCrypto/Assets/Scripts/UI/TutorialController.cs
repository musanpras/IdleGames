using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using MyTween;

public class TutorialController : MonoBehaviour
{
    [SerializeField]
    private GameObject _tutPanel;
    [SerializeField]
    private TextMeshProUGUI _desText;
    [SerializeField]
    private TextMeshProUGUI _titleText;
    [SerializeField]
    private string _introduce;
    [SerializeField]
    private string _firstCustomer;
    [SerializeField]
    private string _introduceParking;
    [SerializeField]
    private string _introduceShoping;
    [SerializeField]
    private string _introduceCheckout;
    [SerializeField]
    private string _introduceFirstSale;
    [SerializeField]
    private string _introduceUpgrade;
   // [SerializeField]
  //  private Image _characterFace;

   // [SerializeField]
   // private Sprite[] _faceIconList;
    [SerializeField]
    private GameObject _tutPoint;
    // Start is called before the first frame update
    void Start()
    {
        if(PlayerPrefs.GetInt("TutComplete") == 0)
           StartCoroutine(ShowTutorial());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator ShowTutorial()
    {
        Camera.main.transform.position = new Vector3(-580.0f,83.0f,86.0f);
        _tutPanel.SetActive(true);
        //_characterFace.sprite = _faceIconList[0];
        _titleText.text = "Assistant";
        _desText.text = _introduce;
        yield return new WaitForSeconds(7.0f);
       // _characterFace.sprite = _faceIconList[1];
        _desText.text = _firstCustomer;
        yield return new WaitForSeconds(5.0f);
      //  _characterFace.sprite = _faceIconList[2];
        _desText.text = _introduceParking;
        yield return new WaitForSeconds(4.0f);
        Tween tweenX = Tween.PositionX(Camera.main.transform, Tween.TweenStyle.Linear, -580.0f,-584,7000);
        Tween tweenZ = Tween.PositionZ(Camera.main.transform, Tween.TweenStyle.Linear, 86.0f, 75, 7000);
        yield return new WaitForSeconds(9.0f);
       // _characterFace.sprite = _faceIconList[3];
        _desText.text = _introduceShoping;
        yield return new WaitForSeconds(12.0f);
       // _characterFace.sprite = _faceIconList[0];
        _desText.text = _introduceCheckout;
        yield return new WaitForSeconds(10.0f);
       // _characterFace.sprite = _faceIconList[1];
        _desText.text = _introduceFirstSale;
        yield return new WaitForSeconds(3.0f);
       // _characterFace.sprite = _faceIconList[2];
        _desText.text = _introduceUpgrade;
        yield return new WaitForSeconds(5.0f);
        _tutPanel.SetActive(false);
        _tutPoint.SetActive(true);
        _tutPoint.transform.localPosition = new Vector3(0, 2, -16);
        yield return new WaitForSeconds(5.0f);
        _tutPoint.SetActive(false);
        PlayerPrefs.SetInt("TutComplete", 1);

    }
}
