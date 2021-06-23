using DG.Tweening;
using UnityEngine;
public class MarketVisualFeedbackSystem : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField]
    private bool _showMoneyFeedback = true;
    [SerializeField]
    [Range(0f, 2f)]
    private float _moneyVisibleDuration = 1f;
    [SerializeField]
    private bool _showProductsFeedback;
    [SerializeField]
    [Range(0f, 2f)]
    private float _productVisibleDuration = 1f;
    [SerializeField]
    private GameObject _moneyFeedBackPrefab;
    public void ShowMoneyFeedback(double amount, Vector3 originPoint)
    {
        if (_showMoneyFeedback)
        {
            
            MoneyFeedback moneyFeedback = ProvideMoneyFeedback();
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("Gameplay");
            sequence.Append(moneyFeedback.ShowAndHide(amount, originPoint, _moneyVisibleDuration));
            sequence.AppendCallback(delegate
            {
                FreeMoneyFeedback(moneyFeedback);
            });
            
        }
    }
    /*
    public void ShowProductPurchased(string productIconName, Vector3 origin, Vector3 target)
    {
        if (_showProductsFeedback)
        {
            
            ProductPurchaseFeedback purchaseFeedback = ProvidePurchaseFeedback();
            Sequence sequence = DOTween.Sequence();
            sequence.SetId("Gameplay");
            sequence.Append(purchaseFeedback.Show(productIconName, origin, target, _productVisibleDuration));
            sequence.AppendCallback(delegate
            {
                FreeProductPurchaseFeedback(purchaseFeedback);
            });
            
        }
    }
    */
    private MoneyFeedback ProvideMoneyFeedback()
    {
        return GameObject.Instantiate(_moneyFeedBackPrefab).GetComponent<MoneyFeedback>();
    }
    private void FreeMoneyFeedback(MoneyFeedback moneyFeedback)
    {
        Destroy(moneyFeedback.gameObject);
    }
    /*
    private ProductPurchaseFeedback ProvidePurchaseFeedback()
    {
        return _gameplayPool.Provide<ProductPurchaseFeedback>("ProductPurchaseFeedback");
    }
    private void FreeProductPurchaseFeedback(ProductPurchaseFeedback purchaseFeedback)
    {
        _gameplayPool.unloadObject(purchaseFeedback.gameObject);
    }
    */
}
