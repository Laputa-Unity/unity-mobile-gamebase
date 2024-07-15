using System;
using TMPro;
using UnityEngine;

public class CommandPanelItem : ConsolePanelItem
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private Material validInputPreset;
    [SerializeField] private Material invalidInputPreset;

    public void OnClickRun()
    {
        var text = inputField.text;
        text = text.Replace(" ", "");
        string letters = string.Empty;
        string numbers = string.Empty;

        foreach (char c in text)
        {
            if (char.IsLetter(c))
            {
                letters += c;
            }
            else if (char.IsDigit(c))
            {
                numbers += c;
            }
        }

        if (numbers.Length == 0)
        {
            ExecuteCommand(letters);
        }
        else
        {
            ExecuteCommand(letters, int.Parse(numbers));
        }
    }

    private void ExecuteCommand(string str)
    {
        try
        {
            switch (str)
            {
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
                case "lv":
                {
                    if (param <= 0)
                    {
                        throw new Exception();
                    }
                    Data.PlayerData.CurrentLevelIndex = param - 1;
                    GameManager.Instance.PlayCurrentLevel();
                    break;
                }
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
