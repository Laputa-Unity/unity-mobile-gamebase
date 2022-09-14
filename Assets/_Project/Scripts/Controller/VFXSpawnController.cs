using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class VFXSpawnController : Singleton<VFXSpawnController>
{
    public List<VisualEffectData> VisualEffectDatas;
    void Awake()
    {
        DontDestroyOnLoad(this);
    }

    public VisualEffectData GetVisualEffectDataByType(VisualEffectType visualEffectType)
    {
        return VisualEffectDatas.Find(item => item.VisualEffectType == visualEffectType);
    }
    
    public void SpawnVFX(VisualEffectType visualEffectType,Vector3 position, Transform parent, Vector3? localScale = null, bool isDestroyedOnEnd = true, float timeDestroy = 3f)
    {
        // Get vfx data
        VisualEffectData visualEffectData = GetVisualEffectDataByType(visualEffectType);
        if (visualEffectData == null) return;
        GameObject randomEffect = visualEffectData.GetRandomEffect();
        if (randomEffect == null) return;
        // Spawn vfx
        GameObject vfxSpawn = Instantiate(randomEffect, parent, false);
        vfxSpawn.transform.position = position;
        if (localScale != null) vfxSpawn.transform.localScale = localScale.Value;
        if (isDestroyedOnEnd) Destroy(vfxSpawn, timeDestroy);
    }
    
    private bool IsItemExistedByVisualEffectType(VisualEffectType visualEffectType)
    {
        foreach (VisualEffectData item in VisualEffectDatas)
        {
            if (item.VisualEffectType == visualEffectType)
            {
                return true;
            }
        }

        return false;
    }

    public void UpdateVFXs()
    {
        for (int i = 0; i < Enum.GetNames(typeof(VisualEffectType)).Length; i++)
        {
            VisualEffectData visualEffectData = new VisualEffectData();
            visualEffectData.VisualEffectType = (VisualEffectType) i;
            if (IsItemExistedByVisualEffectType(visualEffectData.VisualEffectType)) continue;
            VisualEffectDatas.Add(visualEffectData);
        }

        VisualEffectDatas = VisualEffectDatas.GroupBy(elem => elem.VisualEffectType).Select(group => group.First()).ToList();
    }
}

[Serializable]
public class VisualEffectData
{
    public List<GameObject> EffectList;
    public VisualEffectType VisualEffectType;

    public GameObject GetRandomEffect()
    {
        return EffectList[Random.Range(0, EffectList.Count)];
    }
}

public enum VisualEffectType
{
    Default,
}

#if UNITY_EDITOR
[CustomEditor(typeof(VFXSpawnController))]
public class VFXSpawnControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VFXSpawnController vfxSpawnController = (VFXSpawnController) target;
        
        DrawDefaultInspector();

        if (GUILayout.Button("Update VFXs", GUILayout.Height(60)))
        {
            vfxSpawnController.UpdateVFXs();
        }
        
        serializedObject.ApplyModifiedProperties();
    }
}
#endif


