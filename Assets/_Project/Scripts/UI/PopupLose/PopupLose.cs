

public class PopupLose : Popup
{
    protected override void BeforeShow()
    {
        base.BeforeShow();
        SoundController.Instance.PlayFX(SoundType.Lose);
        PopupController.Instance.Show<PopupUI>();
    }

    protected override void BeforeHide()
    {
        base.BeforeHide();
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickReplay()
    {
        FirebaseManager.OnClickButtonReplay();
        
        GameManager.Instance.ReplayGame();
    }
}
