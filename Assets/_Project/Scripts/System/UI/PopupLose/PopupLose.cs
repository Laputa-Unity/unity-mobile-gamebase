public class PopupLose : Popup
{
    protected override void BeforeShow()
    {
        base.BeforeShow();
        PopupController.Instance.Show<PopupUI>();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickSkip()
    {
        Data.PlayerData.CurrentLevelIndex++;
        GameManager.Instance.PlayCurrentLevel();
    }

    public void OnClickReplay()
    {
        GameManager.Instance.ReplayGame();
    }
}
