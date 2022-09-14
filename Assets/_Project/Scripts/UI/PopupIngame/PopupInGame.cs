using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PopupInGame : Popup
{
   [Header("Components")]
   public TextMeshProUGUI LevelText;
   public TextMeshProUGUI LevelTypeText;
   [Header("Debug UI")]
   public List<GameObject> DebugGameObjects;

   protected override void BeforeShow()
   {
      base.BeforeShow();
      
      Setup();
   }

   public void Setup()
   {
      LevelText.text = $"Level {Data.CurrentLevel}";
      if (Data.IsTesting)
      {
         DebugGameObjects.ForEach(item => item.gameObject.SetActive(true));
      }
      else
      {
         DebugGameObjects.ForEach(item=> item.gameObject.SetActive(false));
      }
   }

   public void OnClickHome()
   {
      GameManager.Instance.ReturnHome();
   }

   public void OnClickReplay()
   {
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
      GameManager.Instance.OnLoseGame(0f);
   }

   public void OnClickWin()
   {
      GameManager.Instance.OnWinGame(0f);
   }
}
