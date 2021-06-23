using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ParkingCurrentLevelDisplayer : MonoBehaviour
{
    [SerializeField]
    private TextLocalize _titleLabel;
    [SerializeField]
    private TextLocalize _descriptionLabel;
    [SerializeField]
    private TextMeshProUGUI _levelLabel;
    [SerializeField]
    [FormerlySerializedAs("_employeesProgressLabel")]
    private TextMeshProUGUI _slotsLabel;
    [SerializeField]
    [FormerlySerializedAs("_totalValueLabel")]
    private TextMeshProUGUI _productionLabel;
    [SerializeField]
    [FormerlySerializedAs("_clientsLabel")]
    private TextMeshProUGUI _occupancyLabel;
    [SerializeField]
    private Image _productIcon;
    private ParkingVisualModel _parking;

    public void ShowInternal(ParkingVisualModel element)
    {
        
        _parking = element;
        _titleLabel.key = _parking.id.ToUpper();
        _titleLabel.sheet = "SHOPS";
        _titleLabel.Localize();
        _descriptionLabel.key = _parking.id.ToUpper() + "_DESCRIPTION";
        _descriptionLabel.sheet = "SHOPS";
        _descriptionLabel.Localize();
        _levelLabel.text = _parking.GetParkingVisualLevel().ToString();
        string format = Language.Get("CM_PER_MIN", "COMMON");
        _productionLabel.text = string.Format(format, _parking.GetTotalParkingSlots());
        _occupancyLabel.text = (_parking.OccupancyRate * 100f).ToString("##0") + "%";
        _productIcon.sprite = GamePlaySytem.instance._atlas.GetSprite(_parking.IconName);
        _slotsLabel.text = _parking.TotalSlotsCount.ToString();
        
    }
    public void Refresh()
    {
        
        string format = Language.Get("CM_PER_MIN", "COMMON");
        _productionLabel.text = string.Format(format, _parking.GetTotalParkingSlots());
        _occupancyLabel.text = (_parking.OccupancyRate * 100f).ToString("##0") + "%";
        _parking = GamePlaySytem.instance.marketSystem._view._parkingVisualModel;
        ShowInternal(_parking);
        
    }

    public void Initialize()
    {
        
    }
}
