using TMPro;
using UnityEngine;

public class DebugLogPanelItem : ConsolePanelItem
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextMeshProUGUI text2;
    protected override ConsoleTabType consoleTabType => ConsoleTabType.DebugLog;
    
    void Awake()
    {
        // Subscribe to the log message event
        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string condition, string stacktrace, LogType type)
    {
        text.text = condition;
        text2.text = condition;
        
    }

    void OnDestroy()
    {
        // Unsubscribe when the object is destroyed
        Application.logMessageReceived -= HandleLog;
    }

}
