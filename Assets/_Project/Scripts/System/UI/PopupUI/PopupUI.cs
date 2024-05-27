using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupUI : Popup
{
    public void Debugging()
    {
        Data.PlayerData.CurrentMoney += 100;
    }
}
