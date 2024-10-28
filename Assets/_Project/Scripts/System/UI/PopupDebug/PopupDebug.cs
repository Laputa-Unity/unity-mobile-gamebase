using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TMPro;
using UnityEngine;

public class PopupDebug : Popup
{
    [SerializeField] private TMP_InputField setLevel;
    [SerializeField] private TMP_InputField setCoin;
    [SerializeField] private GameObject panelPlayerData;
    [SerializeField] private TMP_InputField inputField;

    protected override void BeforeShow()
    {
        base.BeforeShow();
        panelPlayerData.SetActive(false);
        PopupController.Instance.Hide<PopupUI>();
    }

    public void OnClickAccept()
    {
        if (!string.IsNullOrEmpty(setLevel.text))
        {
            Data.PlayerData.CurrentLevelIndex = int.Parse(setLevel.text);
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