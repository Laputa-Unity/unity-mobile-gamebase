using CustomInspector;
using TMPro;
using UnityEngine;

public class ProfilerPanelItem : ConsolePanelItem
{
    [SerializeField] private TextMeshProUGUI deviceTemperatureText;
    [SerializeField] private TextMeshProUGUI targetFrameRateText;
    [SerializeField] private TextMeshProUGUI fpsCounterText;
    [SerializeField] private FpsColumnProfiler fpsColumnProfiler;

    [SerializeField] [ReadOnly] private bool _isStop;
    protected override ConsoleTabType consoleTabType => ConsoleTabType.Profiler;

    private void Start()
    {
        targetFrameRateText.text = $"{Application.targetFrameRate}";
    }

    private void Update()
    {
        if (!_isStop)
        {
            fpsCounterText.text = ((int)(1f / Time.unscaledDeltaTime)).ToString();
            fpsColumnProfiler.TickUpdate();
        }
        deviceTemperatureText.text = $"~{GetBatteryTemperature()}°C";
    }
    
    public float GetBatteryTemperature()
    {
        float temperature = 0.0f;

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            using (AndroidJavaObject batteryManager = new AndroidJavaObject("android.os.BatteryManager"))
            {
                using (AndroidJavaObject unityActivity = new AndroidJavaObject("com.unity3d.player.UnityPlayer"))
                {
                    AndroidJavaObject currentActivity = unityActivity.GetStatic<AndroidJavaObject>("currentActivity");
                    using (AndroidJavaObject intent = currentActivity.Call<AndroidJavaObject>("registerReceiver", null, new AndroidJavaObject("android.content.IntentFilter", "android.intent.action.BATTERY_CHANGED")))
                    {
                        temperature = intent.Call<int>("getIntExtra", "temperature", 0) / 10.0f; // Convert to Celsius
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to get battery temperature: " + e.Message);
        }
#endif

        return temperature;
    }

    public void OnClickButton()
    {
        _isStop = !_isStop;
    }
}
