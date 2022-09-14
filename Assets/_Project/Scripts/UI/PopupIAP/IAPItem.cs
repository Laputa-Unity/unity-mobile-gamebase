using UnityEngine;

public class IAPItem : MonoBehaviour
{
    public PopupIAP PopupIAP;
    public IAPType IAPType;

    public GameObject BuyBtn;

    public void SetupData()
    {
        
    }

    public void SetupUI()
    {
        switch (IAPType)
        {
            case IAPType.Buy50k:
                break;
            case IAPType.Buy150k:
                break;
            case IAPType.Buy500k:
                break;
            case IAPType.AllSkin:
                BuyBtn.SetActive(!Data.IsIAPPackUnlocked(IAPType.AllSkin.ToString()));
                break;
            case IAPType.RemoveAds:
                BuyBtn.SetActive(!Data.IsIAPPackUnlocked(IAPType.RemoveAds.ToString()));
                break;
            case IAPType.VIP:
                BuyBtn.SetActive(!Data.IsIAPPackUnlocked(IAPType.VIP.ToString()));
                break;
        }
    }

    public void Setup()
    {
        SetupData();
        SetupUI();
    }
    
    public void OnPurchaseSucceed()
    {
        switch (IAPType)
        {
            case IAPType.Buy50k:
                //IAPManager.Instance.PurchaseProduct(Constant.IAP_GOLD1);
                break;
            case IAPType.Buy150k:
                //IAPManager.Instance.PurchaseProduct(Constant.IAP_GOLD2);
                break;
            case IAPType.Buy500k:
                //IAPManager.Instance.PurchaseProduct(Constant.IAP_GOLD3);
                break;
            case IAPType.AllSkin:
                //IAPManager.Instance.PurchaseProduct(Constant.IAP_ALL_SKINS);
                break;
            case IAPType.RemoveAds:
                //IAPManager.Instance.PurchaseProduct(Constant.IAP_REMOVE_ADS);
                break;
            case IAPType.VIP:
                //IAPManager.Instance.PurchaseProduct(Constant.IAP_VIP);
                break;
        }
    }
}

public enum IAPType
{
    Buy50k,
    Buy150k,
    Buy500k,
    AllSkin,
    RemoveAds,
    VIP,
}