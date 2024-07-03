using UnityEngine;
using UnityEngine.UI;

public class AreaItem : MonoBehaviour
{
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color unselectedColor;
    [SerializeField] private Image image;
    [SerializeField] private GameObject borderLight;
    [SerializeField] private int multiBonus = 1;

    private void OnEnable()
    {
        image.color = unselectedColor;
    }

    public int MultiBonus
    {
        get => multiBonus;
        set => multiBonus = value;
    }

    public void ActivateBorderLight()
    {
        image.color = selectedColor;
        borderLight.SetActive(true);
    }

    public void DeActivateBorderLight()
    {
        image.color = unselectedColor;
        borderLight.SetActive(false);
    }
}