using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public class GameBase : EditorWindow
{
    void OnGUI()
    {
        GUILayout.BeginHorizontal(EditorStyles.toolbar);
        GUILayout.EndHorizontal();
    }

    [MenuItem("GameBase/Open Scene/Loading Scene %F1")]
    public static void OpenLoadingScene(){
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/LoadingScene.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("GameBase/Open Scene/Gameplay Scene %F2")]
    public static void OpenGamePlayScene(){
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/GameplayScene.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("GameBase/Debug/Add 100k Money")]
    public static void Add100kMoney()
    {
        Data.PlayerData.currentMoney += 100000;
        Debug.Log($"<color=Green>Add 100k coin succeed</color>");
    }
    
    [MenuItem("GameBase/Debug/Switch Debug %`")]
    public static void SwitchDebug()
    {
        Data.PlayerData.isTesting = !Data.PlayerData.isTesting;
        Debug.Log($"<color=Green>Data.IsTesting = {Data.PlayerData.isTesting}</color>");
    }
}