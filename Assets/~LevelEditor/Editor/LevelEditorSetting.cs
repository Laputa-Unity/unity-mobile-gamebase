using UnityEngine;

[CreateAssetMenu(fileName = "LevelEditorSetting", menuName = "ScriptableObject/LevelEditorSetting")]
public class LevelEditorSetting : ScriptableObject
{
    public Vector2 windowMinSize = new Vector2(1000, 600);
    public string levelDirectory = "Assets/_Project/Resources/Levels";
    public string prefabDirectory = "Assets/_Project/Resources/LevelDesign";
}
