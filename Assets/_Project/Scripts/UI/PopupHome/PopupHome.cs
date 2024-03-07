public class PopupHome : Popup
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

    public void OnClickStart()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        GameManager.Instance.StartGame();
    }

    public void OnClickDebug()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        PopupController.Instance.Show<PopupDebug>();
    }

    public void OnClickSetting()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        PopupController.Instance.Show<PopupSetting>();
    }

    public void OnClickDailyReward()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        PopupController.Instance.Show<PopupDailyReward>();
    }

    public void OnClickShop()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        PopupController.Instance.Show<PopupShop>();
    }
}
