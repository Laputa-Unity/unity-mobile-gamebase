using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogPanelItem : ConsolePanelItem
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private LogItem logItemPrefab;
    [SerializeField] private Transform content;

    private List<LogItem> _logItems = new List<LogItem>();
    
    protected override ConsoleTabType consoleTabType => ConsoleTabType.DebugLog;
    
    void OnEnable()
    {
        // Subscribe to the log message event
        Application.logMessageReceived += HandleLog;
    }
    
    void OnDisable()
    {
        ClearContent();
        // Unsubscribe when the object is destroyed
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string condition, string stacktrace, LogType type)
    {
        var logItem = LeanPool.Spawn(logItemPrefab, content);
        logItem.Setup(type, condition);
        _logItems.Add(logItem);
        scrollRect.verticalNormalizedPosition = 0f;
    }

    private void ClearContent()
    {
        _logItems.Clear();
        var children = content.childCount;
        for (int i = children - 1; i >= 0; i--)
        {
            LeanPool.Despawn(content.GetChild(i));
        }
    }

    public void OnClickClearContent()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        ClearContent();
    }
}
