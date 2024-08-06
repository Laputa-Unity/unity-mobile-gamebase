using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsColumn : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI text;

    public void Setup(float height, Color color, string fpsText)
    {
        rectTransform.sizeDelta = new Vector2(10, height);
        image.color = color;
        text.text = fpsText;
    }
}
