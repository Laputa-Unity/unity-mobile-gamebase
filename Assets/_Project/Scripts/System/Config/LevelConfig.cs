using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObject/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public LevelLoopType levelLoopType = LevelLoopType.Recycle;
    public int maxLevel;
    public int startLoopLevel;
}

public enum LevelLoopType
{
    Recycle,
    Random,
}