using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDebug : Popup
{
    [SerializeField] private TMP_InputField setLevel;
    [SerializeField] private TMP_InputField setCoin;
    [SerializeField] private Toggle toggleTesting;

    protected override void OnEnable()
    {
        base.OnEnable();
        toggleTesting.isOn = Data.IsTesting;
    }

    public void OnClickAccept()
    {
        if (!string.IsNullOrEmpty(setLevel.text))
        {
            Data.CurrentLevel = int.Parse(setLevel.text);
            GameManager.Instance.PrepareLevel();
            GameManager.Instance.StartGame();
        }
        if (!string.IsNullOrEmpty(setCoin.text))
        {
            Data.CurrentMoney = int.Parse(setCoin.text);
        }

        setCoin.text = string.Empty;
        setLevel.text = string.Empty;
        gameObject.SetActive(false);
    }

    public void ChangeTestingState()
    {
        Data.IsTesting = toggleTesting.isOn;
    }
}