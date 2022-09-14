using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/GameConfig")]
public class GameConfig : ScriptableObject
{
    public float DurationPopup = .5f;
    public int MaxLevel = 2;
    public int StartLoopLevel = 1;
}
