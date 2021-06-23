using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class EmployeesProgressWidget : MonoBehaviour
{
    [SerializeField]
    private Slider _progressSlider;
    [SerializeField]
    private Vector2 _offfset;

    public Camera RaycastCamera;

    private Vector2 _uiOffset;

    private RectTransform _canvasRect;

    private RectTransform _rect;

    public EmployeeController _employee;

    private int EmployeeIndex;

    public bool IsHiding
    {
        get;
        set;
    }

    public void AssociateWith(EmployeeController employee)
    {
        _employee = employee;
        EmployeeIndex = _employee.Index;
        _progressSlider.value = 1f;
        Canvas componentInParent = GetComponentInParent<Canvas>();
        _canvasRect = componentInParent.GetComponent<RectTransform>();
        _uiOffset = new Vector2(_canvasRect.sizeDelta.x * 0.5f, _canvasRect.sizeDelta.y * 0.5f);
    }

    public void Clean()
    {
        _employee = null;
    }

    public void SetProgress(float value)
    {
        _progressSlider.value = 1.0f - value;
    }

    public Sequence Show(float duration = 0.2f)
    {
        CanvasGroup component = GetComponent<CanvasGroup>();
        RectTransform component2 = GetComponent<RectTransform>();
        Sequence sequence = DOTween.Sequence();
        sequence.SetId("GamePlay");
        sequence.Append(component.DOFade(1f, duration));
        sequence.Insert(0f, component2.DOScale(Vector3.one, duration));
        return sequence;
    }

    public Sequence Hide(float duration = 0.2f)
    {
        CanvasGroup component = GetComponent<CanvasGroup>();
        RectTransform component2 = GetComponent<RectTransform>();
        Sequence sequence = DOTween.Sequence();
        sequence.SetId("GamePlay");
        if (duration > 0f)
        {
            sequence.Append(component.DOFade(0f, duration));
            sequence.Insert(0f, component2.DOScale(Vector3.zero, duration));
        }
        else
            component2.localScale = Vector3.zero;
        return sequence;
    }

    protected void LateUpdate()
    {
        if(_employee != null)
        {
            Vector2 a = ConvertWorldToCanvasPoint(_employee.transform.position);
            a += _offfset;
            if(_rect == null)
            {
                _rect = GetComponent<RectTransform>();
            }
            _rect.anchoredPosition = a;
        }
        
    }

    private Vector2 ConvertWorldToCanvasPoint(Vector3 worldPoint)
    {
        Vector2 vector = ((RaycastCamera != null) ? RaycastCamera : Camera.main).WorldToViewportPoint(worldPoint);
        return new Vector2(vector.x * _canvasRect.sizeDelta.x, vector.y * _canvasRect.sizeDelta.y) - _uiOffset;
    }
}