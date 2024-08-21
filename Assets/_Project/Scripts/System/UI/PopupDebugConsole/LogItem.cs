using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private Sprite logIcon;
    [SerializeField] private Sprite warningIcon;
    [SerializeField] private Sprite errorIcon;

    public LogType CurrentLogType { get; set; }

    public void Setup(LogType logType, string desc)
    {
        CurrentLogType = logType;
        switch (logType)
        {
            case LogType.Log:
                icon.sprite = logIcon;
                break;
            case LogType.Warning:
                icon.sprite = warningIcon;
                break;
            case LogType.Assert:
                icon.sprite = warningIcon;
                break;
            case LogType.Error:
                icon.sprite = errorIcon;
                break;
            case LogType.Exception:
                icon.sprite = errorIcon;
                break;
        }

        description.text = $"{desc}";
    }
}