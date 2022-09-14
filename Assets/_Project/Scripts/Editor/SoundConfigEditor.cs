#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
 
[CustomEditor(typeof(SoundConfig))]
public class SoundConfigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var soundConfig = (SoundConfig) target;
 
        if(GUILayout.Button("Update sound list", GUILayout.Height(40)))
        {
            soundConfig.UpdateSoundDatas();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif