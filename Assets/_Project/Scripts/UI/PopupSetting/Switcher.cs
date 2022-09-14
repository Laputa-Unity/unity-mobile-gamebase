using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Switcher : MonoBehaviour
{
    [Header("Datas")]
    public SwitchState SwitchState = SwitchState.Idle;
    public bool IsOn;
    [Header("Components")]
    public SoundSettingType SoundSettingType;
    public Sprite On;
    public Sprite Off;
    public Image Switch;
    public Transform OffPos;
    public Transform OnPos;
    public TextMeshProUGUI SwitchText;
    [Header("Config attribute")] 
    [Range(0.1f,2f)]public float TimeSwitching = .5f;

    private void SetupData()
    {
        switch (SoundSettingType)
        {
            case SoundSettingType.Music:
                IsOn = Data.MusicState;
                break;
            case SoundSettingType.SoundFX:
                IsOn = Data.SoundState;
                break;
            case SoundSettingType.Vibration:
                IsOn = Data.VibrateState;
                break;
        }
    }
    
    private void SetupUI()
    {
        SwitchText.text = IsOn ? "On" : "Off";
        if (IsOn)
        {
            Switch.sprite = On;
        }
        else
        {
            Switch.sprite = Off;
        }
    }

    private void Setup()
    {
        SetupData();
        SetupUI();
    }
    
    private void OnEnable()
    {
        Switch.transform.position = IsOn ? OnPos.position : OffPos.position;
        Setup();
    }

    public void Switching()
    {
        if (SwitchState == SwitchState.Moving) return;
        SwitchState = SwitchState.Moving;
        if (IsOn)
        {
            Switch.transform.DOMove(OffPos.position, TimeSwitching);
        }
        else
        {
            Switch.transform.DOMove(OnPos.position, TimeSwitching);
        }
        DOTween.Sequence().AppendInterval(TimeSwitching / 2f).SetEase(Ease.Linear).AppendCallback(() =>
        {
            switch (SoundSettingType)
            {
                case SoundSettingType.Music:
                    Data.MusicState = !IsOn;
                    //SoundManager.Instance.StopBackgroundSound();
                    break;
                case SoundSettingType.SoundFX:
                    Data.SoundState = !IsOn;
                    break;
                case SoundSettingType.Vibration:
                    Data.VibrateState = !IsOn;
                    break;
            }
            Setup();
        }).OnComplete(() =>
        {
            SwitchState = SwitchState.Idle;
        });
    }
}

public enum SoundSettingType
{
    Music,
    SoundFX,
    Vibration,
}

public enum SwitchState
{
    Idle,
    Moving,
}