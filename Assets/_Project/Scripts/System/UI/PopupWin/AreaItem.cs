using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AreaItem : MonoBehaviour
{
    [SerializeField] private GameObject borderLight;
    [SerializeField] private int multiBonus = 1;

    public int MultiBonus
    {
        get => multiBonus;
        set => multiBonus = value;
    }

    public void ActivateBorderLight()
    {
        borderLight.SetActive(true);
    }

    public void DeActivateBorderLight()
    {
        borderLight.SetActive(false);
    }
}