using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupShop : Popup
{
    [SerializeField] private ShopItem shopItemPrefab;
    [SerializeField] private Transform content;

    private List<ItemData> _listItemData;
    private readonly List<ShopItem> _listShopItem = new List<ShopItem>();
    void Start()
    {
        Observer.BuySkin += Refresh;
        Observer.SelectSkin += Refresh;
    }

    void OnDestroy()
    {
        Observer.BuySkin -= Refresh;
        Observer.SelectSkin -= Refresh;

    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        Setup();
    }

    public void Setup()
    {
        _listItemData ??= ItemController.Instance.GetListItemData();
        _listShopItem.Clear();
        content.Clear();
        
        foreach (var itemData in _listItemData)
        {
            ShopItem newShopItem = Instantiate(shopItemPrefab, content.transform);
            newShopItem.Setup(itemData);
            _listShopItem.Add(newShopItem);
        }
    }

    private void Refresh()
    {
        foreach (var shopItem in _listShopItem)
        {
            shopItem.Refresh();
        }
    }
    
    public void OnClickBack()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        Hide();
    }
}
