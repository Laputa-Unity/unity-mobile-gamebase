using CustomTween;
using UnityEngine;

public class PopupWin : Popup
{
    [SerializeField] private BonusArrowHandler bonusArrowHandler;
    [SerializeField] private GameObject btnRewardAds;
    [SerializeField] private GameObject btnTapToContinue; 
    private int _totalMoney;

    private Sequence _sequence;
    //public int MoneyWin => ConfigController.Level.winLevelMoney;
    public void SetupMoneyWin(int bonusMoney)
    {
        _totalMoney = 100 + bonusMoney;
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        PopupController.Instance.Show<PopupUI>();
        Setup();
        
        _sequence = Sequence.Create().ChainDelay(2f).ChainCallback(() => { btnTapToContinue.SetActive(true); });
    }

    public void Setup()
    {
        btnRewardAds.SetActive(true);
        btnTapToContinue.SetActive(false);
        bonusArrowHandler.MoveObject.ResumeMoving();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickAdsReward()
    {
        GetRewardAds();
    }
    
    public void GetRewardAds()
    {
        Data.PlayerData.CurrentMoney += _totalMoney * bonusArrowHandler.currentAreaItem.MultiBonus;
        bonusArrowHandler.MoveObject.StopMoving();
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);
        _sequence.Stop();

        Sequence.Create().ChainDelay(2f).ChainCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }

    public void OnClickContinue()
    {
        Data.PlayerData.CurrentMoney += _totalMoney;
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);

        Sequence.Create().ChainDelay(2f).ChainCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }
}
