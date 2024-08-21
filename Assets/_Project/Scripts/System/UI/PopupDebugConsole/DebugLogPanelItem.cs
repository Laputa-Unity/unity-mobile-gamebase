using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using UnityEngine.UI;

public class DebugLogPanelItem : ConsolePanelItem
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private LogItem logItemPrefab;
    [SerializeField] private Transform content;
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

    public void UpdateContent()
    {
        bool isLogInfo = btnSwitchInfo.IsOn;
        bool isLogWarning = btnSwitchWarning.IsOn;
        bool isLogError = btnSwitchError.IsOn;
        foreach (var logItem in _logItems)
        {
            switch (logItem.CurrentLogType)
            {
                case LogType.Log:
                    logItem.gameObject.SetActive(isLogInfo);
                    break;
                case LogType.Warning:
                    logItem.gameObject.SetActive(isLogWarning);
                    break;
                case LogType.Assert:
                    logItem.gameObject.SetActive(isLogWarning);
                    break;
                case LogType.Error:
                    logItem.gameObject.SetActive(isLogError);
                    break;
                case LogType.Exception:
                    logItem.gameObject.SetActive(isLogError);
                    break;
            }
        }
    }
}