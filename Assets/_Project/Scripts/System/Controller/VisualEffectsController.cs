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
        if (vfxData != null && vfxData.GetRandomEffect() != null)
        {
            GameObject randomEffect = vfxData.GetRandomEffect();
            GameObject effect = LeanPool.Spawn(randomEffect, position, Quaternion.identity, parent);
            effect.transform.localPosition = position;
        }
        else
        {
            Debug.LogWarning("<color=Red> Missing visual effect </color>");
        }
    }
    
    public void SpawnEffect(EffectName effectName, Vector3 position, Transform parent, float timeDestroy)
    {
        VisualEffectData vfxData = vfxConfig.GetVisualEffectData(effectName);
        if (vfxData != null && vfxData.GetRandomEffect() != null)
        {
            GameObject randomEffect = vfxData.GetRandomEffect();
            GameObject effect = LeanPool.Spawn(randomEffect, position, Quaternion.identity, parent);
            effect.transform.localPosition = position;
            LeanPool.Despawn(effect, timeDestroy);
        }
        else
        {
            Debug.LogWarning("<color=Red> Missing visual effect </color>");
        }
    }
    
    public void SpawnEffect(EffectName effectName, Vector3 position, Transform parent, Vector3 localScale)
    {
        VisualEffectData vfxData = vfxConfig.GetVisualEffectData(effectName);
        if (vfxData != null && vfxData.GetRandomEffect() != null)
        {
            GameObject randomEffect = vfxData.GetRandomEffect();
            GameObject effect = LeanPool.Spawn(randomEffect, parent);
            effect.transform.localPosition = position;
            effect.transform.localScale = localScale;
        }
        else
        {
            Debug.LogWarning("<color=Red> Missing visual effect </color>");
        }
    }
    
    public void SpawnEffect(EffectName effectName, Vector3 position, Transform parent, Vector3 localScale, float timeDestroy)
    {
        VisualEffectData vfxData = vfxConfig.GetVisualEffectData(effectName);
        if (vfxData != null && vfxData.GetRandomEffect() != null)
        {
            GameObject randomEffect = vfxData.GetRandomEffect();
            GameObject effect = LeanPool.Spawn(randomEffect, parent);
            effect.transform.localPosition = position;
            effect.transform.localScale = localScale;
            LeanPool.Despawn(effect, timeDestroy);
        }
        else
        {
            Debug.LogWarning("<color=Red> Missing visual effect </color>");
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
    SparkCoin,
    NukeExplosion,
}