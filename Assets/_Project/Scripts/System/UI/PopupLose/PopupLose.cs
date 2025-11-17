public class PopupLose : Popup
{
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
