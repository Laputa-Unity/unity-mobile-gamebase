using System;
using UnityEngine;

public class VibrationController : SingletonDontDestroy<VibrationController>
{
    [SerializeField] private VibrationConfig vibrationConfig;
    
    private float _timeCounter;
    private float _timeDelay;
    protected override void Awake()
    {
        base.Awake();
        Vibration.Init();

        _timeDelay = vibrationConfig.timeDelay;
    }

    private void Update()
    {
        _timeCounter -= _timeDelay;
    }

    public void HapticLight()
    {
        if (_timeCounter <= 0 && Data.VibrateState)
        {
            Vibration.Vibrate();
            _timeCounter = _timeDelay;
        }
    }
    
    public void HapticMedium()
    {
        if (_timeCounter <= 0 && Data.VibrateState)
        {
            Vibration.VibratePop();
            _timeCounter = _timeDelay;
        }
    }
    
    public void HapticHeavy()
    {
        if (_timeCounter <= 0 && Data.VibrateState)
        {
            Vibration.VibratePeek();
            _timeCounter = _timeDelay;
        }
    }
}