using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogPanelItem : ConsolePanelItem
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private LogItem logItemPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private CustomSwitchButton btnSwitchCollapse;
    [SerializeField] private CustomSwitchButton btnSwitchInfo;
    [SerializeField] private CustomSwitchButton btnSwitchWarning;
    [SerializeField] private CustomSwitchButton btnSwitchError;

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
        bool isCollapse = btnSwitchCollapse.IsOn;
        bool isLogInfo = btnSwitchInfo.IsOn;
        bool isLogWarning = btnSwitchWarning.IsOn;
        bool isLogError = btnSwitchError.IsOn;
        if (type == LogType.Log && !isLogInfo)
        {
            return;
        }

        if ((type == LogType.Warning || type == LogType.Assert) && !isLogWarning)
        {
            return;
        }
        
        if ((type == LogType.Error || type == LogType.Exception) && !isLogError)
        {
            return;
        }

        if (isCollapse)
        {
            var tempLogItem = _logItems.Find(item => item.CacheContent == condition);
            if (tempLogItem == null)
            {
                var logItem = LeanPool.Spawn(logItemPrefab, content);
                logItem.Setup(type, condition, true);
                _logItems.Add(logItem);
            }
            else
            {
                tempLogItem.IncreaseCollapseCounter();
            }
        }
        else
        {
            var logItem = LeanPool.Spawn(logItemPrefab, content);
            logItem.Setup(type, condition, false);
            _logItems.Add(logItem);
        }
        
        
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

    public void UpdateContent()
    {
        bool isCollapse = btnSwitchCollapse.IsOn;
        bool isLogInfo = btnSwitchInfo.IsOn;
        bool isLogWarning = btnSwitchWarning.IsOn;
        bool isLogError = btnSwitchError.IsOn;
        foreach (var logItem in _logItems)
        {
            logItem.Refresh(isCollapse, isLogInfo, isLogWarning, isLogError);
        }
    }
}