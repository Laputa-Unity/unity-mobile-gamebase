using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "ScriptableObject/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    public List<ItemData> itemData;

    public void Initialize()
    {
        //if (IAPManager.Instance.IsPurchased(Constant.IAP_VIP) || IAPManager.Instance.IsPurchased(Constant.IAP_ALL_SKINS)) UnlockAllSkins();
        UnlockDefaultSkins();
    }

    public void UnlockDefaultSkins()
    {
        foreach (ItemData item in itemData)
        {
            if (item.buyType == BuyType.Default)
            {
                item.ClaimItem();
            }
        }
    }

    public void UnlockAllSkins()
    {
        foreach (var data in itemData)
        {
            data.IsUnlocked = true;
        }
    }
    public ItemData GetItemData(string itemIdentity)
    {
        return itemData.Find(item => item.Identity == itemIdentity);
    }

    public List<ItemData> GetListItemDataByType(ItemType itemType)
    {
        return itemData.FindAll(item => item.Type == itemType);
    }

    public ItemData GetGiftItemData()
    {
        List<ItemData> tempList =
            itemData.FindAll(item => !item.IsUnlocked && (item.buyType == BuyType.BuyCoin || item.buyType == BuyType.WatchAds));
        return tempList.Count > 0?tempList[Random.Range(0, tempList.Count)]:null;
    }
}

public class ItemIdentity
{
    public string Identity => $"{Type}_{NumberID}";

    public string Name;
    public ItemType Type;
    public int NumberID;
}

[Serializable]
public class ItemData : ItemIdentity
{
    public BuyType buyType;
    public Sprite shopIcon;
    public int CoinValue;

    public void ClaimItem()
    {
        IsUnlocked = true;
        EquipItem();
    }

    public void EquipItem()
    {
        Data.SetItemEquipped(Identity);
    }
    
    public bool IsUnlocked
    {
        get
        {
            Data.IdItemUnlocked = Identity;
            return Data.IsItemUnlocked;
        }

        set
        {
            //FirebaseManager.OnClaimItemSkin(Identity);
            Data.IdItemUnlocked = Identity;
            Data.IsItemUnlocked = value;
        }
    }
}

public enum BuyType
{
    Default,
    BuyCoin,
    DailyReward,
    WatchAds,
    Event,
}

public enum ItemType
{
    PlayerSkin,
    WeaponSkin,
}