using System;
using System.Collections.Generic;
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
    
}

[Serializable]
public class SoundData
{
    public SoundName name;
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
    CoinMoving,
}
