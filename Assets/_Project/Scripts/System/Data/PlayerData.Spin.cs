using System;
using UnityEngine;

public partial class PlayerData
{
    [SerializeField] private string lastSpin = DateTime.Now.AddDays(-1).ToString();

    public string LastSpin
    {
        get => lastSpin;
        set => lastSpin = value;
    }
}
