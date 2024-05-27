using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelConfig", menuName = "ScriptableObject/LevelConfig")]
public class LevelConfig : ScriptableObject
{
    public LevelLoopType levelLoopType;
    public List<Level> levels;
    public List<Level> loopLevels;

}

public enum LevelLoopType
{
    Recycle,
    Random,
    Custom,
}