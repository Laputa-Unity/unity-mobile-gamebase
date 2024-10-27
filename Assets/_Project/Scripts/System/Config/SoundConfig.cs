using System;
using System.Collections.Generic;
using System.Linq;
using CustomInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName ="SoundConfig",menuName = "ScriptableObject/SoundConfig")]
public class SoundConfig : ScriptableObject
{
    public List<SoundData> soundData;

    public SoundData GetSoundDataByType(SoundName soundName)
    {
        return soundData.Find(item => item.name == soundName);
    }


    [Button]
    public void UpdateSoundData()
    {
        for (int i = 0; i < Enum.GetNames(typeof(SoundName)).Length; i++)
        {
            SoundData data = new SoundData
            {
                name = (SoundName) i
            };
            if (IsItemExistedBySoundType(data.name)) continue;
            soundData.Add(data);
        }

        soundData = soundData.GroupBy(elem => elem.name).Select(group => group.First()).ToList();
    }

    private bool IsItemExistedBySoundType(SoundName soundName)
    {
        foreach (SoundData item in soundData)
        {
            if (item.name == soundName)
            {
                return true;
            }
        }

        return false;
    }
}


[Serializable]
public class SoundData
{
    [EnumExtend] public SoundName name;
    public float delayTime;
    public List<AudioClip> clips;

    public AudioClip GetRandomAudioClip()
    {
        if (clips.Count > 0)
        {
            return clips[Random.Range(0, clips.Count)];
        }

        return null;
    }
}   

public enum SoundName
{
    Background,
    Music,
    ClickButton,
    PurchaseCompleted,
    PurchaseFailed,
    CollectCoin,
    NukeExplosion,
SpawnCoin,
CaribeanThemeSong, BackgroundIngame, SpinWheel, }
