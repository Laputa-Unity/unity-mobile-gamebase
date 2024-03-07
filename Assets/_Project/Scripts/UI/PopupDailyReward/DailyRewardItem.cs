using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardItem : MonoBehaviour
{
    public int dayIndex;
    public TextMeshProUGUI textDay;
    public TextMeshProUGUI rewardValue;
    public Image greenTick;
    public Image backgroundClaim;
    public Image backgroundCanNotClaim;
    public Image iconItem;
    private int _coinValue;
    private DailyRewardItemState _dailyRewardItemState;
    private DailyRewardData _dailyRewardData;
    private PopupDailyReward _popupDailyReward;
    public DailyRewardItemState DailyRewardItemState => _dailyRewardItemState;
    public DailyRewardData DailyRewardData => _dailyRewardData;

    public void SetUp(PopupDailyReward popup, int i)
    {
        _popupDailyReward = popup;
        dayIndex = i + 1;
        SetUpData();
        SetUpUI(i);
    }

    public void SetDefaultUI()
    {
        backgroundClaim.gameObject.SetActive(false);
        backgroundCanNotClaim.gameObject.SetActive(false);
        greenTick.gameObject.SetActive(false);
    }

    private void SetUpData()
    {
        // Setup data
        _dailyRewardData = Data.IsStartLoopingDailyReward
            ? ConfigController.DailyRewardConfig.DailyRewardDataLoop[dayIndex - 1]
            : ConfigController.DailyRewardConfig.DailyRewardData[dayIndex - 1];

        _coinValue = _dailyRewardData.Value;
        // Setup states
        if (_dailyRewardData.DailyRewardType == DailyRewardType.Currency)
        {
        }
        else if (_dailyRewardData.DailyRewardType == DailyRewardType.Skin)
        {
            //shopItemData = ConfigController.ItemConfig.GetShopItemDataById(dailyRewardData.SkinID);
        }

        if (Data.DailyRewardDayIndex > dayIndex)
        {
            _dailyRewardItemState = DailyRewardItemState.Claimed;
        }
        else if (Data.DailyRewardDayIndex == dayIndex)
        {
            if (!Data.IsClaimedTodayDailyReward())
                _dailyRewardItemState = DailyRewardItemState.ReadyToClaim;
            else
                _dailyRewardItemState = DailyRewardItemState.NotClaim;
        }
        else
        {
            _dailyRewardItemState = DailyRewardItemState.NotClaim;
        }
    }

    public void SetUpUI(int i)
    {
        SetDefaultUI();
        textDay.text = $"Day {i + 1}";
        rewardValue.text = _coinValue.ToString();
        switch (_dailyRewardItemState)
        {
            case DailyRewardItemState.Claimed:
                backgroundClaim.gameObject.SetActive(false);
                backgroundCanNotClaim.gameObject.SetActive(true);
                greenTick.gameObject.SetActive(true);
                break;
            case DailyRewardItemState.ReadyToClaim:
                backgroundClaim.gameObject.SetActive(true);
                backgroundCanNotClaim.gameObject.SetActive(false);
                greenTick.gameObject.SetActive(false);
                break;
            case DailyRewardItemState.NotClaim:
                backgroundClaim.gameObject.SetActive(false);
                backgroundCanNotClaim.gameObject.SetActive(false);
                greenTick.gameObject.SetActive(false);
                break;
        }

        switch (_dailyRewardData.DailyRewardType)
        {
            case DailyRewardType.Currency:
                rewardValue.gameObject.SetActive(true);
                iconItem.sprite = _dailyRewardData.Icon;
                iconItem.SetNativeSize();
                break;
            case DailyRewardType.Skin:
                //Icon.sprite = shopItemData.Icon;
                iconItem.SetNativeSize();
                break;
        }
    }

    public void OnClaim(bool isClaimX5 = false)
    {
        switch (_dailyRewardData.DailyRewardType)
        {
            case DailyRewardType.Currency:
                MoneyHandler.Instance.SetFrom(transform.position);
                Data.MoneyTotal += _coinValue * (isClaimX5 ? 5 : 1);
                break;
            case DailyRewardType.Skin:
                //shopItemData.IsUnlocked = true;
                //Data.CurrentEquippedSkin = shopItemData.Id;
                break;
        }
    }
}

public enum DailyRewardItemState
{
    Claimed,
    ReadyToClaim,
    NotClaim
}