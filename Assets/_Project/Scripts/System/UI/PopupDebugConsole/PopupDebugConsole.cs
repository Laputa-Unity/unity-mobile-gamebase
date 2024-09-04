using System.Collections.Generic;
using CustomTween;
using UnityEngine;

public class PopupDebugConsole : Popup
{
    [SerializeField] private GameObject btnShow;
    [SerializeField] private GameObject frameAnchor;
    [SerializeField] private GameObject frame;
    [SerializeField] private List<ConsoleTabItem> tabs;
    [SerializeField] private List<ConsolePanelItem> panels;
    private ConsoleTabType _currentTab = ConsoleTabType.DebugLog;
    private bool _isActiveFrame = true;
    private Vector3 _saveFrameScale = Vector3.one;
    
    protected override void OnEnable()
    {
        base.OnEnable();
        Observer.SelectConsoleTab += Setup;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Observer.SelectConsoleTab -= Setup;
    }

    protected override void BeforeShow()
    {
        base.BeforeShow();
        Setup(_currentTab);
    }

    private void Setup(ConsoleTabType tabType)
    {
        btnShow.SetActive(!_isActiveFrame);
        frame.SetActive(_isActiveFrame);
        
        _currentTab = tabType;
        foreach (var tab in tabs)
        {
            tab.Setup(_currentTab);
        }
        foreach (var panel in panels)
        {
            panel.Setup(_currentTab);
        }
    }
    
   public void OnClickExit()
   {
       SoundController.Instance.PlayFX(SoundName.ClickButton);
       Hide();
   }


   public void OnClickShowFrame()
   {
       SoundController.Instance.PlayFX(SoundName.ClickButton);
       ShowFrame();
   }

   public void OnClickHideFrame()
   {
       SoundController.Instance.PlayFX(SoundName.ClickButton);
       HideFrame();
   }

   private void ShowFrame()
   {
       frameAnchor.transform.position = btnShow.transform.position;
       btnShow.SetActive(false);
       frame.SetActive(true);
       frame.transform.localScale = Vector3.zero;
       Tween.Scale(frame.transform, _saveFrameScale, .2f).OnComplete(() =>
       {
           _isActiveFrame = true;
       });
   }
   
   private void HideFrame()
   {
       _saveFrameScale = frame.transform.localScale;
       Tween.Scale(frame.transform, Vector3.zero, .2f).OnComplete(()=>
       {
           btnShow.transform.position = frameAnchor.transform.position;
           frame.SetActive(false);
           btnShow.SetActive(true);
           _isActiveFrame = false;
       });
   }
}

public enum ConsoleTabType
{
    Command,
    Information,
    Profiler,
    DebugLog,
}