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
        if (_timeCounter <= 0 && Data.PlayerData.VibrationState)
        {
            Vibration.VibratePop();
            _timeCounter = _timeDelay;
        }
    }
    
    public void HapticMedium()
    {
        if (_timeCounter <= 0 && Data.PlayerData.VibrationState)
        {
            Vibration.Vibrate();
            _timeCounter = _timeDelay;
        }
    }
    
    public void HapticHeavy()
    {
        if (_timeCounter <= 0 && Data.PlayerData.VibrationState)
        {
            Vibration.VibratePeek();
            _timeCounter = _timeDelay;
        }
    }
}