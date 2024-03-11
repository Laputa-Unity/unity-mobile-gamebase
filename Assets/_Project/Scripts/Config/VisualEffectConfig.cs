using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VisualEffectConfig", menuName = "ScriptableObject/VisualEffectConfig")]
public class VisualEffectConfig : ScriptableObject
{
    public List<VisualEffectData> visualEffectData;
    
    public VisualEffectData GetVisualEffectData(EffectName effectName)
    {
        return visualEffectData.Find(item => item.name == effectName);
    }
}
