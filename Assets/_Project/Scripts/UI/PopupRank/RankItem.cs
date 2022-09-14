using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RankItem : MonoBehaviour
{
    [Header("Components")]
    public TextMeshProUGUI RankText;
    public Image RankIcon;
    public Image FlatImage;
    public Text PlayerNameText;
    public TextMeshProUGUI LevelText;
    [Header("Data")] 
    public List<Sprite> Top3Sprites;
    private bool IsOnTop3(int rank)
    {
        return rank >= 0 && rank <= 2;
    }
    
    public void Setup(int rank, string countryCode, string playerName, int level)
    {
        RankText.text = $"{rank+1}";
        if (IsOnTop3(rank))
        {
            RankIcon.gameObject.SetActive(true);
            RankIcon.sprite = Top3Sprites[rank];
        }
        else
        {
            RankIcon.gameObject.SetActive(false);
        }
        
        FlatImage.sprite = ConfigController.CountryConfig.GetDataByCode(countryCode).Icon;
        PlayerNameText.text = playerName;
        LevelText.text = $"Level\n{level}";
    }
}