using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDebug : Popup
{
    [SerializeField] private TMP_InputField setLevel;
    [SerializeField] private TMP_InputField setCoin;
    [SerializeField] private Toggle toggleTesting;

    protected override void BeforeShow()
    {
        base.BeforeShow();
        PopupController.Instance.Hide<PopupUI>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        toggleTesting.isOn = Data.PlayerData.IsTesting;
    }

    public void OnClickAccept()
    {
        if (!string.IsNullOrEmpty(setLevel.text))
        {
            Data.PlayerData.CurrentLevelIndex = int.Parse(setLevel.text) - 1;
            GameManager.Instance.PrepareLevel();
            GameManager.Instance.StartGame();
        }
        if (!string.IsNullOrEmpty(setCoin.text))
        {
            Data.PlayerData.CurrentMoney = int.Parse(setCoin.text);
        }

        setCoin.text = string.Empty;
        setLevel.text = string.Empty;
        gameObject.SetActive(false);
    }

    public void ChangeTestingState()
    {
        Data.PlayerData.IsTesting = toggleTesting.isOn;
    }
}