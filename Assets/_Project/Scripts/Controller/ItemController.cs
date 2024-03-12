using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : SingletonDontDestroy<ItemController>
{
    [SerializeField] private ItemConfig itemConfig;

    public ItemData GetItemData(string itemIdentity)
    {
        return itemConfig.GetItemData(itemIdentity);
    }

    public ItemData GetRandomItemData()
    {
        return itemConfig.itemData[Random.Range(0, itemConfig.itemData.Count)];
    }

    public ItemData GetCurrentPlayerSkin()
    {
        return itemConfig.itemData.Find(item => item.itemType == ItemType.PlayerSkin);
    }
}
