using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : Popup
{
    [SerializeField] private Image icon;
    [SerializeField] private GameObject price;
    [SerializeField] private GameObject owned;
    [SerializeField] private GameObject selected;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject disableBuy;
    [SerializeField] private GameObject btnBuy;

    private ItemData _itemData;
    private PlayerData playerData => Data.PlayerData;
    public void Setup(ItemData itemData)
    {
        _itemData = itemData;

        Refresh();
    }

    public void Refresh()
    {
        var isSelected = playerData.CurrentSkin == _itemData.identity;
        var enableToBuy = playerData.CurrentMoney >= _itemData.price;

        icon.sprite = _itemData.shopIcon;
        if (playerData.OwnedSkins.Contains(_itemData.identity))
        {
            price.SetActive(false);
            btnBuy.SetActive(false);
            disableBuy.SetActive(false);
            owned.SetActive(!isSelected);
            selected.SetActive(isSelected);
        }
        else
        {
            owned.SetActive(false);
            selected.SetActive(false);
            priceText.text = _itemData.price.ToString();
            disableBuy.SetActive(!enableToBuy);
            btnBuy.SetActive(enableToBuy);
        }
    }

    public void OnClickSelect()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        if (playerData.OwnedSkins.Contains(_itemData.identity))
        {
            playerData.CurrentSkin = _itemData.identity;
            Observer.SelectSkin?.Invoke();
        }
    }

    public void OnClickBuy()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        SoundController.Instance.PlayFX(SoundName.PurchaseCompleted);
        playerData.CurrentMoney -= _itemData.price;
        playerData.OwnedSkins.Add(_itemData.identity);
        playerData.CurrentSkin = _itemData.identity;
        Observer.BuySkin?.Invoke();
    }
}
