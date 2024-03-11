using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PopupConfig", menuName = "ScriptableObject/PopupConfig")]
public class PopupConfig : ScriptableObject
{
    public float durationPopup = .5f;
    public List<Popup> popups;
    
    
}
