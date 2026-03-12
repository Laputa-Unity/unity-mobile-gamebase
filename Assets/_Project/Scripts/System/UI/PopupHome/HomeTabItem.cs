using UnityEngine;
using UnityEngine.UI;

public class HomeTabItem : MonoBehaviour
{
    [SerializeField] private Image frame;
    private bool _isSelected;

    public bool IsSelected
    {
        get => _isSelected;
        set => _isSelected = value;
    }
}
