using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupDebug : Popup
{
    [SerializeField] private TMP_InputField setLevel;
    [SerializeField] private TMP_InputField setCoin;
    [SerializeField] private Toggle toggleTesting;
    [SerializeField] private GameObject panelPlayerData;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private PopupDebugConsole popupDebugConsolePrefab;

    public static PopupDebugConsole PopupDebugConsole;

    protected override void BeforeShow()
    {
        base.BeforeShow();
        panelPlayerData.SetActive(false);
        PopupController.Instance.Hide<PopupUI>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        toggleTesting.isOn = GameController.IsTesting;
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
        GameController.IsTesting = toggleTesting.isOn;
    }

    public void OnClickPanelPlayerData()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        panelPlayerData.SetActive(true);
        var settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        inputField.text = JsonConvert.SerializeObject(Data.PlayerData, settings);
    }

    public void OnClickExitPanelPlayerData()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        panelPlayerData.SetActive(false);
    }

    public void OnClickUpdate()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        try
        {
            Data.UpdateData(inputField.text);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            throw;
        }
    }
}