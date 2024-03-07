using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Switcher : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SwitchState switchState = SwitchState.Idle;
    [SerializeField] private bool isOn;
    [FormerlySerializedAs("SettingType")]
    [Header("Components")]
    [SerializeField] private SettingType settingType;
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;
    [SerializeField] private Image @switch;
    [SerializeField] private Transform offPos;
    [SerializeField] private Transform pos;
    [SerializeField] private TextMeshProUGUI switchText;
    [Header("Config attribute")] 
    [Range(0.1f,3f)] public float timeSwitching = .5f;

    private void SetupData()
    {
        switch (settingType)
        {
            case SettingType.BackgroundSound:
                isOn = Data.BgSoundState;
                break;
            case SettingType.FxSound:
                isOn = Data.FxSoundState;
                break;
            case SettingType.Vibration:
                isOn = Data.VibrateState;
                break;
        }
    }
    
    private void SetupUI()
    {
        if (switchText) switchText.text = isOn ? "On" : "Off";
        @switch.sprite = isOn ? @on : off;
    }

    private void Setup()
    {
        SetupData();
        SetupUI();
    }
    
    private void OnEnable()
    {
        @switch.transform.position = isOn ? pos.position : offPos.position;
        Setup();
    }

    public void Switching()
    {
        if (switchState == SwitchState.Moving) return;
        switchState = SwitchState.Moving;
        if (isOn)
        {
            @switch.transform.DOMove(offPos.position, timeSwitching);
        }
        else
        {
            @switch.transform.DOMove(pos.position, timeSwitching);
        }
        DOTween.Sequence().AppendInterval(timeSwitching / 2f).SetEase(Ease.Linear).AppendCallback(() =>
        {
            switch (settingType)
            {
                case SettingType.BackgroundSound:
                    Data.BgSoundState = !isOn;
                    break;
                case SettingType.FxSound:
                    Data.FxSoundState = !isOn;
                    break;
                case SettingType.Vibration:
                    Data.VibrateState = !isOn;
                    break;
            }
            Setup();
        }).OnComplete(() =>
        {
            switchState = SwitchState.Idle;
        });
    }
}

public enum SettingType
{
    BackgroundSound,
    FxSound,
    Vibration,
}

public enum SwitchState
{
    Idle,
    Moving,
}