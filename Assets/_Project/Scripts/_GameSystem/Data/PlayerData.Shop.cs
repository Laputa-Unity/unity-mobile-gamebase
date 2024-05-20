using System.Collections.Generic;

public partial class PlayerData
{
    public string currentSkin = "Skin_01";
    public List<string> ownedSkins = new List<string>();

    public bool IsOwnedSkin(string identity)
    {
        return ownedSkins.Contains(identity);
    }
}