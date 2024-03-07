using DG.Tweening;
using UnityEngine;

public class PopupWin : Popup
{
    [SerializeField] private BonusArrowHandler bonusArrowHandler;
    [SerializeField] private GameObject btnRewardAds;
    [SerializeField] private GameObject btnTapToContinue; 
    [SerializeField] private int totalMoney;

    private Sequence sequence;
    public int MoneyWin => ConfigController.Game.winLevelMoney;
    public void SetupMoneyWin(int bonusMoney)
    {
        totalMoney = MoneyWin + bonusMoney;
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        PopupController.Instance.Show<PopupUI>();
        Setup();
        
        sequence = DOTween.Sequence().AppendInterval(2f).AppendCallback(() => { btnTapToContinue.SetActive(true); });
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
        Data.MoneyTotal += totalMoney * bonusArrowHandler.CurrentAreaItem.MultiBonus;
        bonusArrowHandler.MoveObject.StopMoving();
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);
        sequence?.Kill();

        DOTween.Sequence().AppendInterval(2f).AppendCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }

    public void OnClickContinue()
    {
        Data.MoneyTotal += totalMoney;
        btnRewardAds.SetActive(false);
        btnTapToContinue.SetActive(false);

        DOTween.Sequence().AppendInterval(2f).AppendCallback(() => { GameManager.Instance.PlayCurrentLevel(); });
    }
}
