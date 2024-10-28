using System;
using TMPro;
using UnityEngine;

public class CommandPanelItem : ConsolePanelItem
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Material validInputPreset;
    [SerializeField] private Material invalidInputPreset;

    protected override ConsoleTabType consoleTabType => ConsoleTabType.Command;

    public void OnClickRun()
    {
        var text = inputField.text;
        text = text.Replace(" ", "");
        string letters = string.Empty;
        string numbers = string.Empty;
        bool isFloat = false;

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                letters += c;
            }
            else if (c == '.')
            {
                if (!isFloat)
                {
                    isFloat = true;
                    numbers += c;
                }
            }
            else if (char.IsDigit(c))
            {
                numbers += c;
            }
        }

        if (string.IsNullOrEmpty(numbers))
        {
            ExecuteCommand(letters.ToLower());
        }
        else
        {
            if (isFloat)
            {
                ExecuteCommand(letters.ToLower(), float.Parse(numbers));
            }
            else
            {
                ExecuteCommand(letters.ToLower(), int.Parse(numbers));
            }
        }
    }

    private void ExecuteCommand(string str)
    {
        try
        {
            switch (str)
            {
                case "time_stop":
                    Time.timeScale = 0;
                    break;
                case "time_resume":
                    Time.timeScale = 1;
                    break;
                default:
                    resultText.text = "Command not found";
                    return;
            }
            
            resultText.fontSharedMaterial = validInputPreset;
            resultText.text = "Command executed successfully";
        }
        catch (Exception e)
        {
            resultText.fontSharedMaterial = invalidInputPreset;
            resultText.text = e.Message;
            throw;
        }
    }
    
    private void ExecuteCommand(string str, int param)
    {
        try
        {
            switch (str)
            {
                case "level":
                {
                    if (param <= 0)
                    {
                        throw new Exception();
                    }
                    Data.PlayerData.CurrentLevelIndex = param;
                    GameManager.Instance.PlayCurrentLevel();
                    break;
                }
                case "coin":
                    Data.PlayerData.CurrentMoney += param;
                    break;
                case "timescale":
                    Time.timeScale = param;
                    break;
                default:
                    resultText.text = "Command not found";
                    return;
            }
            
            resultText.fontSharedMaterial = validInputPreset;
            resultText.text = "Command executed successfully";
        }
        catch (Exception e)
        {
            resultText.fontSharedMaterial = invalidInputPreset;
            resultText.text = e.Message;
            throw;
        }
    }
    
    private void ExecuteCommand(string str, float param)
    {
        try
        {
            switch (str)
            {
                case "timescale":
                    Time.timeScale = param;
                    break;
                default:
                    resultText.text = "Command not found";
                    return;
            }
            
            resultText.fontSharedMaterial = validInputPreset;
            resultText.text = "Command executed successfully";
        }
        catch (Exception e)
        {
            resultText.fontSharedMaterial = invalidInputPreset;
            resultText.text = e.Message;
            throw;
        }
    }
}
