#if UNITY_EDITOR
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
    
    [MenuItem("GameBase/Switch Debug %`")]
    public static void SwitchDebug()
    {
        Data.IsTesting = !Data.IsTesting;
        Debug.Log($"<color=Green>Data.IsTesting = {Data.IsTesting}</color>");
    }
    
    [MenuItem("GameBase/Open Scene/Loading Scene %F1")]
    public static void OpenLoadingScene(){
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.LoadingScene}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    }

    [MenuItem("GameBase/Open Scene/Gameplay Scene %F2")]
    public static void OpenGamePlayScene(){
        EditorSceneManager.OpenScene($"Assets/_Project/Scenes/{Constant.GameplayScene}.unity");
        Debug.Log($"<color=Green>Change scene succeed</color>");
    } 
    
    [MenuItem("GameBase/Data/Clear Data %F3")]
    public static void ClearAll()
    {
        PlayerPrefs.DeleteAll();
        Debug.Log($"<color=Green>Clear data succeed</color>");
    }
    
    [MenuItem("GameBase/Data/Add 100k Money")]
    public static void Add100kMoney()
    {
        Data.MoneyTotal += 100000;
        Debug.Log($"<color=Green>Add 100k coin succeed</color>");
    }
}
#endif
