using System.Collections.Generic;
using CustomInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualEffectConfig", menuName = "ScriptableObject/VisualEffectConfig")]
public class VisualEffectConfig : ScriptableObject
{
    [TableList(Draggable = true, HideAddButton = false, HideRemoveButton = false, AlwaysExpanded = false)] public List<VisualEffectData> visualEffectData;
    
    public VisualEffectData GetVisualEffectData(EffectName effectName)
    {
        return visualEffectData.Find(item => item.name == effectName);
    }
}
