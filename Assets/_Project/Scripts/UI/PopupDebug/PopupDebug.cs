using UnityEngine.UI;

public class PopupDebug : Popup
{
    public Text SetCoin;
    public Text SetLevel;
    public Toggle ToggleTesting;

    public void OnEnable()
    {
        ToggleTesting.isOn = Data.IsTesting;
    }

    public void OnClickAccept()
    {
        if (SetCoin.text != null && SetCoin.text != "")
        {
            Data.CurrencyTotal = int.Parse(SetCoin.text);
        }
        if (SetLevel.text != null && SetLevel.text != "")
        {
            Data.CurrentLevel = int.Parse(SetLevel.text);
            GameManager.Instance.PrepareLevel();
            GameManager.Instance.StartGame();
        }

        SetCoin.text = string.Empty;
        SetLevel.text = string.Empty;
        gameObject.SetActive(false);
    }

    public void ChangeTestingState()
    {
        Data.IsTesting = ToggleTesting.isOn;
    }
}