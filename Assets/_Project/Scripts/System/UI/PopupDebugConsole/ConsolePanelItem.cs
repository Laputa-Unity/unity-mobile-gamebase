using UnityEngine;

public class ConsolePanelItem : MonoBehaviour
{
    [SerializeField] private ConsoleTabType consoleTabType;

    public virtual void Setup(ConsoleTabType tabType)
    {
        gameObject.SetActive(consoleTabType == tabType);
    }
}
    