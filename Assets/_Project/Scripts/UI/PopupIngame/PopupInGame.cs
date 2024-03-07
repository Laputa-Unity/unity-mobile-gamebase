using TMPro;

public class PopupInGame : Popup
{
   public TextMeshProUGUI levelText;

   protected override void BeforeShow()
   {
      base.BeforeShow();

      Setup();
   }
   
   private void Setup()
   {
      levelText.text = $"Level {Data.CurrentLevel}";
   }

   public void OnClickHome()
   {
      SoundController.Instance.PlayFX(SoundName.ClickButton);
      GameManager.Instance.ReturnHome();
   }

   public void OnClickReplay()
   {
      SoundController.Instance.PlayFX(SoundName.ClickButton);
      GameManager.Instance.ReplayGame();
   }

   public void OnClickPrevious()
   {
      GameManager.Instance.BackLevel();
   }

   public void OnClickSkip()
   {
      GameManager.Instance.NextLevel();
   }

   public void OnClickLose()
   {
      GameManager.Instance.OnLoseGame(1f);
   }

   public void OnClickWin()
   {
      GameManager.Instance.OnWinGame(1f);
   }
}
