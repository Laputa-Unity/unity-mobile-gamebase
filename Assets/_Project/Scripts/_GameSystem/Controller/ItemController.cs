using UnityEngine;

public class ItemController : SingletonDontDestroy<ItemController>
{
    [SerializeField] private ItemConfig itemConfig;
    void Start()
    {
        itemConfig.Initialize();
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
