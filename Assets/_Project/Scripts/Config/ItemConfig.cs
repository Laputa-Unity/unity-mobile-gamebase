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
        UnlockDefaultSkins();
    }

    public void UnlockDefaultSkins()
    {
        foreach (ItemData item in itemData)
        {
            if (item.buyType == BuyType.Default)
            {
                item.IsUnlocked = true;
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
        return itemData.FindAll(item => item.type == itemType);
    }

    public ItemData GetGiftItemData()
    {
        List<ItemData> tempList = itemData.FindAll(item => !item.IsUnlocked && (item.buyType == BuyType.BuyMoney || item.buyType == BuyType.WatchAds));
        return tempList.Count > 0?tempList[Random.Range(0, tempList.Count)]:null;
    }
}

public class ItemIdentity
{
    public string Identity => $"{type}_{numberID}";
    
    public ItemType type;
    public int numberID;
}

[Serializable]
public class ItemData : ItemIdentity
{
    public ItemType itemType;
    public BuyType buyType;
    public GameObject skinPrefab;
    public Sprite shopIcon;
    public int coinValue;

    public bool IsUnlocked
    {
        get
        {
            Data.IdItemUnlocked = Identity;
            return Data.IsItemUnlocked;
        }

        set
        {
            Data.IdItemUnlocked = Identity;
            Data.IsItemUnlocked = value;
        }
    }
}

public enum BuyType
{
    Default,
    BuyMoney,
    DailyReward,
    WatchAds,
    Event,
}

public enum ItemType
{
    PlayerSkin,
    WeaponSkin,
}