using CustomTween;
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
    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;
    [SerializeField] private Image switchImage;
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
                isOn = Data.PlayerData.MusicVolume > 0;
                break;
            case SettingType.FxSound:
                isOn = Data.PlayerData.SoundVolume > 0;
                break;
            case SettingType.Vibration:
                isOn = Data.PlayerData.VibrationState;
                break;
        }
    }
    
    private void SetupUI()
    {
        if (switchText) switchText.text = isOn ? "On" : "Off";
    }

    private void Setup()
    {
        SetupData();
        SetupUI();
    }
    
    private void OnEnable()
    {
        switchImage.transform.position = isOn ? pos.position : offPos.position;
        switchImage.color = isOn ? onColor : offColor;
        Setup();
    }

    public void Switching()
    {
        if (switchState == SwitchState.Moving) return;
        switchState = SwitchState.Moving;
        
        switch (settingType)
        {
            case SettingType.BackgroundSound:
                Data.PlayerData.MusicVolume = !isOn ? 0 : 1;
                break;
            case SettingType.FxSound:
                Data.PlayerData.SoundVolume = !isOn ? 0 : 1;
                break;
            case SettingType.Vibration:
                Data.PlayerData.VibrationState = !isOn;
                break;
        }
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        
        if (isOn)
        {
            Tween.Position(switchImage.transform, offPos.position, timeSwitching);
            Tween.Color(switchImage, offColor, timeSwitching);
        }
        else
        {
            Tween.Position(switchImage.transform, pos.position, timeSwitching);
            Tween.Color(switchImage, onColor, timeSwitching);
        }
        Sequence.Create().ChainDelay(timeSwitching / 2f).ChainCallback(Setup).OnComplete(() =>
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