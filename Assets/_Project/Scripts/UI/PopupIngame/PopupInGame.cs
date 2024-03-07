using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

public class PopupInGame : Popup
{
   public TextMeshProUGUI levelText;
   public TextMeshProUGUI levelTypeText;

   private List<UIEffect> UIEffects => GetComponentsInChildren<UIEffect>().ToList();
   
   private const string InGameOnClickHome = "InGameOnClickHome";
   private const string InGameOnClickReplay = "InGameOnClickReplay";

   public void Start()
   {
      Observer.WinLevel += HideUI;
      Observer.LoseLevel += HideUI;
   }

   public void OnDestroy()
   {
      Observer.WinLevel -= HideUI;
      Observer.LoseLevel -= HideUI;
   }

   protected override void BeforeShow()
   {
      base.BeforeShow();

      Setup();
   }
   
   private void Setup()
   {
      levelText.text = $"Level {Data.CurrentLevel}";
      levelTypeText.text = $"Level {(Data.UseLevelABTesting == 0 ? "A" : "B")}";
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

   public void OnClickLevelA()
   {
      Data.UseLevelABTesting = 0;
      GameManager.Instance.ReplayGame();
   }

   public void OnClickLevelB()
   {
      Data.UseLevelABTesting = 1;
      GameManager.Instance.ReplayGame();
   }

   public void OnClickLose()
   {
      GameManager.Instance.OnLoseGame(1f);
   }

   public void OnClickWin()
   {
      GameManager.Instance.OnWinGame(1f);
   }

   private void HideUI(Level level = null)
   {
      foreach (UIEffect item in UIEffects)
      {
         item.PlayAnim();
      }
   }
}
