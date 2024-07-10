using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PopupSpin : Popup
{
    [SerializeField] private Spin spin;
    [SerializeField] private TextMeshProUGUI textTimeLeft;
    [SerializeField] private CustomButton btnSpin;
    [SerializeField] private GameObject btnFreeSpinAds;

    private bool _isSpinning;
    private List<CustomButton>_listUiButton;
    private void Start()
    {
        _listUiButton = GetComponentsInChildren<CustomButton>().ToList();
        SetupSlotItems();
    }

    void Update()
    {
        if (_isSpinning)
        {
            btnSpin.Interactable = false;
            btnFreeSpinAds.SetActive(!_isSpinning);
            textTimeLeft.gameObject.SetActive(!_isSpinning);
        }
        else
        {
            if (DateTime.TryParse(Data.PlayerData.LastSpin, out var lastTime))
            {
                bool isEnableSpin = DateTime.Now > lastTime.AddHours(12);
                btnSpin.Interactable = isEnableSpin;
                btnFreeSpinAds.SetActive(!isEnableSpin);
                textTimeLeft.gameObject.SetActive(!isEnableSpin);
                if (!isEnableSpin)
                {
                    var timeLeft = lastTime.AddHours(12) - DateTime.Now;
                    textTimeLeft.text = $"Time left: {timeLeft:hh\\:mm\\:ss}";
                }
            }
            else
            {
                Data.PlayerData.LastSpin = DateTime.Now.AddDays(-1).Date.ToString();
            }
        }
    }

    private void SetupSlotItems()
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
        _isSpinning = true;
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        Spin();
    }

    public void OnClickFreeSpinAds()
    {
        Data.PlayerData.LastSpin = DateTime.Now.AddDays(-1).Date.ToString();
    }

    private void Spin()
    {
        SetStateButtons(false);
        spin.OnSpin();
        Data.PlayerData.LastSpin = DateTime.Now.ToString();
    }
    
    public void OnStopWheel()
    {
        SetStateButtons(true);
        _isSpinning = false;
        spin.OnStopWheel();
    }

    private void SetStateButtons(bool isInteractable)
    {
        foreach (var uiButton in _listUiButton)
        {
            uiButton.Interactable = isInteractable;
        }
    }
}
