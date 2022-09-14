using TMPro;
using UnityEngine;

public class CoinTotal : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI currencyTotal;

    private void Awake()
    {
        EventController.CurrencyTotalChanged += UpdateCurrencyText;
        UpdateCurrencyText();
    }

    private void UpdateCurrencyText()
    {
        currencyTotal.text = Data.CurrencyTotal.ToString();
    }
}
