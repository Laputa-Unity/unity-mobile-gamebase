using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/GameConfig")]
public class GameConfig : ScriptableObject
{
    private bool isTesting;

    public bool IsTesting
    {
        get => isTesting;
        set
        {
            isTesting = value;
            Observer.DebugChanged?.Invoke();
        }
    }
}
