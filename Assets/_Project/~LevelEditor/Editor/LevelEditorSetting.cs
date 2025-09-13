using UnityEngine;

[CreateAssetMenu(fileName = "LevelEditorSetting", menuName = "ScriptableObject/LevelEditorSetting")]
public class LevelEditorSetting : ScriptableObject
{
    public Vector2 windowMinSize = new Vector2(1000, 600);
    public string levelDirectory = "Assets/_Project/Resources/Levels";
    [Header("Level Grid Config")]
    [Range(1, 14)] public int rowCount = 6;
    [Range(1, 10)] public int columnCount = 6;
    
}
