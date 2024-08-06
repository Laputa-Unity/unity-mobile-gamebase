using UnityEngine;

public class ConsoleAreaItem : MonoBehaviour
{
    [SerializeField] private bool activeGroup;
    [SerializeField] private GameObject group;

    void Start()
    {
        Setup();
    }

    public void OnClickTitleFrame()
    {
        group.SetActive(!group.activeSelf);
    }

    private void Setup()
    {
        group.SetActive(activeGroup);
    }

    private void OnValidate()
    {
        Setup();
    }
}
