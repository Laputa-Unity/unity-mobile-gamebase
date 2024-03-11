using System;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;


public class VisualEffectsController : SingletonDontDestroy<VisualEffectsController>
{
    [SerializeField] private VisualEffectConfig vfxConfig;
    
    public void SpawnEffect(EffectName effectName, Vector3 position, Transform parent)
    {
        VisualEffectData vfxData = vfxConfig.GetVisualEffectData(effectName);
        if (vfxData != null)
        {
            GameObject randomEffect = vfxData.GetRandomEffect();
            GameObject effect = LeanPool.Spawn(randomEffect, parent);
            effect.transform.position = position;
        }
    }
    
    public void SpawnEffect(EffectName effectName, Vector3 position, Transform parent, Vector3 localScale)
    {
        VisualEffectData vfxData = vfxConfig.GetVisualEffectData(effectName);
        if (vfxData != null)
        {
            GameObject randomEffect = vfxData.GetRandomEffect();
            GameObject effect = LeanPool.Spawn(randomEffect, parent);
            effect.transform.position = position;
            effect.transform.localScale = localScale;
        }
    }
    
    public void SpawnEffect(EffectName effectName, Vector3 position, Transform parent, Vector3 localScale, float timeDestroy)
    {
        VisualEffectData vfxData = vfxConfig.GetVisualEffectData(effectName);
        if (vfxData != null)
        {
            GameObject randomEffect = vfxData.GetRandomEffect();
            GameObject effect = LeanPool.Spawn(randomEffect, parent);
            effect.transform.position = position;
            effect.transform.localScale = localScale;
            LeanPool.Despawn(effect, timeDestroy);
        }
    }
}

[Serializable]
public class VisualEffectData
{
    public EffectName name;
    public List<GameObject> effects;


    public GameObject GetRandomEffect()
    {
        return effects[Random.Range(0, effects.Count)];
    }
}

public enum EffectName
{
    Default,
}