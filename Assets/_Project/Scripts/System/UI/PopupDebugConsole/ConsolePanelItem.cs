using UnityEngine;

public class ConsolePanelItem : MonoBehaviour
{
    protected virtual ConsoleTabType consoleTabType { get; set; }

    public virtual void Setup(ConsoleTabType tabType)
    {
        gameObject.SetActive(consoleTabType == tabType);
    }
}
    