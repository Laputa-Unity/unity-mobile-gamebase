using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private GameObject counterGo;
    [SerializeField] private TextMeshProUGUI counter;

    [SerializeField] private Sprite logIcon;
    [SerializeField] private Sprite warningIcon;
    [SerializeField] private Sprite errorIcon;

    private int _collapseCounter = 1;
    public string CacheContent { get; set; }

    public LogType CurrentLogType { get; set; }

    public void Setup(LogType logType, string desc, bool isCollapse)
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
        CacheContent = $"{desc}";
        counterGo.SetActive(isCollapse);
        counter.text = $"{_collapseCounter}";
    }
    
    public void Refresh(bool isCollapse, bool isLogInfo, bool isLogWarning, bool isLogError)
    {
        counterGo.SetActive(isCollapse);
        counter.text = $"{_collapseCounter}";
        
        switch (CurrentLogType)
        {
            case LogType.Log:
                gameObject.SetActive(isLogInfo);
                break;
            case LogType.Warning:
                gameObject.SetActive(isLogWarning);
                break;
            case LogType.Assert:
                gameObject.SetActive(isLogWarning);
                break;
            case LogType.Error:
                gameObject.SetActive(isLogError);
                break;
            case LogType.Exception:
                gameObject.SetActive(isLogError);
                break;
        }
    }

    public void IncreaseCollapseCounter()
    {
        counter.text = $"{++_collapseCounter}";
    }
}