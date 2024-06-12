using UnityEngine;

public class PopupSpin : Popup
{
    [SerializeField] private Spin spin;

    private void Start()
    {
        Setup();
    }

    private void Setup()
    {
        spin.Setup(this, SpinController.Instance.GetSlotItemData());
    }

    public void OnClickBack()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        Hide();
    }
    
    public void OnClickSpin()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        Spin();
    }

    private void Spin()
    {
        spin.OnSpin();
    }

    public void OnStopWheel()
    {
        spin.OnStopWheel();
    }

}
