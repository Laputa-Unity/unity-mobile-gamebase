using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ItemController : SingletonDontDestroy<ItemController>
{
    [SerializeField] private ItemConfig itemConfig;

    private void Start()
    {
        itemConfig.Initialize();
    }

    public List<ItemData> GetListItemData()
    {
        return itemConfig.itemData;
    }

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
