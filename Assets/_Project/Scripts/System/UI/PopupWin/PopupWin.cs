using CustomTween;
using UnityEngine;

public class PopupWin : Popup
{
    private int _totalMoney = 100;
    private Sequence _sequence;

    protected override void BeforeShow()
    {
        base.BeforeShow();
        Setup();
    }

    public void Setup()
    {
        
    }

    public void OnClickContinue()
    {
        Data.PlayerData.CurrentGold += _totalMoney;
        Tween.Delay( 2f,() =>
        {
            GameManager.Instance.PlayCurrentLevel();
        });
    }
}
