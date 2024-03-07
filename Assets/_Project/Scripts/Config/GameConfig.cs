using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("UI config")]
    public float durationPopup = .5f;
    public int watchAdsMoney = 1000;
    [Header("Level config")] 
    public LevelLoopType levelLoopType;
    public int maxLevel = 2;
    public int startLoopLevel;
    [Header("Gameplay config")]
    public int winLevelMoney = 100;
    
}

public enum LevelLoopType
{
    NormalLoop,
    RandomLoop,
}