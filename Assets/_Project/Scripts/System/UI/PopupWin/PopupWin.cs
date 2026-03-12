using CustomTween;
using UnityEngine;

public class PopupWin : Popup
{
    [SerializeField] private CustomButton btnContinue;
    
    private int _totalMoney = 100;
    private Sequence _sequence;

    protected override void BeforeShow()
    {
        base.BeforeShow();
        Setup();
    }

    public void Setup()
    {
        btnContinue.gameObject.SetActive(true);
    }

    public void OnClickContinue()
    {
        Data.PlayerData.CurrentGold += _totalMoney;
        btnContinue.gameObject.SetActive(false);
        Tween.Delay( 2f,() =>
        {
            GameManager.Instance.PlayCurrentLevel();
        });
    }
}
