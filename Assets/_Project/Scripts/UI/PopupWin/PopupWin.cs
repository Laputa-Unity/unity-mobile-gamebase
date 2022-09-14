

public class PopupWin : Popup
{
    protected override void BeforeShow()
    {
        base.BeforeShow();
        SoundController.Instance.PlayFX(SoundType.PopupWinShow);
        PopupController.Instance.Show<PopupUI>();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickContinue()
    {
        GameManager.Instance.PrepareLevel();
        GameManager.Instance.StartGame();
    }
}
