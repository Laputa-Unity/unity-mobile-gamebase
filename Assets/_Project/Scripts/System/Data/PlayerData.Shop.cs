using System.Collections.Generic;
using UnityEngine;

public partial class PlayerData
{
    [SerializeField] private string currentSkin = "Skin_00";
    [SerializeField] private List<string> ownedSkins = new List<string>();

    public string CurrentSkin
    {
        get => currentSkin;
        set => currentSkin = value;
    }

    public List<string> OwnedSkins
    {
        get => ownedSkins;
        set => ownedSkins = value;
    }

    public bool IsOwnedSkin(string identity)
    {
        return ownedSkins.Contains(identity);
    }
}