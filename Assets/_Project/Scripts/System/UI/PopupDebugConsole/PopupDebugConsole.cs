using System.Collections.Generic;
using UnityEngine;

public class PopupDebugConsole : Popup
{
    [SerializeField] private List<ConsoleTabItem> tabs;
    [SerializeField] private List<ConsolePanelItem> panels;
    private ConsoleTabType _currentTab = ConsoleTabType.Command;

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
}

public enum ConsoleTabType
{
    Command,
    Information,
    Profiler,
    Settings,
}