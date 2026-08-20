using UnityEngine;

public class IAPController : MonoBehaviour
{
    public string SKU_1000_COINS = "pool_5000_coins";
    public string SKU_5000_COINS = "pool_10000_coins";
    public string SKU_10000_COINS = "pool_25000_coins";
    public string SKU_50000_COINS = "pool_75000_coins";
    public string SKU_100000_COINS = "pool_200000_coins";

    private void Start()
    {
        Object.DontDestroyOnLoad(base.transform.gameObject);
        GameManager.Instance.IAPControl = this;
    }
    
    public void OnPurchaseComplete(int index) { }
    public void BuyProduct(int index) { }
    public void RestorePurchases() { }
}
