using UnityEngine;

public class VibrationController : SingletonDontDestroy<VibrationController>
{
    public float timeDelay = .2f;

    private float _timeCounter;
    protected override void Awake()
    {
        base.Awake();
        Vibration.Init();

        _timeCounter = timeDelay;
    }

    private void Update()
    {
        _timeCounter -= Time.deltaTime;
    }

    public void HapticLight()
    {
        if (_timeCounter <= 0)
        {
            Vibration.Vibrate();
            _timeCounter = timeDelay;
        }
    }
    
    public void HapticMedium()
    {
        if (_timeCounter <= 0)
        {
            Vibration.VibratePop();
            _timeCounter = timeDelay;
        }
    }
    
    public void HapticHeavy()
    {
        if (_timeCounter <= 0)
        {
            Vibration.VibratePeek();
            _timeCounter = timeDelay;
        }
    }
}