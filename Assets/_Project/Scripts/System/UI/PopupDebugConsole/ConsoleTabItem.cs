using UnityEngine;

public class ConsoleTabItem : MonoBehaviour
{
    [SerializeField] private ConsoleTabType consoleTabType;
    [SerializeField] private CustomButton customButton;

    public void Setup(ConsoleTabType tabType)
    {
        customButton.Interactable = consoleTabType != tabType;
    }

    public void OnClickSelect()
    {
        SoundController.Instance.PlayFX(SoundName.ClickButton);
        Observer.SelectConsoleTab?.Invoke(consoleTabType);
    }
}
