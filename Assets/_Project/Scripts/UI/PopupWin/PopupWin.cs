using CustomTween;
using UnityEngine;

public class PopupWin : Popup
{
    [SerializeField] private BonusArrowHandler bonusArrowHandler;
    [SerializeField] private GameObject btnRewardAds;
    [SerializeField] private GameObject btnTapToContinue; 
    [SerializeField] private int totalMoney;

    private Sequence sequence;
    //public int MoneyWin => ConfigController.Level.winLevelMoney;
    public void SetupMoneyWin(int bonusMoney)
    {
        totalMoney = 100 + bonusMoney;
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        PopupController.Instance.Show<PopupUI>();
        Setup();
        
        sequence = Sequence.Create().ChainDelay(2f).ChainCallback(() => { btnTapToContinue.SetActive(true); });
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
        Data.CurrentMoney += totalMoney * bonusArrowHandler.CurrentAreaItem.MultiBonus;
        bonusArrowHandler.MoveObject.StopMoving();
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);
        sequence.Stop();

        Sequence.Create().ChainDelay(2f).ChainCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }

    public void OnClickContinue()
    {
        Data.CurrentMoney += totalMoney;
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);

        Sequence.Create().ChainDelay(2f).ChainCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }
}
