using System;
using System.Collections.Generic;
using CustomInspector;
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
                if (!Data.PlayerData.IsOwnedSkin(item.identity))
                {
                    Data.PlayerData.OwnedSkins.Add(item.identity);
                }
            }
        }
    }

    public void UnlockAllSkins()
    {
        foreach (var item in itemData)
        {
            if (!Data.PlayerData.OwnedSkins.Contains(item.identity))
            {
                Data.PlayerData.OwnedSkins.Add(item.identity);
            }
        }
    }

    public ItemData GetItemData(string itemIdentity)
    {
        return itemData.Find(item => item.identity == itemIdentity);
    }

    public List<ItemData> GetListItemDataByType(ItemType itemType)
    {
        return itemData.FindAll(item => item.itemType == itemType);
    }

    public ItemData GetGiftItemData()
    {
        List<ItemData> tempList = itemData.FindAll(item => Data.PlayerData.IsOwnedSkin(item.identity) && (item.buyType == BuyType.Money || item.buyType == BuyType.WatchAds));
        return tempList.Count > 0 ? tempList[Random.Range(0, tempList.Count)] : null;
    }
}

[Serializable]
public class ItemData
{
    public string identity;
    public ItemType itemType;
    public BuyType buyType;
    public GameObject skinPrefab;
    public Sprite shopIcon;
    [ShowIf("buyType", BuyType.Money)] public int price;
}

public enum BuyType
{
    Default,
    Money,
    DailyReward,
    WatchAds,
    Event,
}

public enum ItemType
{
    PlayerSkin,
    WeaponSkin,
}