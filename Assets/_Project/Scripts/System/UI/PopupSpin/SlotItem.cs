using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SlotItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI rewardText;

    private PopupSpin _popupSpin;
    private SlotItemData _slotItemData;

    public void Setup(PopupSpin popupSpin, SlotItemData data)
    {
        _popupSpin ??= popupSpin;
        _slotItemData ??= data;
        
        rewardText.gameObject.SetActive(true);
        rewardText.text = _slotItemData.amount.ToString();
        icon.sprite = _slotItemData.sprite;
        icon.SetNativeSize();
        
    }

    public void ClaimReward()
    {
        if (_slotItemData.type == SlotItemDataType.Money)
        {
            Data.PlayerData.CurrentMoney += _slotItemData.amount;
        }
    }

    public bool IsTarget(int weight)
    {
        return weight >= _slotItemData.minWeight && weight <= _slotItemData.maxWeight;
    }
}
